using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduSphere.Models
{
    [BsonIgnoreExtraElements] // Bunu eklemek hata riskini azaltır
    public class ChatUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("lastname")]
        public string Lastname { get; set; }
    }
}