using QrVision.Domain.Entities;

namespace QrVision.Domain.Interfaces.Services
{
    public interface IVideoQrCodeExtractionService
    {
        Task<List<QrCodeResult>> ExtractAndDecodeQrCodeAsync(string videoPath, CancellationToken token);
    }
}
