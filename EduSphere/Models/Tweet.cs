using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace EduSphere.Models
{
    public class Tweet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("text")]
        public string Text { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("mediaData")]
        public byte[] MediaData { get; set; }  // 🆕 Fotoğraf/video içeriği

        [BsonElement("mediaType")]
        public string MediaType { get; set; }  // 🆕 "image/jpeg", "video/mp4" gibi MIME türü
    }
}