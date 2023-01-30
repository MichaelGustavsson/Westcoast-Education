using System.ComponentModel.DataAnnotations;

namespace westcoast_education.api.Models
{
    public class StudentModel : PersonModel
    {
        //Navigation
        public Guid CourseId { get; set; }

        public CourseModel Course { get; set; }
    }
}