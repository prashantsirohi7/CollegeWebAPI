using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentWebAPIProject.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentWebAPIProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public ActionResult<LoginResponceDTO> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please enter username and password");

            var responce = new LoginResponceDTO
            {
                username = "pras"
            };

            if(string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                return Unauthorized("Invalid username or password");

            byte[] key = null;
            string issuer = "";
            string audience = "";
            if (string.IsNullOrEmpty(model.Policy))
                model.Policy = "";
            switch (model.Policy.ToLower())
            {
                case "local":
                    key = Encoding.ASCII.GetBytes(_configuration["JwtKeyForLocalUsers"]);
                    issuer = _configuration["LocalIssuer"];
                    audience = _configuration["LocalAudience"];
                    break;
                default:
                    key = Encoding.ASCII.GetBytes(_configuration["JwtKey"]);
                    break;
            }

            if (model.UserName == "pras" && model.Password == "123")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "Admin")
                    }),
                    Expires = DateTime.Now.AddMinutes(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                if(!string.IsNullOrEmpty(model.Policy))
                {
                    tokenDescriptor.Issuer = issuer;
                    tokenDescriptor.Audience = audience;
                }

                var token = tokenHandler.CreateToken(tokenDescriptor);
                responce.token = tokenHandler.WriteToken(token);
                return Ok(responce);
            }
            else
                return Ok("Invalid username or password");
        }
    }
}
