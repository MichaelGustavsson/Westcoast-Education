using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Data;
using westcoast_education.api.Models;
using westcoast_education.api.ViewModels;

namespace westcoast_education.api.Controllers
{
    [ApiController]
    [Route("api/v1/teachers")]
    public class TeachersController : ControllerBase
    {
        private readonly EducationContext _context;
        public TeachersController(EducationContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<ActionResult> ListTeachers()
        {
            var result = await _context.Teachers
            .Select(c => new TeacherViewModel
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

        [HttpGet("{id}", Name = "GetTeacher")]
        public async Task<ActionResult> GetTeacher(Guid id)
        {
            var result = await _context.Teachers.FindAsync(id);
            TeacherViewModel teacher = MapTeacherToViewModel(result);

            return Ok(teacher);
        }

        [HttpPost()]
        public async Task<ActionResult> AddTeacher(AddPersonViewModel model)
        {
            // Glöm inte att göra en kontroll så läraren inte finns i systemet redan
            var teacher = new TeacherModel
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
                Country = model.Country
            };

            _context.Teachers.Add(teacher);

            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetTeacher", new { id = teacher.Id }, MapTeacherToViewModel(teacher));
            }

            return StatusCode(500, "Internal Server Error");
        }

        private TeacherViewModel MapTeacherToViewModel(TeacherModel result)
        {
            return new TeacherViewModel
            {
                Id = result.Id,
                BirthOfDate = result.BirthOfDate.ToShortDateString(),
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                Phone = result.Phone,
                Address = result.Address,
                PostalCode = result.PostalCode,
                City = result.City,
                Country = result.Country
            };
        }
    }
}