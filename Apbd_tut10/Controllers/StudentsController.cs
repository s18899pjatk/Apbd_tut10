using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apbd_tut10.Entities;
using Apbd_tut10.Models;
using Apbd_tut10.Models.Requests;
using Apbd_tut10.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apbd_tut10.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsServiceDb studentsServiceDb;

        public StudentsController(IStudentsServiceDb studentsServiceDb)
        {
            this.studentsServiceDb = studentsServiceDb;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(studentsServiceDb.GetStudents());
        }

        [HttpPost]
        public IActionResult InsertStudent(InsertStudentRequest request)
        {
            var response = studentsServiceDb.InsertStudent(request);
            if (response == null) return BadRequest("Such student has already exists in db");
            return Ok(response);
        }

        [HttpPost("update")]
        public IActionResult DeleteStudent(UpdateStudentRequest request)
        {
            var response = studentsServiceDb.UpdateStudent(request);
            if (response == null) return BadRequest("Such student doesnt exists");
            return Ok(response);
        }

        [HttpPost("delete/{id}")]
        public IActionResult DeleteStudent(string id)
        {
            var response = studentsServiceDb.DeleteStudent(id);
            if (response == null) return BadRequest("Such student doesnt exists");
            return Ok(response);
        }


        [HttpPost("enroll", Name = nameof(EnrollStudent))]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var response = studentsServiceDb.EnrollStudent(request);
            if (response == null) return BadRequest("Cannot create such a student");
            else return CreatedAtAction(nameof(EnrollStudent), response);
        }

        [HttpPost("promote", Name = nameof(PromoteStudents))]
        public IActionResult PromoteStudents(PromoteStudentRequest request)
        {
            var response = studentsServiceDb.PromoteStudent(request);
            return CreatedAtAction(nameof(PromoteStudents), response);
        }
    }
}