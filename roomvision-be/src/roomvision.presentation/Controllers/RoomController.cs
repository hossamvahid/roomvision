using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using roomvision.application.Interfaces.Servicies.RoomServices;
using roomvision.domain.Common;

namespace roomvision.presentation.Controllers
{
    [Route("api/v1/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IScanRoomService _scanRoomService;

        public RoomController(IScanRoomService scanRoomService)
        {
            _scanRoomService = scanRoomService;
        }

        [HttpPost("scan")]
        public async Task<IActionResult> ScanRoom([FromQuery] string room)
        {
            if(string.IsNullOrEmpty(room))
            {
                return BadRequest("Room name is required.");
            }

            using var memoryStream = new MemoryStream();
            await Request.Body.CopyToAsync(memoryStream);
            var imageData = memoryStream.ToArray();

            var result = await _scanRoomService.Execute(imageData, room);

            if (result.IsFailure)
            {
                return result.ErrorType switch
                {
                    ErrorTypes.NotFound => NotFound(new { Error = result.Error }),
                    ErrorTypes.Conflict => Conflict(new { Error = result.Error }),
                    ErrorTypes.Validation => BadRequest(new { Error = result.Error }),
                    ErrorTypes.Unauthorized => Unauthorized(new { Error = result.Error }),
                    _ => StatusCode(500, new { Error = "An unexpected error occurred." }),
                };
            }

            return Ok();
        }
    }
}
