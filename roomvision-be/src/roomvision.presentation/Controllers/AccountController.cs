using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roomvision.application.Common;
using roomvision.application.Interfaces.Servicies.AccountServices;
using roomvision.presentation.Request.AccountRequest;

namespace roomvision.presentation.Controllers
{
    [ApiController]
    [Route("api/v1/account")]
    public class AccountController : ControllerBase
    {
        private readonly ICreateAccountService _createAccountService;
        private readonly IResetAccountPasswordService _resetAccountPasswordService;

        public AccountController(
            ICreateAccountService createAccountService,
            IResetAccountPasswordService resetAccountPasswordService)
        {
            _createAccountService = createAccountService;
            _resetAccountPasswordService = resetAccountPasswordService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccount request)
        {
            var result = await _createAccountService.Execute(request.Email!, request.Name!);

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

        [HttpPatch("reset-password")]
        [Authorize(Roles = "Account")]

        public async Task<IActionResult> ResetAccountPassword([FromBody] ResetAccountPassword request)
        {

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var userId = int.Parse(userIdClaim!);

            var result = await _resetAccountPasswordService.Execute(userId, request.NewPassword!);

            if (result.IsFailure)
            {
                return result.ErrorType switch
                {
                    ErrorTypes.NotFound => NotFound(new { Error = result.Error }),
                    _ => StatusCode(500, new { Error = "An unexpected error occurred." })
                };
            }

            return Ok();
        }
        
        
    }
}