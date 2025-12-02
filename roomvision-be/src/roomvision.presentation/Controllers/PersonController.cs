using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using roomvision.domain.Common;
using roomvision.application.Interfaces.Servicies.PersonServices;

namespace roomvision.presentation.Controllers
{
    [Route("api/v1/person")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly ICreatePersonService _createPersonService;

        public PersonController(ICreatePersonService createPersonService)
        {
            _createPersonService = createPersonService;
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

            var result = await _createPersonService.Execute(personName, memoryStream.ToArray());
            if (result.IsFailure)
            {
                return result.ErrorType switch
                {
                    ErrorTypes.NotFound => NotFound(new{Error = result.Error}),
                    ErrorTypes.Conflict => Conflict(new{Error = result.Error}),
                    _ => StatusCode(500, new{Error = result.Error})
                };
            }
            return Ok();
        }
    }
}
