using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace EduSphere.Models
{
    public class Schedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Department { get; set; }
        public List<DaySchedule> Days { get; set; } = new List<DaySchedule>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "Admin";
    }

    public class DaySchedule
    {
        public string Day { get; set; }
        public List<CourseSchedule> Courses { get; set; } = new List<CourseSchedule>();
    }

    public class CourseSchedule
    {
        public string CourseName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Instructor { get; set; }
        public string Classroom { get; set; }
    }
}