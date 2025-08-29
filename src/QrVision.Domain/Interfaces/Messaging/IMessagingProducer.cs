namespace QrVision.Domain.Interfaces.Messaging
{
    public interface IMessagingProducer
    {
        Task SendAsync<TMessage>(TMessage message, string queue);
    }
}
