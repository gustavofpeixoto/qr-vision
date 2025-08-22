using Microsoft.AspNetCore.Mvc;

namespace QrVision.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        [HttpPost("/upload")]
        public async Task<IActionResult> UploadAsync(IFormFile videoFile)
        {
            return Accepted();
        }
    }
}
