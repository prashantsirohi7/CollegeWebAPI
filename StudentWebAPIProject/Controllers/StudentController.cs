﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StudentWebAPIProject.DBSets;
using StudentWebAPIProject.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
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
        //For inegrating with DB
        private readonly CollegeDBContext _collegeDBContext;

        public StudentController(IMyLogger myLogger, ILogger<StudentController> inbuildLogger, CollegeDBContext collegeDBContext)
        {
            _myLogger = myLogger;
            _inbuildLogger = inbuildLogger;
            _collegeDBContext = collegeDBContext;
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

            var student = _collegeDBContext.Students.Where(x => x.Id == id).FirstOrDefault();
            if (student is null)
                return NotFound($"No student found with {id}");

            var studentDto = new StudentDTO
            {
                id = student.Id,
                name = student.Name,
                email = student.Email,
                address = student.Address,
                DOB = student.DOB,
                addmissionDate = student.AddmissionDate
            };
            _myLogger.Log("Fetched Student Details");
            return Ok(studentDto);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<StudentDTO>> Students()
        {
            var students = _collegeDBContext.Students.Select(x => new StudentDTO
            {
                id = x.Id,
                name = x.Name,
                email = x.Email,
                address = x.Address,
                DOB = x.DOB,
                addmissionDate = x.AddmissionDate
            });
            return Ok(students);
        }

        [HttpGet]
        [Route("{name:alpha}", Name = "StudentDetailsByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                _inbuildLogger.LogInformation("Student name is empty");
                return BadRequest("Student name is empty");
            }

            var student = _collegeDBContext.Students.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (student is null)
            {
                _inbuildLogger.LogError($"No student found with {name}");
                return NotFound($"No student found with {name}");
            }

            var studentDto = new StudentDTO
            {
                id = student.Id,
                name = student.Name,
                email = student.Email,
                address = student.Address,
                DOB = student.DOB,
                addmissionDate = student.AddmissionDate
            };

            _inbuildLogger.LogInformation("Student found");
            return Ok(studentDto);
        }

        [HttpDelete("{id:int}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]     //for documentation
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> Delete(int id)
        {
            if (id <= 0)
                return BadRequest($"Not a valid student id - {id}");

            var student = _collegeDBContext.Students.Where(x => x.Id == id).FirstOrDefault();
            if (student is null)
                return NotFound($"No student found with {id}");

            _collegeDBContext.Students.Remove(student);
            _collegeDBContext.SaveChanges();

            return Ok();
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

            var student = new DBSets.Student
            {
                Name = model.name,
                Address = model.address,
                DOB = model.DOB,
                Email = model.email,
                AddmissionDate = model.addmissionDate
            };
            _collegeDBContext.Students.Add(student);

            _collegeDBContext.SaveChanges();

            model.id = student.Id;
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

            /*
            var student = _collegeDBContext.Students.Where(x => x.Id == model.id).FirstOrDefault();
            if (student is null)
                return NotFound();

            student.Name = model.name;
            student.Address = model.address;
            student.Email = model.email;
            student.DOB = model.DOB;
            student.AddmissionDate = model.addmissionDate;
            _collegeDBContext.SaveChanges();
            */

            // Add asnotracking to avoid tracking for update to DB
            var student = _collegeDBContext.Students.AsNoTracking().Where(x => x.Id == model.id).FirstOrDefault();
            if (student is null)
                return NotFound();

            var studentEntity = new DBSets.Student
            {
                Id = student.Id,
                Name = model.name,
                Address = model.address,
                DOB = model.DOB,
                Email = model.email,
                AddmissionDate = model.addmissionDate
            };

            _collegeDBContext.Students.Update(studentEntity);
            _collegeDBContext.SaveChanges();
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

            var student = _collegeDBContext.Students.Where(x => x.Id == id).FirstOrDefault();
            if (student is null)
                return NotFound();

            var studentDto = new StudentDTO
            {
                id = student.Id,
                name = student.Name,
                address = student.Address,
                email = student.Email,
                DOB = student.DOB,
                addmissionDate = student.AddmissionDate
            };

            model.ApplyTo(studentDto, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            student.Name = studentDto.name;
            student.Address = studentDto.address;
            student.Email = studentDto.email;
            student.DOB = studentDto.DOB;
            student.AddmissionDate = studentDto.addmissionDate;
            _collegeDBContext.SaveChanges();

            return NoContent();
        }
    }
}
