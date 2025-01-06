using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StudentWebAPIProject.DBSets;
using StudentWebAPIProject.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using IMyLogger = StudentWebAPIProject.Logging.ILogger;
using AutoMapper;
using StudentWebAPIProject.DBSets.Repository;
using Microsoft.AspNetCore.Authorization;

namespace StudentWebAPIProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StudentAutoMapperController : ControllerBase
    {
        private readonly IMyLogger _myLogger;
        //inbuild logger
        private readonly ILogger<StudentController> _inbuildLogger;
        //AutoMapper
        private readonly IMapper _mapper;
        //private readonly ICollegeRepository<Student> _studentRepository;
        private readonly IStudentRepository _studentRepository;
        private ApiResponce _apiResponce;

        public StudentAutoMapperController(IMyLogger myLogger, ILogger<StudentController> inbuildLogger, 
            IStudentRepository studentRepository, IMapper mapper)
        {
            _myLogger = myLogger;
            _inbuildLogger = inbuildLogger;
            _studentRepository = studentRepository;
            _mapper = mapper;
            _apiResponce = new ApiResponce();
        }

        [HttpGet("{id:int}", Name = "StudentDetailsByIdAutoMapper")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponce>> StudentDetailsAsync(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid student id - {id}");

            try
            {
                var student = await _studentRepository.GetByFilterAsync(student => student.Id == id);
                if (student is null)
                    return NotFound($"No student found with {id}");

                var studentDto = _mapper.Map<StudentDTO>(student);

                _apiResponce.Data = studentDto;
                _apiResponce.Status = true;
                _apiResponce.StatusCode = HttpStatusCode.OK;

                _myLogger.Log("Fetched Student Details");
                return Ok(_apiResponce);
            }
            catch (Exception ex)
            {
                _apiResponce.Status = false;
                _apiResponce.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponce.Errors.Add(ex.Message);
                return _apiResponce;
            }
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> StudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            var studentDto = _mapper.Map<IEnumerable<StudentDTO>>(students);

            return Ok(studentDto);
        }

        [HttpGet]
        [Route("{name:alpha}", Name = "StudentDetailsByNameAutoMapper")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> GetStudentByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                _inbuildLogger.LogInformation("Student name is empty");
                return BadRequest("Student name is empty");
            }

            var student = await _studentRepository.GetByFilterAsync(student => student.Name.ToLower() == name.ToLower());
            if (student is null)
            {
                _inbuildLogger.LogError($"No student found with {name}");
                return NotFound($"No student found with {name}");
            }

            var studentDto = _mapper.Map<StudentDTO>(student);

            _inbuildLogger.LogInformation("Student found");
            return Ok(studentDto);
        }

        [HttpDelete("Delete/{id:int}", Name = "DeleteStudentAutoMapper")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid student id - {id}");

            var student = await _studentRepository.GetByFilterAsync(student => student.Id == id);
            if (student is null)
                return NotFound($"No student found with {id}");

            await _studentRepository.DeleteAsync(student);

            return Ok();
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]     //for documentation
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> CreateStudentAsync([FromBody]StudentDTO model)
        {
            // if ApiController attribue is not used
            //if (!ModelState.IsValid)
            //    return BadRequest();

            if(model is  null)
                return BadRequest();

            // Validation in code
            //if (model.addmissionDate < DateTime.Today)
            //    return BadRequest("Admission date is less than today date");

            var student = _mapper.Map<Student>(model); 
            var newStudent = await _studentRepository.CreateAsync(student);

            model.id = newStudent.Id;
            return CreatedAtRoute("StudentDetailsByIdAutoMapper", new { id = model.id }, model);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]     //for documentation
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateStudentDetailsAsync([FromBody]StudentDTO model)
        {
            if(model is null || model.id <= 0)
                return BadRequest();

            // Add asnotracking to avoid tracking for update to DB
            var student = await _studentRepository.GetByFilterAsync(student => student.Id == model.id, true);
            if (student is null)
                return NotFound();

            var studentEntity = _mapper.Map<Student>(model); 

            await _studentRepository.UpdateAsync(studentEntity);
            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]     //for documentation
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateStudentPartialDetailsAsync(int id, [FromBody] JsonPatchDocument<StudentDTO> model)
        {
            if (model is null || id <= 0)
                return BadRequest();

            var student = await _studentRepository.GetByFilterAsync(student => student.Id == id, true);
            if (student is null)
                return NotFound();

            var studentDto = _mapper.Map<StudentDTO>(student);

            model.ApplyTo(studentDto, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            student = _mapper.Map<Student>(studentDto);
            await _studentRepository.UpdateAsync(student);

            return NoContent();
        }
    }
}
