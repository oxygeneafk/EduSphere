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
            var database = client.GetDatabase("EduSphereDB");
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
        public IActionResult Create(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                TempData["Error"] = "Tweet boş olamaz!";
                return RedirectToAction("Index");
            }

            var username = User?.Identity?.IsAuthenticated == true
                ? User.Identity.Name
                : "Anonim";

            var tweet = new Tweet
            {
                Text = text,
                Username = username,
                CreatedAt = DateTime.UtcNow
            };

            _tweetsCollection.InsertOne(tweet);

            return RedirectToAction("Index");
        }
    }
}