namespace QrVision.Domain.Interfaces.Services
{
    public interface IUploadVideoService
    {
        public Task ExecuteAsync(Stream fileStream, string originalFileName);
    }
}
