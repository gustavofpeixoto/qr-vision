using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QrVision.Domain.Interfaces.Repositories;
using QrVision.Infra.Repositories;
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

            return services;
        }
    }
}
