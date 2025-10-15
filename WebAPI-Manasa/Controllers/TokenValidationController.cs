using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI_Manasa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenValidationController : ControllerBase
    {
        [Authorize]
        [HttpGet("Validate")]
        public IActionResult ValidateToken()
        {
            return Ok("Authorized successfully ✅");
        }
    }
}
