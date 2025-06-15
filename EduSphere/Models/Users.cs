using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EduSphere.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("lastname")]
        public string Lastname { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("phonenumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("adress")]
        public string Adress { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }
    }
}