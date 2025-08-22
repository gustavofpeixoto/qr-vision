using QrVision.Domain.Enums;

namespace QrVision.Domain.Entities
{
    public class VideoAnalysis : Entity
    {
        public VideoAnalysisStatus Status { get; set; }
        public string OriginalFileName { get; set; } = default!;
        public List<QrCodeResult> QrCodeResults { get; set; } = default!;
        public string ErrorMessage { get; set; } = default!;
    }
}
