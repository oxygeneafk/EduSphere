using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EduSphere.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly IMongoCollection<Announcement> _announcementCollection;

        public AnnouncementController(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("EduSphere");
            _announcementCollection = database.GetCollection<Announcement>("Announcements");
        }

        // YÖNETİCİ DUYURU LİSTESİ (admin veya yönetim için)
        public IActionResult Index()
        {
            var list = _announcementCollection.Find(_ => true)
                        .SortByDescending(a => a.CreatedAt)
                        .ToList();
            return View(list); // Views/Announcement/Index.cshtml
        }

        // DUYURU EKLEME FORMU (admin için)
        public IActionResult Create()
        {
            return View(); // Views/Announcement/Create.cshtml
        }

        // DUYURU EKLEME POST (admin için)
        [HttpPost]
        public IActionResult Create(string title, string content)
        {
            var author = User?.Identity?.IsAuthenticated == true
                ? User.Identity.Name
                : "Admin";

            var announcement = new Announcement
            {
                Title = title,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                Author = author
            };
            _announcementCollection.InsertOne(announcement);
            return RedirectToAction("Index");
        }

        // KULLANICIYA AÇIK DUYURULAR (public liste)
        public IActionResult Public()
        {
            var list = _announcementCollection.Find(_ => true)
                        .SortByDescending(a => a.CreatedAt)
                        .ToList();
            return View(list); // Views/Announcement/Public.cshtml
        }
    }
}