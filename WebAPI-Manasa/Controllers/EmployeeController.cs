using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_Manasa.Models;

namespace WebAPI_Manasa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [HttpPost("Generate")]
        public IActionResult GenerateToken([FromBody] TokenRequest request)
        {
            // Static user data
            //string username = "JohnDoe";
            //string role = "Role";
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Role))
            {
                return BadRequest("Username and Role are required.");
            }



            // Secret key (must be long & secure in production)
            string secretKey = "My_Super_Secret_Key_1234567899999";

            // Create claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, request.Role)
            };

            // Create key and credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create token
            var token = new JwtSecurityToken(
                issuer: "Manasa",
                audience: "MyAppUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            // Return token
            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            });
        }
    }
}
