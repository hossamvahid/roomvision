using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public AccountController(ICreateAccountService createAccountService)
        {
            _createAccountService = createAccountService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccount request)
        {
            var result = await _createAccountService.Execute(request.Email!, request.Name!);

            if(result.IsFailure)
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