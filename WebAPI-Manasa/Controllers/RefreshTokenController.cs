using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebAPI_Manasa.Models;

namespace WebAPI_Manasa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private static List<UserSession> _userSessions = new(); // Stores refresh tokens permanently (no expiration)

        public RefreshTokenController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // ✅ Login - Generates Access & Refresh Token
        [HttpPost("login")]
        public IActionResult Login([FromBody] TokenRequest user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Role))
                return BadRequest("Username and Role are required.");

            var tokens = _tokenService.GenerateTokens(user.Username, user.Role);

            // Save user session with refresh token
            _userSessions.Add(new UserSession
            {
                Username = user.Username,
                Role = user.Role,
                RefreshToken = tokens.RefreshToken
            });

            return Ok(tokens);
        }

        // ✅ Refresh - Only needs Refresh Token, returns new Access Token
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh token is required.");

            // Find user by refresh token
            var existingUser = _userSessions.FirstOrDefault(u => u.RefreshToken == refreshToken);
            if (existingUser == null)
                return Unauthorized("Invalid refresh token.");

            // Generate new Access Token only
            var newAccessToken = _tokenService.GenerateAccessToken(existingUser.Username, existingUser.Role);

            return Ok(new { AccessToken = newAccessToken });
        }
    }
}

