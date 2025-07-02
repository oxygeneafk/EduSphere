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
            try
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
                TempData["Success"] = "Etkinlik başarıyla eklendi!";
                return RedirectToAction("Manage");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
                return View();
            }
        }

        // Admin paneli için etkinlik yönetimi
        public IActionResult Manage()
        {
            var list = _eventsCollection.Find(_ => true).SortByDescending(e => e.CreatedAt).ToList();
            return View(list);
        }

        // Etkinlik sil (admin) - GET konfirmasyonu
        public IActionResult Delete(string id)
        {
            var evt = _eventsCollection.Find(e => e.Id == id).FirstOrDefault();
            if (evt == null) return NotFound();
            return View(evt);
        }

        // Etkinlik sil (admin) - POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                var result = _eventsCollection.DeleteOne(e => e.Id == id);
                if (result.DeletedCount > 0)
                {
                    TempData["Success"] = "Etkinlik başarıyla silindi!";
                }
                else
                {
                    TempData["Error"] = "Etkinlik bulunamadı!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
            }
            return RedirectToAction("Manage");
        }

        // Etkinlik hızlı sil (AJAX için)
        [HttpPost]
        public IActionResult QuickDelete(string id)
        {
            try
            {
                var result = _eventsCollection.DeleteOne(e => e.Id == id);
                if (result.DeletedCount > 0)
                {
                    return Json(new { success = true, message = "Etkinlik başarıyla silindi!" });
                }
                else
                {
                    return Json(new { success = false, message = "Etkinlik bulunamadı!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
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
            try
            {
                var update = Builders<Event>.Update
                    .Set(e => e.Title, title)
                    .Set(e => e.Description, description)
                    .Set(e => e.Date, date)
                    .Set(e => e.Location, location)
                    .Set(e => e.Latitude, latitude)
                    .Set(e => e.Longitude, longitude);

                var result = _eventsCollection.UpdateOne(e => e.Id == id, update);
                if (result.ModifiedCount > 0)
                {
                    TempData["Success"] = "Etkinlik başarıyla güncellendi!";
                }
                else
                {
                    TempData["Error"] = "Etkinlik güncellenemedi!";
                }
                return RedirectToAction("Manage");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata: " + ex.Message;
                return View();
            }
        }
    }
}