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
                return StatusCode(201, teacher);
            }

            return StatusCode(500, "Internal Server Error");
        }
    }
}