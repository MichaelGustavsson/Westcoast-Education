using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Data;
using westcoast_education.api.Models;
using westcoast_education.api.ViewModels;

namespace westcoast_education.api.Controllers
{
    [ApiController]
    [Route("api/v1/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly EducationContext _context;
        public CoursesController(EducationContext context)
        {
            _context = context;
        }

        // Lista alla kurser...
        [HttpGet()]
        public async Task<ActionResult> ListCourses()
        {
            var result = await _context.Courses
            .Include(i => i.Teacher)
            .Select(c => new CoursesViewModel
            {
                CourseId = c.CourseId,
                Title = c.Title,
                CourseNumber = c.CourseNumber,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Teacher = new TeacherViewModel
                {
                    Id = c.Teacher.Id,
                    BirthOfDate = c.Teacher.BirthOfDate.ToShortDateString(),
                    FirstName = c.Teacher.FirstName,
                    LastName = c.Teacher.LastName,
                    Email = c.Teacher.Email,
                    Phone = c.Teacher.Phone,
                    Address = c.Teacher.Address,
                    PostalCode = c.Teacher.PostalCode,
                    City = c.Teacher.City,
                    Country = c.Teacher.Country
                },
                Students = c.Students.Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    BirthOfDate = s.BirthOfDate.ToShortDateString(),
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    PostalCode = s.PostalCode,
                    City = s.City,
                    Country = s.Country
                }).ToList()
            })
            .ToListAsync();
            return Ok(result);
        }

        // Lägg till en ny kurs...
        [HttpPost()]
        public async Task<ActionResult> AddCourse(AddCourseViewModel model)
        {
            var course = new CourseModel
            {
                CourseId = Guid.NewGuid(),
                Title = model.Title,
                CourseNumber = model.CourseNumber,
                Duration = model.Duration,
                StartDate = model.StartDate,
                EndDate = model.StartDate.AddDays(model.Duration * 7)
            };

            await _context.Courses.AddAsync(course);

            if (await _context.SaveChangesAsync() > 0)
            {
                return StatusCode(201);
            }

            return StatusCode(500, "Internal Server Error");
        }

        // Lägg till lärare till kurs...
        [HttpPatch()]
        public async Task<ActionResult> AddTeacherToCourse(Guid courseId, Guid teacherId)
        {
            // Hämta kursen...
            var course = await _context.Courses.FindAsync(courseId);

            if (course is null) return BadRequest("Kunde inte hitta kursen i systemet.");

            // Hämta läraren...
            var teacher = await _context.Teachers.FindAsync(teacherId);

            if (teacher is null) return BadRequest($"Kunde inte hitta läraren i systemet");

            course.Teacher = teacher;

            _context.Courses.Update(course);

            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return StatusCode(500, "Internal Server Error");
        }
    }
}