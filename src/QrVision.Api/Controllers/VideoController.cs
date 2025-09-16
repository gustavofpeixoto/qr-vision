using Microsoft.AspNetCore.Mvc;
using QrVision.Domain.Constants;
using QrVision.Domain.Interfaces.Services;

namespace QrVision.Api.Controllers
{
    [ApiController]
    [Route("api/video")]
    public class VideoController(
        IUploadVideoService uploadVideoService,
        IGetAnalysisResultService getAnalysisResultService)
        : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync(IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
                return BadRequest(ErrorMessagesConst.FileNotFound);

            await using var stream = videoFile.OpenReadStream();
            var videoAnalysisId = await uploadVideoService.ExecuteAsync(stream, videoFile.Name);

            return Ok(new { Id = videoAnalysisId });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAnalysisResultAsync(Guid id)
        {
            var videoAnalysisResultDto = await getAnalysisResultService.ExecuteAsync(id);

            if (videoAnalysisResultDto.QrCodes.Count == 0)
                return NotFound();

            return Ok(videoAnalysisResultDto);
        }
    }
}
