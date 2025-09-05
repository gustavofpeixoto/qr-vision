using QrVision.Domain.Constants;
using QrVision.Domain.Entities;
using QrVision.Domain.Interfaces.Messaging;
using QrVision.Domain.Interfaces.Repositories;
using QrVision.Domain.Interfaces.Services;
using QrVision.Infra.Messaging.Messages;

namespace QrVision.Infra.Services
{
    public class UploadVideoService(IMessagingProducer messagingProducer,
        IVideoAnalysisRepository videoAnalysisRepository) : IUploadVideoService
    {
        private const string _sharedVideoPath = "/app/videos";

        public async Task<Guid> ExecuteAsync(Stream fileStream, string originalFileName)
        {
            var fileName = await StoreVideoAsync(fileStream, originalFileName);
            var videoAnalysis = new VideoAnalysis();

            await videoAnalysisRepository.AddAsync(videoAnalysis);

            var message = new ProcessVideoMessage(videoAnalysis.Id, fileName);
            await messagingProducer.SendAsync(message, QueueNameConst.ProcessVideo);

            return videoAnalysis.Id;
        }

        private static async Task<string> StoreVideoAsync(Stream fileStream, string originalFileName)
        {
            if (!Directory.Exists(_sharedVideoPath)) Directory.CreateDirectory(_sharedVideoPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            var filePath = Path.Combine(_sharedVideoPath, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(stream);

            return fileName;
        }
    }
}
