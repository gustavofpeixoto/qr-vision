using Microsoft.AspNetCore.Mvc;
using QrVision.Domain.Interfaces.Services;

namespace QrVision.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController(IVideoAnalysisService videoAnalysisService) : ControllerBase
    {
        [HttpPost("/upload")]
        public async Task<IActionResult> UploadAsync(IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            // 1. Salvar o arquivo temporariamente
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(videoFile.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            var result = await videoAnalysisService.ExtractAndDecodeQrCodeAsync(filePath, new CancellationToken());

            return Accepted();
        }
    }
}
