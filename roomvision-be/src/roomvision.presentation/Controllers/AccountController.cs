using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using roomvision.domain.Common;
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
        private readonly IGetAccountNameService _getAccountNameService;
        private readonly IGetAccountsService _getAccountsService;

        public AccountController(
            ICreateAccountService createAccountService,
            IResetAccountPasswordService resetAccountPasswordService,
            IGetAccountNameService getAccountNameService,
            IGetAccountsService getAccountsService)
        {
            _createAccountService = createAccountService;
            _resetAccountPasswordService = resetAccountPasswordService;
            _getAccountNameService = getAccountNameService;
            _getAccountsService = getAccountsService;
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

        [HttpGet("get-account-name")]
        [Authorize(Roles = "Account")]
        public async Task<IActionResult> GetAccountName()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

            var result = await _getAccountNameService.Execute(int.Parse(userId));

            if (result.IsFailure)
            {
                return result.ErrorType switch
                {
                    ErrorTypes.NotFound => NotFound(new { Error = result.Error }),
                    _ => StatusCode(500, new { Error = "An unexpected error occurred." })
                };
            }

            return Ok(new { AccountName = result.Value });
        }

        [HttpGet("list")]
        [Authorize(Roles = "Account")]
        public async Task<IActionResult> GetAccounts([FromQuery] int page, [FromQuery] int pageSize = 6)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var result = await _getAccountsService.Execute(page, pageSize, int.Parse(userId));
            return Ok(result.Value);
        }


    }
}