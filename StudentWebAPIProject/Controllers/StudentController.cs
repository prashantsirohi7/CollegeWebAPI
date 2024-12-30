using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StudentWebAPIProject.Models;
using System.Net;
using IMyLogger = StudentWebAPIProject.Logging.ILogger;

namespace StudentWebAPIProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IMyLogger _myLogger;
        //inbuild logger
        private readonly ILogger<StudentController> _inbuildLogger;

        public StudentController(IMyLogger myLogger, ILogger<StudentController> inbuildLogger)
        {
            _myLogger = myLogger;
            _inbuildLogger = inbuildLogger;
        }

        [HttpGet("{id:int}", Name = "StudentDetailsById")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> StudentDetails(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid student id - {id}");

            var student = CollegeRepository.Students.Where(x => x.id == id).FirstOrDefault();
            if (student is null)
                return NotFound($"No student found with {id}");

            var studentDto = new StudentDTO
            {
                id = student.id,
                name = student.name,
                email = student.email,
                address = student.address,
                age = student.age
            };
            _myLogger.Log("Fetched Student Details");
            return Ok(studentDto);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<StudentDTO>> Students()
        {
            var students = CollegeRepository.Students.Select(x => new StudentDTO
            {
                id = x.id,
                name = x.name,
                email = x.email,
                address = x.address,
                age = x.age
            });
            return Ok(students);
        }

        [HttpGet]
        [Route("{name:alpha}", Name = "StudentDetailsByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Student> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                _inbuildLogger.LogInformation("Student name is empty");
                return BadRequest("Student name is empty");
            }

            var student = CollegeRepository.Students.Where(x => x.name.ToLower() == name.ToLower()).FirstOrDefault();
            if (student is null)
            {
                _inbuildLogger.LogError($"No student found with {name}");
                return NotFound($"No student found with {name}");
            }

            var studentDto = new StudentDTO
            {
                id = student.id,
                name = student.name,
                email = student.email,
                address = student.address,
                age = student.age
            };

            _inbuildLogger.LogInformation("Student found");
            return Ok(studentDto);
        }

        [HttpDelete("{id:alpha}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> Delete(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid student id - {id}");

            var student = CollegeRepository.Students.Where(x => x.id == id).FirstOrDefault();
            if (student is null)
                return NotFound($"No student found with {id}");

            return Ok(CollegeRepository.Students.Remove(student));
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]     //for documentation
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> CreateStudent([FromBody]StudentDTO model)
        {
            // if ApiController attribue is not used
            //if (!ModelState.IsValid)
            //    return BadRequest();

            if(model is  null)
                return BadRequest();

            // Validation in code
            //if (model.addmissionDate < DateTime.Today)
            //    return BadRequest("Admission date is less than today date");

            var id = CollegeRepository.Students.LastOrDefault().id + 1;
            CollegeRepository.Students.Add(new Student
            {
                id = id,
                name = model.name,
                address = model.address,
                age = model.age,
                email = model.email
            });

            model.id = id;
            return CreatedAtRoute("StudentDetailsById", new { id = model.id }, model);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]     //for documentation
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateStudentDetails([FromBody]StudentDTO model)
        {
            if(model is null || model.id <= 0)
                return BadRequest();

            var student = CollegeRepository.Students.Where(x => x.id == model.id).FirstOrDefault();
            if (student is null)
                return NotFound();

            student.name = model.name;
            student.address = model.address;
            student.email = model.email;

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]     //for documentation
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateStudentPartialDetails(int id, [FromBody] JsonPatchDocument<StudentDTO> model)
        {
            if (model is null || id <= 0)
                return BadRequest();

            var student = CollegeRepository.Students.Where(x => x.id == id).FirstOrDefault();
            if (student is null)
                return NotFound();

            var studentDto = new StudentDTO
            {
                id = student.id,
                name = student.name,
                address = student.address,
                email = student.email,
                age = student.age
            };

            model.ApplyTo(studentDto, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            student.name = studentDto.name;
            student.address = studentDto.address;
            student.email = studentDto.email;
            student.age = studentDto.age;

            return NoContent();
        }
    }
}
