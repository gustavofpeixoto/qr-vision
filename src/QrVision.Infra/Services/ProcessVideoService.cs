using QrVision.Domain.Constants;
using QrVision.Domain.Entities;
using QrVision.Domain.Enums;
using QrVision.Domain.Interfaces.Messaging;
using QrVision.Domain.Interfaces.Repositories;
using QrVision.Domain.Interfaces.Services;
using QrVision.Infra.Messaging.Messages;

namespace QrVision.Infra.Services
{
    public class ProcessVideoService( IMessagingProducer messagingProducer,
        IVideoAnalysisRepository videoAnalysisRepository) : IProcessVideoService
    {
        public async Task ExecuteAsync(Stream fileStream, string originalFileName)
        {
            var fileName = await StoreVideoAsync(fileStream, originalFileName);
            var videoAnalysis = new VideoAnalysis(VideoAnalysisStatus.Processing);

            await videoAnalysisRepository.AddAsync(videoAnalysis);

            var message = new ProcessVideoMessage(videoAnalysis.Id, fileName);
            await messagingProducer.SendAsync(message, QueueNameConst.ProcessVideo);
        }

        private static async Task<string > StoreVideoAsync(Stream fileStream, string originalFileName)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(stream);

            return filePath;
        }
    }
}
