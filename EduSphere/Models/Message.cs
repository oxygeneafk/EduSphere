using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace EduSphere.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("fromUsername")]
        public string FromUsername { get; set; }

        [BsonElement("toUsername")]
        public string ToUsername { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("mediaUrl")]
        public string MediaUrl { get; set; } // <-- Medya yolu (foto/video için)

        [BsonElement("sentAt")]
        public DateTime SentAt { get; set; }
    }
}