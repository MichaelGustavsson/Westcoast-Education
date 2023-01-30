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
            .Include(s => s.Students)
            .ToListAsync();

            var courses = new List<CoursesViewModel>();

            foreach (var item in result)
            {
                courses.Add(MapCourseToViewModel(item));
            }

            return Ok(courses);
        }

        [HttpGet("{courseId}", Name = "GetCourse")]
        public async Task<ActionResult> GetCourse(Guid courseId)
        {
            var result = await _context.Courses.Include(c => c.Teacher).Include(s => s.Students).SingleOrDefaultAsync(c => c.CourseId == courseId);

            if (result is null) return BadRequest("Kunde inte hitta kursen");
            else
            {
                CoursesViewModel course = MapCourseToViewModel(result);

                return Ok(course);
            }
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
                return CreatedAtAction("GetCourse", new { courseId = course.CourseId }, MapCourseToViewModel(course));
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

        private CoursesViewModel MapCourseToViewModel(CourseModel result)
        {
            var course = new CoursesViewModel
            {
                CourseId = result.CourseId,
                Title = result.Title,
                CourseNumber = result.CourseNumber,
                StartDate = result.StartDate,
                EndDate = result.EndDate
            };

            if (result.Teacher is not null)
            {
                course.Teacher = new TeacherViewModel
                {
                    Id = result.Teacher.Id,
                    BirthOfDate = result.Teacher.BirthOfDate.ToShortDateString(),
                    FirstName = result.Teacher.FirstName,
                    LastName = result.Teacher.LastName,
                    Email = result.Teacher.Email,
                    Phone = result.Teacher.Phone,
                    Address = result.Teacher.Address,
                    PostalCode = result.Teacher.PostalCode,
                    City = result.Teacher.City,
                    Country = result.Teacher.Country
                };
            }

            if (result.Students is not null)
            {
                course.Students = result.Students.Select(s => new StudentViewModel
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
                }).ToList();
            }

            return course;
        }
    }
}