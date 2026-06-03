using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using roomvision.application.Interfaces.Servicies.RoomServices;
using roomvision.domain.Common;
using roomvision.presentation.Request.RoomRequest;
using roomvision.presentation.Response.RoomResponse;

namespace roomvision.presentation.Controllers
{
    [Route("api/v1/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IScanRoomService _scanRoomService;
        private readonly IScanResultService _scanResultService;
        private readonly IGetRooms _getRooms;
        private readonly ICreateRoomService _createRoomService;

        public RoomController(IScanRoomService scanRoomService, IScanResultService scanResultService, IGetRooms getRooms, ICreateRoomService createRoomService)
        {
            _scanRoomService = scanRoomService;
            _scanResultService = scanResultService;
            _getRooms = getRooms;
            _createRoomService = createRoomService;
        }

        [HttpPost("scan")]
        [Authorize(Roles = "Room")]
        public async Task<IActionResult> ScanRoom([FromQuery] string room)
        {
            if (string.IsNullOrEmpty(room))
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
        [Authorize(Roles = "Account")]
        public async Task<IActionResult> GetScanResult([FromQuery] string room)
        {
            if (string.IsNullOrEmpty(room))
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
                TotalFaces = result.Value.TotalFaces,
                TotalUnknown = result.Value.IdentifiedFaces!.Count(f => f == "Unknown"),
                ScannedAt = result.Value.ScannedAt
            };

            return Ok(mappedResult);
        }

        [HttpGet("get")]
        [Authorize(Roles = "Account")]
        public async Task<IActionResult> GetRooms([FromQuery] int page, [FromQuery] int pageSize = 6)
        {
            var result = await _getRooms.Execute(page, pageSize);
            return Ok(result.Value);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoom request)
        {
            var result = await _createRoomService.Execute(request.RoomName!, request.Password!);

            if (result.IsFailure)
            {
                return result.ErrorType switch
                {
                    ErrorTypes.Conflict => Conflict(new { Error = result.Error }),
                    _ => StatusCode(500, new { Error = "An unexpected error occurred." })
                };
            }

            return Ok();
        }
    }
}
