using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using System.Linq;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace EduSphere.Controllers
{
    public class UserSettingsController : Controller
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Tweet> _tweetsCollection;

        public UserSettingsController(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("EduSphere");
            _usersCollection = database.GetCollection<User>("Users");
            _tweetsCollection = database.GetCollection<Tweet>("Tweets");
        }

        // PROFİL: GÖRÜNTÜLE
        public IActionResult MyProfile()
        {
            var username = GetLoggedUsername();
            var user = _usersCollection.Find(u => u.Username == username).FirstOrDefault();
            return View(user);
        }

        // PROFİL: BİLGİ GÜNCELLE
        [HttpPost]
        public IActionResult UpdateProfile(string name, string lastname, string email, string phonenumber, string department, string adress)
        {
            var username = GetLoggedUsername();

            var update = Builders<User>.Update
                .Set(u => u.Name, name)
                .Set(u => u.Lastname, lastname)
                .Set(u => u.Email, email)
                .Set(u => u.PhoneNumber, phonenumber)
                .Set(u => u.Department, department)
                .Set(u => u.Adress, adress);

            _usersCollection.UpdateOne(u => u.Username == username, update);

            return RedirectToAction("MyProfile");
        }

        // PROFİL: FOTOĞRAF YÜKLE
        [HttpPost]
        public IActionResult UpdateProfilePhoto(IFormFile profilePhoto)
        {
            var username = GetLoggedUsername();

            if (profilePhoto != null && profilePhoto.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    profilePhoto.CopyTo(ms);
                    var bytes = ms.ToArray();
                    var type = profilePhoto.ContentType;
                    var update = Builders<User>.Update
                        .Set(u => u.ProfilePhoto, bytes)
                        .Set(u => u.ProfilePhotoType, type);

                    _usersCollection.UpdateOne(u => u.Username == username, update);
                }
            }
            return RedirectToAction("MyProfile");
        }

        // PROFİL: FOTOĞRAF SİL
        [HttpPost]
        public IActionResult RemoveProfilePhoto()
        {
            var username = GetLoggedUsername();

            var update = Builders<User>.Update
                .Unset(u => u.ProfilePhoto)
                .Unset(u => u.ProfilePhotoType);

            _usersCollection.UpdateOne(u => u.Username == username, update);

            return RedirectToAction("MyProfile");
        }

        // PROFİL: HESAP SİL
        [HttpPost]
        public IActionResult DeleteAccount()
        {
            var username = GetLoggedUsername();

            _usersCollection.DeleteOne(u => u.Username == username);

            // Girişten çıkış işlemi burada yapılabilir
            return RedirectToAction("Index", "Home");
        }

        // TWEETLERİ GÖRÜNTÜLE
        public IActionResult MyTweet()
        {
            var username = GetLoggedUsername();
            var tweets = _tweetsCollection.Find(t => t.Username == username)
                                          .SortByDescending(t => t.CreatedAt)
                                          .ToList();
            return View(tweets);
        }

        // TWEET EKLE
        [HttpPost]
        public IActionResult AddTweet(string content)
        {
            var username = GetLoggedUsername();
            var user = _usersCollection.Find(u => u.Username == username).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(content) && user != null)
            {
                var tweet = new Tweet
                {
                    Username = username,
                    Name = user.Name,
                    Text = content,
                    CreatedAt = DateTime.UtcNow
                };
                _tweetsCollection.InsertOne(tweet);
            }
            return RedirectToAction("MyTweet");
        }

        // TWEET SİL
        [HttpPost]
        public IActionResult DeleteTweet(string id)
        {
            var username = GetLoggedUsername();
            _tweetsCollection.DeleteOne(t => t.Id == id && t.Username == username);
            return RedirectToAction("MyTweet");
        }

        // TWEET GÜNCELLE
        [HttpPost]
        public IActionResult UpdateTweet(string id, string updatedText)
        {
            var username = GetLoggedUsername();
            var update = Builders<Tweet>.Update
                .Set(t => t.Text, updatedText)
                .Set(t => t.CreatedAt, DateTime.UtcNow);
            _tweetsCollection.UpdateOne(t => t.Id == id && t.Username == username, update);
            return RedirectToAction("MyTweet");
        }

        // ORTAK: GİRİŞ YAPAN KULLANICI ADI AL
        private string GetLoggedUsername()
        {
            return User?.Identity?.IsAuthenticated == true
                ? User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value ?? ""
                : "";
        }
    }
}