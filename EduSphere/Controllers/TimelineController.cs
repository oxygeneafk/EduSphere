using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EduSphere.Controllers
{
    public class TimelineController : Controller
    {
        private readonly IMongoCollection<Tweet> _tweetsCollection;

        public TimelineController(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("EduSphere");
            _tweetsCollection = database.GetCollection<Tweet>("Tweets");
        }

        public IActionResult Index(string search)
        {
            FilterDefinition<Tweet> filter = Builders<Tweet>.Filter.Empty;

            if (!string.IsNullOrEmpty(search))
            {
                if (search.StartsWith("@"))
                {
                    var username = search.Substring(1); // "@" işaretini çıkar
                    filter = Builders<Tweet>.Filter.Regex(t => t.Username, new MongoDB.Bson.BsonRegularExpression(username, "i"));
                }
                else
                {
                    filter = Builders<Tweet>.Filter.Or(
                        Builders<Tweet>.Filter.Regex(t => t.Name, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                        Builders<Tweet>.Filter.Regex(t => t.Text, new MongoDB.Bson.BsonRegularExpression(search, "i"))
                    );
                }
            }

            var tweets = _tweetsCollection.Find(filter)
                .SortByDescending(t => t.CreatedAt)
                .ToList();

            ViewBag.Search = search; // tekrar input'a geri doldurmak için
            return View(tweets);
        }


        [HttpPost]
        public IActionResult Create(string text, IFormFile media)
        {
            if (string.IsNullOrWhiteSpace(text) && media == null)
            {
                TempData["Error"] = "Tweet boş olamaz!";
                return RedirectToAction("Index");
            }

            var username = User?.Identity?.IsAuthenticated == true
                ? User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value ?? "Anonim"
                : "Anonim";

            var name = User?.Identity?.IsAuthenticated == true
                ? User.Claims.FirstOrDefault(c => c.Type == "Name")?.Value ?? "Anonim"
                : "Anonim";

            var tweet = new Tweet
            {
                Text = text,
                Name = name,
                Username = username,
                CreatedAt = DateTime.UtcNow
            };

            if (media != null && media.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    media.CopyTo(memoryStream);
                    tweet.MediaData = memoryStream.ToArray();
                    tweet.MediaType = media.ContentType;
                }
            }

            _tweetsCollection.InsertOne(tweet);

            return RedirectToAction("Index");
        }
    }
}
