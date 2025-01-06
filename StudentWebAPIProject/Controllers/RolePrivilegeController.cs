using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentWebAPIProject.DBSets.Repository;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePrivilegeController : ControllerBase
    {
        private readonly ICollegeRepository<RolePrivilege> _repository;
        private readonly IMapper _mapper;
        private ApiResponce _apiResponce;
        public RolePrivilegeController(ICollegeRepository<RolePrivilege> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _apiResponce = new();
        }

        [HttpPost("Create", Name = "CreateRolePrivilege")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponce>> CreateRolePrivilegeAsync([FromBody] RolePrivilegeDTO dto)
        {
            if (dto is null)
                return BadRequest();

            try
            {
                var rolePrivilege = _mapper.Map<RolePrivilege>(dto);
                rolePrivilege.IsDeleted = false;
                rolePrivilege.CreatedDate = DateTime.Now;
                rolePrivilege.ModifiedDate = DateTime.Now;

                var result = await _repository.CreateAsync(rolePrivilege);
                dto.Id = result.Id;

                _apiResponce.Data = dto;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.OK;
                //return Ok(_apiResponce);
                return CreatedAtRoute("GetRolePrivilegeById", new { id = dto.Id}, _apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Errors.Add(ex.Message);
                _apiResponce.Status = false;
                _apiResponce.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                return _apiResponce;
            }
        }

        [HttpGet("{id:int}", Name = "GetRolePrivilegeById")]
        //[Route("{id:int}", Name = "GetRolePrivilegeById")]
        public async Task<ActionResult<ApiResponce>> GetRolePrivilegeByIdAsync(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid role Privilege id - {id}");
            try
            {
                var result = await _repository.GetByFilterAsync(rolePrivilege => rolePrivilege.Id == id);
                if (result is null)
                    return NotFound($"No role Privilege found with {id}");

                _apiResponce.Data = _mapper.Map<RolePrivilegeDTO>(result);
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

        [HttpGet("All", Name = "GetAllRolePrivileges")]
        public async Task<ActionResult<ApiResponce>> GetAllRolePrivilegesAsync()
        {
            try
            {
                var result = await _repository.GetAllAsync();
                _apiResponce.Data = _mapper.Map<List<RolePrivilegeDTO>>(result);
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

        [HttpGet("GetAllRolePrivilegeByRoleId/{roleId:int}", Name = "GetAllRolePrivilegeByRoleId")]
        public async Task<ActionResult<ApiResponce>> GetAllRolePrivilegeByRoleIdAsync(int roleId)
        {
            if (roleId <= 0)
                return BadRequest($"Not a valid role id - {roleId}");
            try
            {
                var result = await _repository.GetAllByFilterAsync(rolePrivilege => rolePrivilege.RoleId == roleId);
                if (result is null)
                    return NotFound($"No role Privilege found with {roleId}");

                _apiResponce.Data = _mapper.Map<List<RolePrivilegeDTO>>(result);
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

        [HttpGet("{name:alpha}", Name = "GetRolePrivilegeByName")]
        public async Task<ActionResult<ApiResponce>> GetRolePrivilegeByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest($"Not a valid name - {name}");
            try
            {
                var result = await _repository.GetByFilterAsync(rolePrivilege => rolePrivilege.RolePriviliegeName.ToLower() == name.ToLower());
                if (result is null)
                    return NotFound($"No role found with {name}");

                _apiResponce.Data = _mapper.Map<RolePrivilegeDTO>(result);
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

        [HttpPut("Update", Name = "UpdateRolePrivilege")]
        public async Task<ActionResult<ApiResponce>> UpdateRolePrivilegeAsync([FromBody] RolePrivilegeDTO dto)
        {
            if (dto is null || dto.Id <= 0)
                return BadRequest();
            try
            {
                var existingRolePrivilege = await _repository.GetByFilterAsync(rolePrivilege => rolePrivilege.Id == dto.Id, true);
                if (existingRolePrivilege is null)
                    return NotFound($"No role found with {dto.Id}");

                var newRolePrivilege = _mapper.Map<RolePrivilege>(dto);

                var result = await _repository.UpdateAsync(newRolePrivilege);
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

        [HttpDelete("Delete/{id:int}", Name = "DeleteRolePrivilegeById")]
        public async Task<ActionResult<ApiResponce>> DeleteRolePrivilegeAsync(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid role Privilege id - {id}");
            try
            {
                var existingRolePrivilege = await _repository.GetByFilterAsync(role => role.Id == id);
                if (existingRolePrivilege is null)
                    return NotFound($"No role Privilege found with {id}");

                var result = await _repository.DeleteAsync(existingRolePrivilege);
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
