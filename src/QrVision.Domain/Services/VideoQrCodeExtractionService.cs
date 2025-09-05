using System.Collections.Concurrent;
using FFMpegCore;
using QrVision.Domain.Entities;
using QrVision.Domain.Interfaces.Services;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QrVision.Domain.Services
{
    public class VideoQrCodeExtractionService : IVideoQrCodeExtractionService
    {
        private const string _sharedVideoPath = "/app/videos";

        public async Task<List<QrCodeResult>> ExtractAndDecodeQrCodeAsync(string videoFileName, CancellationToken token)
        {
            var videoFilePath = Path.Combine(_sharedVideoPath, videoFileName);
            var results = new ConcurrentDictionary<string, QrCodeResult>();
            var mediaInfo = await FFProbe.AnalyseAsync(videoFilePath);
            var duration = mediaInfo.Duration; var frameRate = 1;
            var totalFrames = (int)Math.Floor(duration.TotalSeconds * frameRate);

            if (totalFrames <= 0) { return []; }

            await Parallel.ForEachAsync(
                Enumerable.Range(0, totalFrames), new ParallelOptions 
                { 
                    CancellationToken = token, MaxDegreeOfParallelism = Environment.ProcessorCount 
                },
                async (i, ct) =>
                {
                    var timestamp = TimeSpan.FromSeconds(i / (double)frameRate);
                    var tempFramePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");

                    try
                    {
                        await FFMpeg.SnapshotAsync(videoFilePath, tempFramePath, new System.Drawing.Size(640, 480), timestamp);
                        using var image = await Image.LoadAsync<Rgba32>(tempFramePath, ct);

                        var reader = new ZXing.ImageSharp.BarcodeReader<Rgba32>();
                        var qrResult = reader.Decode(image);

                        if (qrResult != null)
                        {
                            results.TryAdd(qrResult.Text, new QrCodeResult
                            {
                                Content = qrResult.Text,
                                TimestampInSeconds = timestamp.TotalSeconds
                            });
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Log.Warning("Processamento paralelo cancelado.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Erro ao processar frame no timestamp {timestamp}: {ex.Message}", timestamp, ex.Message);
                    }
                    finally
                    {
                        if (File.Exists(tempFramePath)) File.Delete(tempFramePath);
                    }
                });

            if (File.Exists(videoFilePath)) File.Delete(videoFilePath);

            return [.. results.Values.OrderBy(r => r.TimestampInSeconds)];
        }
    }
}