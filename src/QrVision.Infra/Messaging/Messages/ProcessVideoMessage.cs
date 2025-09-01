namespace QrVision.Infra.Messaging.Messages
{
    public class ProcessVideoMessage(Guid videoAnalysisId, string videoFilePath)
    {
        public Guid VideoAnalysisId { get; private set; } = videoAnalysisId;
        public string VideoFilePath { get; private set; } = videoFilePath;
    }
}
