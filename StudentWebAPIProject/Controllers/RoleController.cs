using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentWebAPIProject.DBSets.Repository;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class RoleController : ControllerBase
    {
        private readonly ICollegeRepository<Role> _repository;
        private readonly IMapper _mapper;
        private ApiResponce _apiResponce;
        public RoleController(ICollegeRepository<Role> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _apiResponce = new();
        }

        [HttpPost("Create", Name = "CreateRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponce>> CreateRoleAsync([FromBody] RoleDTO dto)
        {
            if (dto is null)
                return BadRequest();

            try
            {
                var role = _mapper.Map<Role>(dto);
                role.IsDeleted = false;
                role.CreatedDate = DateTime.Now;
                role.ModifiedDate = DateTime.Now;

                var result = await _repository.CreateAsync(role);
                dto.Id = result.Id;

                _apiResponce.Data = dto;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                //return Ok(_apiResponce);
                return CreatedAtRoute("GetRoleById", new { id = dto.Id}, _apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }

        [HttpGet("{id:int}", Name = "GetRoleById")]
        public async Task<ActionResult<ApiResponce>> GetRoleByIdAsync(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid role id - {id}");
            try
            {
                var result = await _repository.GetByFilterAsync(role => role.Id == id);
                if (result is null)
                    return NotFound($"No role found with {id}");

                _apiResponce.Data = _mapper.Map<RoleDTO>(result);
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

        [HttpGet("All", Name = "GetAllRoles")]
        public async Task<ActionResult<ApiResponce>> GetAllRolesAsync()
        {
            try
            {
                var result = await _repository.GetAllAsync();
                _apiResponce.Data = _mapper.Map<List<RoleDTO>>(result);
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

        [HttpGet("{name:alpha}", Name = "GetRoleByName")]
        public async Task<ActionResult<ApiResponce>> GetRoleByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest($"Not a valid name - {name}");
            try
            {
                var result = await _repository.GetByFilterAsync(role => role.RoleName.ToLower() == name.ToLower());
                if (result is null)
                    return NotFound($"No role found with {name}");

                _apiResponce.Data = _mapper.Map<RoleDTO>(result);
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

        [HttpPut("Update", Name = "UpdateRole")]
        public async Task<ActionResult<ApiResponce>> UpdateRoleAsync([FromBody] RoleDTO dto)
        {
            if (dto is null || dto.Id <= 0)
                return BadRequest();
            try
            {
                var existingRole = await _repository.GetByFilterAsync(role => role.Id == dto.Id, true);
                if (existingRole is null)
                    return NotFound($"No role found with {dto.Id}");

                var newRole = _mapper.Map<Role>(dto);

                var result = await _repository.UpdateAsync(newRole);
                _apiResponce.Data = dto;
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

        [HttpDelete("Delete/{id:int}", Name = "DeleteRoleById")]
        public async Task<ActionResult<ApiResponce>> DeleteRoleAsync(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid role id - {id}");
            try
            {
                var existingRole = await _repository.GetByFilterAsync(role => role.Id == id);
                if (existingRole is null)
                    return NotFound($"No role found with {id}");

                var result = await _repository.DeleteAsync(existingRole);
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
