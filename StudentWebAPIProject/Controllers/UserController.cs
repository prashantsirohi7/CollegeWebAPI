using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentWebAPIProject.DBSets.Repository;
using StudentWebAPIProject.Models;
using StudentWebAPIProject.Services;

namespace StudentWebAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private ApiResponce _apiResponce;
        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
            _apiResponce = new();
        }

        [HttpPost("Create", Name = "CreateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponce>> CreateUserAsync([FromBody] UserDTO dto)
        {
            try
            {
                var result = await _userService.CreateUser(dto);

                _apiResponce.Data = result;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                return CreatedAtRoute("GetUserById", new { id = result.Id}, _apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }

        [HttpGet("{id:int}", Name = "GetUserById")]
        public async Task<ActionResult<ApiResponce>> GetUserByIdAsync(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid user id - {id}");
            try
            {
                var result = await _userService.GetUserById(id);
                if (result is null)
                    return NotFound($"No role found with {id}");

                _apiResponce.Data = result;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }

        [HttpGet("All", Name = "GetAllUsers")]
        public async Task<ActionResult<ApiResponce>> GetAllUsersAsync()
        {
            try
            {
                var result = await _userService.GetAllUsers();
                _apiResponce.Data = result;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }

        [HttpGet("{name:alpha}", Name = "GetUserByName")]
        public async Task<ActionResult<ApiResponce>> GetUserByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest($"Not a valid name - {name}");
            try
            {
                var result = await _userService.GetUserByName(name);
                if (result is null)
                    return NotFound($"No user found with {name}");

                _apiResponce.Data = result;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }

        [HttpGet("GetAllUsersByUserTypeId/{userTypeId:int}", Name = "GetAllUsersByUserTypeId")]
        public async Task<ActionResult<ApiResponce>> GetAllRolePrivilegeByRoleIdAsync(int userTypeId)
        {
            if (userTypeId <= 0)
                return BadRequest($"Not a valid userType id - {userTypeId}");
            try
            {
                var result = await _userService.GetUserByUserTypeId(userTypeId);
                if (result is null)
                    return NotFound($"No user found with {userTypeId}");

                _apiResponce.Data = result;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }

        [HttpPut("Update", Name = "UpdateUser")]
        public async Task<ActionResult<ApiResponce>> UpdateUserAsync([FromBody] UserReadOnlyDTO dto)
        {
            if (dto is null || dto.Id <= 0)
                return BadRequest();
            try
            {
                var result = await _userService.UpdateUser(dto);
                if (result is null)
                    return NotFound($"No user found with {dto.Id}");

                _apiResponce.Data = result;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }

        [HttpDelete("Delete/{id:int}", Name = "DeleteUserById")]
        public async Task<ActionResult<ApiResponce>> DeleteUserAsync(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid role Privilege id - {id}");
            try
            {
                var result = await _userService.DeleteUserById(id);

                _apiResponce.Data = true;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }

        [HttpPut("UpdatePassword", Name = "UpdatePassword")]
        public async Task<ActionResult<ApiResponce>> UpdatePasswordUserAsync([FromQuery] int id, [FromBody] string password)
        {
            if (password is null || id <= 0)
                return BadRequest();
            try
            {
                var result = await _userService.UpdatePassword(id, password);

                _apiResponce.Data = true;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }
    }
}
