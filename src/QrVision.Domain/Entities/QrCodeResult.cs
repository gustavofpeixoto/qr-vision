namespace QrVision.Domain.Entities
{
    public class QrCodeResult
    {
        public string Content { get; set; } = default!;
        public double TimestampInSeconds { get; set; }
    }
}
