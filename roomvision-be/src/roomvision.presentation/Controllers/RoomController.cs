using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using roomvision.application.Interfaces.Servicies.RoomServices;
using roomvision.domain.Common;
using roomvision.presentation.Response.RoomResponse;

namespace roomvision.presentation.Controllers
{
    [Route("api/v1/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IScanRoomService _scanRoomService;
        private readonly IScanResultService _scanResultService;

        public RoomController(IScanRoomService scanRoomService, IScanResultService scanResultService)
        {
            _scanRoomService = scanRoomService;
            _scanResultService = scanResultService;
        }

        [HttpPost("scan")]
        [Authorize(Roles = "Room")]
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

        [HttpGet("scan/result")]
        [Authorize(Roles = "Room")]
        public async Task<IActionResult> GetScanResult([FromQuery] string room)
        {
            if(string.IsNullOrEmpty(room))
            {
                return BadRequest("Room name is required.");
            }

            var result = await _scanResultService.Execute(room);
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

            var mappedResult = new ScanResult
            {
                IdentifiedFaces = result.Value!.IdentifiedFaces,
                ScannedAt = result.Value.ScannedAt
            };
            
            return Ok(mappedResult);
        }
    }
}
