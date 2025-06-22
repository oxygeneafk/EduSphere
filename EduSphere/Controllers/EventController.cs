using EduSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;

namespace EduSphere.Controllers
{
    public class EventController : Controller
    {
        private readonly IMongoCollection<Event> _eventsCollection;

        public EventController(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("EduSphere");
            _eventsCollection = database.GetCollection<Event>("Events");
        }

        // Public etkinlik listesi
        public IActionResult Index()
        {
            var list = _eventsCollection.Find(_ => true).SortByDescending(e => e.CreatedAt).ToList();
            return View(list);
        }

        // Etkinlik detay
        public IActionResult Details(string id)
        {
            var evt = _eventsCollection.Find(e => e.Id == id).FirstOrDefault();
            if (evt == null) return NotFound();
            return View(evt);
        }

        // Etkinlik ekle formu (admin)
        public IActionResult Create()
        {
            return View();
        }

        // Etkinlik ekle POST (admin)
        [HttpPost]
        public IActionResult Create(string title, string description, DateTime date, string location, double latitude, double longitude)
        {
            var author = User?.Identity?.IsAuthenticated == true ? User.Identity.Name : "Admin";
            var evt = new Event
            {
                Title = title,
                Description = description,
                Date = date,
                Location = location,
                Latitude = latitude,
                Longitude = longitude,
                CreatedAt = DateTime.UtcNow,
                Author = author
            };
            _eventsCollection.InsertOne(evt);
            return RedirectToAction("Manage");
        }

        // Admin paneli için etkinlik yönetimi
        public IActionResult Manage()
        {
            var list = _eventsCollection.Find(_ => true).SortByDescending(e => e.CreatedAt).ToList();
            return View(list);
        }

        // Etkinlik sil (admin)
        [HttpPost]
        public IActionResult Delete(string id)
        {
            _eventsCollection.DeleteOne(e => e.Id == id);
            return RedirectToAction("Manage");
        }

        // Etkinlik düzenle formu (admin)
        public IActionResult Edit(string id)
        {
            var evt = _eventsCollection.Find(e => e.Id == id).FirstOrDefault();
            if (evt == null) return NotFound();
            return View(evt);
        }

        // Etkinlik düzenle POST (admin)
        [HttpPost]
        public IActionResult Edit(string id, string title, string description, DateTime date, string location, double latitude, double longitude)
        {
            var update = Builders<Event>.Update
                .Set(e => e.Title, title)
                .Set(e => e.Description, description)
                .Set(e => e.Date, date)
                .Set(e => e.Location, location)
                .Set(e => e.Latitude, latitude)
                .Set(e => e.Longitude, longitude);

            _eventsCollection.UpdateOne(e => e.Id == id, update);
            return RedirectToAction("Manage");
        }
    }
}