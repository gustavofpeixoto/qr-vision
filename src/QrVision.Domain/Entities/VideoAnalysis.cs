using QrVision.Domain.Enums;

namespace QrVision.Domain.Entities
{
    public class VideoAnalysis
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; }
        public VideoAnalysisStatus Status { get; private set; } = VideoAnalysisStatus.Processing;
        public List<QrCodeResult> QrCodeResults { get; private set; } = [];

        public void AddResults(List<QrCodeResult> results)
        {
            QrCodeResults.AddRange(results);
            Status = VideoAnalysisStatus.ProcessingCompleted;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
