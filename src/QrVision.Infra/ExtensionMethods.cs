using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QrVision.Domain.Interfaces.Messaging;
using QrVision.Domain.Interfaces.Repositories;
using QrVision.Domain.Interfaces.Services;
using QrVision.Infra.Messaging;
using QrVision.Infra.Repositories;
using QrVision.Infra.Services;
using QrVision.Infra.Settings;

namespace QrVision.Infra
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IVideoAnalysisRepository, VideoAnalysisRepository>(sp =>
            {
                var mongoDbConnectionString = configuration["MONGODB_CONNECTION_STRING"];
                var mongoDbDatabaseName = configuration["MONGODB_DATABASE_NAME"];
                var mongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = mongoDbConnectionString,
                    DatabaseName = mongoDbDatabaseName
                };

                return new VideoAnalysisRepository(mongoDbSettings);
            });

            services.AddSingleton<RabbitMqConnectionManager>();
            services.AddSingleton<IMessagingProducer, RabbitMqMessagingProducer>();
            services.AddScoped<IProcessVideoService, ProcessVideoService>();

            return services;
        }

        public static IConfigurationBuilder AddJsonByFileName(this IConfigurationBuilder configurationBuilder, string jsonFileName)
        {
            var currentDirectory = AppContext.BaseDirectory;
            var filePath = Path.Combine(currentDirectory, $"{jsonFileName}.json");

            return configurationBuilder.AddJsonFile(filePath, optional: false, reloadOnChange: true);
        }
    }
}
