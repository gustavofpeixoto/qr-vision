namespace QrVision.Domain.Interfaces.Services
{
    public interface IProcessVideoService
    {
        public Task ExecuteAsync(Stream fileStream, string originalFileName);
    }
}
