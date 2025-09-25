using QrVision.Domain.Enums;

namespace QrVision.Domain.Dtos
{
    public class VideoAnalysisResultDto
    {
        public Guid AnalysisId { get; set; }
        public VideoAnalysisStatus Status { get; set; }
        public List<QrCodeResultDto> QrCodes { get; set; } = [];
    }
}
