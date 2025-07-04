﻿using MongoDB.Bson;
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

        [BsonElement("department")]
        public string Department { get; set; }

        [BsonElement("adress")]
        public string Adress { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        // YENİ EKLENEN ALANLAR
        [BsonElement("role")]
        public string Role { get; set; } = "Student"; // Default Student

        [BsonElement("studentNumber")]
        public string StudentNumber { get; set; }

        // ----- PROFİL FOTOĞRAFI DESTEĞİ -----
        [BsonElement("profilePhoto")]
        public byte[] ProfilePhoto { get; set; }

        [BsonElement("profilePhotoType")]
        public string ProfilePhotoType { get; set; }
    }
}