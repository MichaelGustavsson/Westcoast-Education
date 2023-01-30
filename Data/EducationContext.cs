using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Models;

namespace westcoast_education.api.Data
{
    public class EducationContext : DbContext
    {
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<StudentModel> Students { get; set; }
        public DbSet<TeacherModel> Teachers { get; set; }

        public EducationContext(DbContextOptions options) : base(options) { }
    }
}