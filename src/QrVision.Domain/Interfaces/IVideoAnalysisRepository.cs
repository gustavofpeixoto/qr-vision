using QrVision.Domain.Entities;

namespace QrVision.Domain.Interfaces
{
    public interface IVideoAnalysisRepository
    {
        Task AddAsync(VideoAnalysis videoAnalysis);
        Task GetByIdAsync(Guid id);
        Task UpdateAsync(VideoAnalysis videoAnalysis);
    }
}
