using QrVision.Domain.Entities;

namespace QrVision.Domain.Dtos
{
    public class QrCodeResultDto
    {
        public string Content { get; set; } = default!;
        public double TimestampInSeconds { get; set; }

        public static explicit operator QrCodeResultDto(QrCodeResult qrCodeResult)
        {
            var qrCodeResultDto = new QrCodeResultDto
            {
                Content = qrCodeResult.Content,
                TimestampInSeconds = qrCodeResult.TimestampInSeconds,
            };

            return qrCodeResultDto;
        }
    }
}
