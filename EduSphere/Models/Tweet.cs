using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduSphere.Models
{
    [BsonIgnoreExtraElements]
    public class Tweet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Text { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
    }
}