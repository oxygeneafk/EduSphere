using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace EduSphere.Models
{
    public class Schedule
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Department { get; set; }
        public List<DaySchedule> Days { get; set; }
    }

    public class DaySchedule
    {
        public string Day { get; set; }
        public List<string> Courses { get; set; }
    }
}   