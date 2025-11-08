using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using roomvision.application.Common;
using roomvision.application.Interfaces.Servicies.FaceServices;

namespace roomvision.presentation.Controllers
{
    [Route("api/v1/face")]
    [ApiController]
    public class FaceController : ControllerBase
    {
        private readonly ICreateFaceService _createFaceService;

        public FaceController(ICreateFaceService createFaceService)
        {
            _createFaceService = createFaceService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Account")]
        public async Task<IActionResult> CreateFace([FromForm] string personName, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Photo file is required.");
            }
            
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var result = await _createFaceService.Execute(personName, memoryStream.ToArray());
            if (result.IsFailure)
            {
                return result.ErrorType switch
                {
                    ErrorTypes.NotFound => NotFound(result),
                    _ => StatusCode(500, result)
                };
            }
            return Ok();
        }
    }
}
