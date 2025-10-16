using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI_Manasa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolebasedController : ControllerBase
    {

        // 👇 Only Admins can access this endpoint
        [Authorize(Roles = "Admin")]
        [HttpGet("AdminOnly")]
        public IActionResult AdminOnly()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok($"Hello {username}! You are an {role}, and you can access this Admin-only endpoint.");
        }

        // 👇 Only Users can access this endpoint
        [Authorize(Roles = "User")]
        [HttpGet("UserOnly")]
        public IActionResult UserOnly()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok($"Hello {username}! You are a {role}, and you can access this User-only endpoint.");
        }

        // 👇 Both Admin and User can access this
        [Authorize(Roles = "HR")]
        [HttpGet("Common")]
        public IActionResult Common()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok($"Welcome {username}! You are a {role}, and you can access this common endpoint.");
        }

        // 👇 Public endpoint (no token required)
        [AllowAnonymous]
        [HttpGet("Public")]
        public IActionResult Public()
        {
            return Ok("This is a public endpoint — no token required.");
        }
    }
}
