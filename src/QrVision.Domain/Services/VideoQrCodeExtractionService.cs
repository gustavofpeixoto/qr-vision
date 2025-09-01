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
        public async Task<List<QrCodeResult>> ExtractAndDecodeQrCodeAsync(string videoPath, CancellationToken token)
        {
            var results = new ConcurrentDictionary<string, QrCodeResult>();
            var mediaInfo = await FFProbe.AnalyseAsync(videoPath);
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
                        await FFMpeg.SnapshotAsync(videoPath, tempFramePath, new System.Drawing.Size(640, 480), timestamp);
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
                        // O Parallel.ForEachAsync foi cancelado, o que é esperado.
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Erro ao processar frame no timestamp {timestamp}: {ex.Message}");
                    }
                    finally
                    {
                        if (File.Exists(tempFramePath)) File.Delete(tempFramePath);
                    }
                });

            if (File.Exists(videoPath)) File.Delete(videoPath);

            return [.. results.Values.OrderBy(r => r.TimestampInSeconds)];
        }
    }
}