using QrVision.Domain.Enums;

namespace QrVision.Domain.Entities
{
    public class VideoAnalysis(VideoAnalysisStatus status)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public VideoAnalysisStatus Status { get; set; } = status;
        public List<QrCodeResult> QrCodeResults { get; set; } = default!;
    }
}
