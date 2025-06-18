using System;

namespace EduSphere.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string FromUsername { get; set; }
        public string ToUsername { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}   