using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace EduSphere.Models
{
    public class EventParticipant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("eventId")]
        public string EventId { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("userEmail")]
        public string UserEmail { get; set; }

        [BsonElement("joinedAt")]
        public DateTime JoinedAt { get; set; }
    }
}