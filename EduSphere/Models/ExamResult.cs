using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace EduSphere.Models
{
    public class ExamResult
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string StudentId { get; set; } // User Id'si
        public string StudentUsername { get; set; }
        public string StudentName { get; set; }
        public string StudentDepartment { get; set; }

        public string ExamId { get; set; } // Exam collection'daki sınav Id'si
        public string CourseName { get; set; }
        public string ExamType { get; set; } // Vize, Final, Bütünleme
        public DateTime ExamDate { get; set; }

        public double Score { get; set; } // 0-100 arası puan
        public string LetterGrade { get; set; } // AA, BA, BB, CB, CC, DC, DD, FD, FF
        public bool IsPassed { get; set; } // Geçti/Kaldı

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "Admin";
    }
}