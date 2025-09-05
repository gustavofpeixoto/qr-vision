using QrVision.Domain.Dtos;

namespace QrVision.Domain.Interfaces.Services
{
    public interface IGetAnalysisResultService
    {
        public Task<VideoAnalysisResultDto> ExecuteAsync(Guid analysisId);
    }
}
