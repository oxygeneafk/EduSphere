using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

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

        public IActionResult Index()
        {
            var tweets = _tweetsCollection.Find(_ => true)
                .SortByDescending(t => t.CreatedAt)
                .ToList();

            return View(tweets ?? new List<Tweet>());
        }

        [HttpPost]
        public IActionResult Create(string text, IFormFile media)
        {
            if (string.IsNullOrWhiteSpace(text) && media == null)
            {
                TempData["Error"] = "Tweet boş olamaz!";
                return RedirectToAction("Index");
            }

            var tweet = new Tweet
            {
                Text = text,
                CreatedAt = DateTime.UtcNow,
                Username = User.Identity.IsAuthenticated ? User.Identity.Name : "Anonim",
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