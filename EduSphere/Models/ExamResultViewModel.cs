using System;

namespace EduSphere.Models
{
    public class ExamResultViewModel
    {
        public string CourseName { get; set; }
        public string ExamType { get; set; }
        public DateTime ExamDate { get; set; }
        public double Score { get; set; }
        public string LetterGrade { get; set; }
        public bool IsPassed { get; set; }
        public string PassStatus => IsPassed ? "Geçti" : "Kaldı";
    }
}