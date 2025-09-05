namespace QrVision.Domain.Interfaces.Services
{
    public interface IUploadVideoService
    {
        public Task<Guid> ExecuteAsync(Stream fileStream, string originalFileName);
    }
}
