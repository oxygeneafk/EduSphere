using System;

namespace EduSphere.Models
{
    public class ExamCalendarViewModel
    {
        public string CourseName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string ExamType { get; set; }
        public string Classroom { get; set; }
        public string Instructor { get; set; }
        public int Duration { get; set; }
    }
}