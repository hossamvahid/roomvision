using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using roomvision.application.Common;
using roomvision.application.Interfaces.Servicies;
using roomvision.presentation.Request;

namespace roomvision.presentation.Controllers
{
    [ApiController]
    [Route("api/v1/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("user")]
        public async Task<IActionResult> UserAuthenticate([FromBody] AccountAuthentication request)
        {
            var result = await _authenticationService.UserAuthenticateAsync(request.Email!, request.Password!);

            if (result.IsFailure)
            {
                return result.ErrorType switch
                {
                    ErrorTypes.Unauthorized => Unauthorized(new { Error = result.Error }),
                    _ => StatusCode(500, new { Error = "An unexpected error occurred." })
                };
            }

            return Ok(new { Token = result.Value });
        }

        [HttpPost("room")]
        public async Task<IActionResult> RoomAuthenticate([FromBody] RoomAuthentication request)
        {
            var result = await _authenticationService.RoomAuthenticateAsync(request.RoomName!, request.Password!);

            if (result.IsFailure)
            {
                return result.ErrorType switch
                {
                    ErrorTypes.Unauthorized => Unauthorized(new { Error = result.Error }),
                    _ => StatusCode(500, new { Error = "An unexpected error occurred." })
                };
            }

            return Ok(new { Token = result.Value });
        }
        

    }
}