using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Data;
using westcoast_education.api.Models;
using westcoast_education.api.ViewModels;

namespace westcoast_education.api.Controllers
{
    [ApiController]
    [Route("api/v1/students")]
    public class StudentsController : ControllerBase
    {
        private readonly EducationContext _context;
        public StudentsController(EducationContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<ActionResult> ListStudents()
        {
            var result = await _context.Students
            .Select(c => new StudentViewModel
            {
                Id = c.Id,
                BirthOfDate = c.BirthOfDate.ToShortDateString(),
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                PostalCode = c.PostalCode,
                City = c.City,
                Country = c.Country
            })
            .ToListAsync();
            return Ok(result);
        }

        [HttpPost()]
        public async Task<ActionResult> AddStudent([FromQuery] Guid courseId, AddPersonViewModel model)
        {
            // Kontrollera om kursen existerar...
            var course = await _context.Courses.FindAsync(courseId);

            if (course is null) return BadRequest("Vi kunde inte hitta kursen i systemet.");

            var student = new StudentModel
            {
                Id = Guid.NewGuid(),
                BirthOfDate = model.BirthOfDate,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                PostalCode = model.PostalCode,
                City = model.City,
                Country = model.Country,
                Course = course
            };

            await _context.Students.AddAsync(student);

            if (await _context.SaveChangesAsync() > 0)
            {
                return StatusCode(201);
            }

            return StatusCode(500, "Internal Server Error");
        }
    }
}