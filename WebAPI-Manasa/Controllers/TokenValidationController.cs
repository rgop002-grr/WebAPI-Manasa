using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI_Manasa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenValidationController : ControllerBase
    {
        [HttpPost("Validate")]
        public IActionResult ValidateToken([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required");

            string secretKey = "My_Super_Secret_Key_1234567899999";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // Validation parameters
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "Manasa",
                    ValidAudience = "MyAppUsers",
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero // Removes default 5-min expiry buffer
                };

                // Validate token
                var principal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);

                // Get claims
                var username = principal.Identity?.Name;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;

                return Ok(new
                {
                    Message = "Token is valid",
                    Username = username,
                    Role = role,
                    Expiration = validatedToken.ValidTo
                });
            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized("Token has expired");
            }
            catch (Exception ex)
            {
                return Unauthorized($"Invalid token: {ex.Message}");
            }
        }

    }
}
