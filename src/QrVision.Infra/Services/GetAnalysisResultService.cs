using QrVision.Domain.Dtos;
using QrVision.Domain.Interfaces.Repositories;
using QrVision.Domain.Interfaces.Services;
using Serilog;

namespace QrVision.Infra.Services
{
    public class GetAnalysisResultService(IVideoAnalysisRepository videoAnalysisRepository) : IGetAnalysisResultService
    {
        public async Task<VideoAnalysisResultDto> ExecuteAsync(Guid analysisId)
        {
            Log.Information("Iniciando busca do resultado de análise | analysisId: {analysisId}", analysisId);

            var videoAnalysis = await videoAnalysisRepository.GetByIdAsync(analysisId);

            if (videoAnalysis == null)
            {
                Log.Warning("Nenhuma análise encontrada para | analysisId: {analysisId}", analysisId);

                return new VideoAnalysisResultDto();
            }

            var videoAnalysisResult = new VideoAnalysisResultDto { AnalysisId = videoAnalysis.Id };

            videoAnalysisResult.QrCodes.AddRange(videoAnalysis.QrCodeResults.Select(qrCode => (QrCodeResultDto)qrCode));

            return videoAnalysisResult;
        }
    }
}
