using Microsoft.Extensions.DependencyInjection;
using QrVision.Domain.Interfaces.Services;
using QrVision.Domain.Services;

namespace QrVision.Domain
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IVideoQrCodeExtractionService, VideoQrCodeExtractionService>();

            return services;
        }
    }
}
