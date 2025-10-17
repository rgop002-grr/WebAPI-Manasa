using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_Manasa.Models;
using WebAPI_Manasa.Models.Controllers;
using static WebAPI_Manasa.Controllers.RefreshTokenController;

namespace WebAPI_Manasa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        // In-memory dictionary for storing refresh tokens
        private static Dictionary<string, string> userRefreshTokens = new Dictionary<string, string>();

        [HttpPost("Generate")]
        public IActionResult GenerateToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Role))
                return BadRequest("Username and Role are required.");

            string secretKey = "My_Super_Secret_Key_1234567899999";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, request.Role)
            };

            // Generate Access Token
            var accessToken = new JwtSecurityToken(
                issuer: "Manasa",
                audience: "MyAppUsers",
                claims: claims,
                expires: DateTime.Now.AddSeconds(30), // short life for testing
                signingCredentials: creds
            );

            // Generate Refresh Token (random string)
            var refreshToken = Guid.NewGuid().ToString();

            // Store refresh token with username
            userRefreshTokens[request.Username] = refreshToken;

            return Ok(new RefreshTokenModel
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken,
                Expiration = accessToken.ValidTo
            });
        }

        // Helper method to validate refresh token
        public static bool ValidateRefreshToken(string username, string refreshToken)
        {
            return userRefreshTokens.ContainsKey(username) && userRefreshTokens[username] == refreshToken;
        }


        // Helper to generate new access token
        public static string GenerateAccessToken(string username, string role)
        {
            string secretKey = "My_Super_Secret_Key_1234567899999";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var newToken = new JwtSecurityToken(
                issuer: "Manasa",
                audience: "MyAppUsers",
                claims: claims,
                expires: DateTime.Now.AddSeconds(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(newToken);
        }

        [HttpPost("Refresh")]
        public IActionResult Refresh([FromBody] string token)
        {
            var storedToken = userRefreshTokens.FirstOrDefault(t => t.Value == token);

            if (string.IsNullOrEmpty(storedToken.Key))
                return Unauthorized("Invalid or expired refresh token");

            // Example username and role (you can store these properly)
            var username = storedToken.Key;
            var role = "User"; // or retrieve actual role if available

            // Generate new access token
            var newAccessToken = GenerateAccessToken(username, role);

            return Ok(new
            {
                AccessToken = newAccessToken
            });
        }
    }
}


