using QrVision.Domain.Constants;
using QrVision.Domain.Interfaces.Repositories;
using QrVision.Domain.Interfaces.Services;
using QrVision.Domain.Serialization;
using QrVision.Infra.Messaging;
using QrVision.Infra.Messaging.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;

namespace QrVision.Worker.Consumers
{
    public class ProcessVideoMessageConsumer(
        RabbitMqConnectionManager rabbitMqConnectionManager,
        IVideoAnalysisRepository videoAnalysisRepository,
        IVideoQrCodeExtractionService videoQrCodeExtractionService)
        : RabbitMqConsumer(rabbitMqConnectionManager)
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ExecuteAsync(QueueNameConst.ProcessVideo, stoppingToken);
            await Task.CompletedTask;
        }

        protected async Task ExecuteAsync(string queueName, CancellationToken cancellationToken = default)
        {
            Log.Information("Iniciando execução do consumer: {consumer}", nameof(ProcessVideoMessageConsumer));

            await ConnectAsync(queueName, cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(Channel);

            consumer.ReceivedAsync += ProcessMessageAsync;

            await Channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);

            Log.Information("Finalizando execução do consumer: {consumer}", nameof(ProcessVideoMessageConsumer));
        }

        protected override async Task ProcessMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                Log.Information("Serializando conteúdo da mensagem para o consumer: {consumer}", nameof(ProcessVideoMessageConsumer));

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deserializedMessage = JsonSerializerHelper.Deserialize<ProcessVideoMessage>(message);

                Log.Information("Iniciando processo de extração de conteúdo dos QR Codes identificados no vídeo. Id da análise: {VideoAnalysisId} | Consumer: {consumer}", deserializedMessage.VideoAnalysisId, nameof(ProcessVideoMessageConsumer));

                var qrCodeResults = await videoQrCodeExtractionService.ExtractAndDecodeQrCodeAsync(deserializedMessage.VideoFilePath, new CancellationToken());

                Log.Information("Obtendo solicitação de análise de vídeo na base de dados. Id da análise: {VideoAnalysisId} | Consumer: {consumer}", deserializedMessage.VideoAnalysisId, nameof(ProcessVideoMessageConsumer));

                var videoAnalysis = await videoAnalysisRepository.GetByIdAsync(deserializedMessage.VideoAnalysisId);

                videoAnalysis.AddResults(qrCodeResults);
                
                Log.Information("Obtendo solicitação de análise de vídeo na base de dados. Id da análise: {VideoAnalysisId} | Consumer: {consumer}", deserializedMessage.VideoAnalysisId, nameof(ProcessVideoMessageConsumer));

                await videoAnalysisRepository.UpdateAsync(videoAnalysis);
                await Channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                await Channel.BasicNackAsync(ea.DeliveryTag, false, false);
                Log.Warning("Erro: {@e}", e);
            }
        }
    }
}
