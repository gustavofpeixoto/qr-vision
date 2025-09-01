using Microsoft.AspNetCore.Mvc;
using QrVision.Domain.Constants;
using QrVision.Domain.Interfaces.Services;

namespace QrVision.Api.Controllers
{
    [ApiController]
    [Route("api/video")]
    public class VideoController(IProcessVideoService processVideoService) : ControllerBase
    {
        [HttpPost("/upload")]
        public async Task<IActionResult> UploadAsync(IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0) 
                return BadRequest(ErrorMessagesConst.FileNotFound);

            await using var stream = videoFile.OpenReadStream();
            await processVideoService.ExecuteAsync(stream, videoFile.Name);

            return Accepted();
        }
    }
}
