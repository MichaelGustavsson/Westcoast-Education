namespace westcoast_education.api.Models
{
    public class TeacherModel : PersonModel
    {
        public IList<CourseModel> Courses { get; set; }
    }
}