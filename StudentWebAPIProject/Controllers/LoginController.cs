using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentWebAPIProject.DBSets.Repository;
using StudentWebAPIProject.Models;
using StudentWebAPIProject.Services;
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
        private readonly ICollegeRepository<User> _userRepository;
        private readonly IUserService _userService;
        public LoginController(IConfiguration configuration, ICollegeRepository<User> userRepository, IUserService userService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _userService = userService;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //Hint: Pwd is same as username
        public async Task<ActionResult<LoginResponceDTO>> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please enter username and password");

            var responce = new LoginResponceDTO
            {
                //username = "pras"
                username = model.UserName
            };

            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
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

            var user = await _userRepository.GetByFilterAsync(x => x.Username == model.UserName);
            if (user is null)
                return NotFound("User not found");

            var verifyPassword = _userService.VerifyUserPassword(user.Id, model.Password).Result;


            //if (model.UserName == "pras" && model.Password == "123")
            if (verifyPassword)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.Role, "SuperAdmin")
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
