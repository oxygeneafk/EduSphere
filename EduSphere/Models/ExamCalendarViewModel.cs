namespace EduSphere.Models
{
    public class ExamCalendarViewModel
    {
        public string CourseName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string ExamType { get; set; }
    }
}
