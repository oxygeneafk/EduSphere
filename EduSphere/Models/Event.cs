using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace EduSphere.Models
{
    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("date")]
        public DateTime Date { get; set; }
        [BsonElement("location")]
        public string Location { get; set; }
        [BsonElement("latitude")]
        public double Latitude { get; set; }
        [BsonElement("longitude")]
        public double Longitude { get; set; }
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("author")]
        public string Author { get; set; }
    }
}