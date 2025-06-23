using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using MongoDB.Bson;
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

        // DUYURU DETAY GÖRÜNTÜLEME
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Duyuru ID'si gerekli.");
            }

            var announcement = _announcementCollection.Find(a => a.Id == id).FirstOrDefault();
            if (announcement == null)
            {
                return NotFound("Duyuru bulunamadı.");
            }

            return View(announcement);
        }

        // DUYURU EKLEME FORMU (admin için)
        public IActionResult Create()
        {
            return View(); // Views/Announcement/Create.cshtml
        }

        // DUYURU EKLEME POST (admin için)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Announcement model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    ModelState.AddModelError("Title", "Başlık gereklidir.");
                }

                if (string.IsNullOrWhiteSpace(model.Content))
                {
                    ModelState.AddModelError("Content", "İçerik gereklidir.");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var author = User?.Identity?.IsAuthenticated == true
                    ? User.Identity.Name
                    : "Admin";

                var announcement = new Announcement
                {
                    Title = model.Title.Trim(),
                    Content = model.Content.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    Author = author
                };

                _announcementCollection.InsertOne(announcement);
                TempData["SuccessMessage"] = "Duyuru başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Duyuru oluşturulurken bir hata oluştu: " + ex.Message;
                return View(model);
            }
        }

        // DUYURU DÜZENLEME FORMU
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Duyuru ID'si gerekli.";
                return RedirectToAction("Index");
            }

            var announcement = _announcementCollection.Find(a => a.Id == id).FirstOrDefault();
            if (announcement == null)
            {
                TempData["ErrorMessage"] = "Duyuru bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(announcement);
        }

        // DUYURU DÜZENLEME POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Announcement model)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Duyuru ID'si gerekli.";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    ModelState.AddModelError("Title", "Başlık gereklidir.");
                }

                if (string.IsNullOrWhiteSpace(model.Content))
                {
                    ModelState.AddModelError("Content", "İçerik gereklidir.");
                }

                if (!ModelState.IsValid)
                {
                    model.Id = id;
                    return View(model);
                }

                var existingAnnouncement = _announcementCollection.Find(a => a.Id == id).FirstOrDefault();
                if (existingAnnouncement == null)
                {
                    TempData["ErrorMessage"] = "Duyuru bulunamadı.";
                    return RedirectToAction("Index");
                }

                var update = Builders<Announcement>.Update
                    .Set(a => a.Title, model.Title.Trim())
                    .Set(a => a.Content, model.Content.Trim());

                _announcementCollection.UpdateOne(a => a.Id == id, update);
                TempData["SuccessMessage"] = "Duyuru başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Duyuru güncellenirken bir hata oluştu: " + ex.Message;
                model.Id = id;
                return View(model);
            }
        }

        // DUYURU SİLME ONAY SAYFASI
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Duyuru ID'si gerekli.";
                return RedirectToAction("Index");
            }

            var announcement = _announcementCollection.Find(a => a.Id == id).FirstOrDefault();
            if (announcement == null)
            {
                TempData["ErrorMessage"] = "Duyuru bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(announcement);
        }

        // DUYURU SİLME ONAY POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Duyuru ID'si gerekli.";
                    return RedirectToAction("Index");
                }

                var result = _announcementCollection.DeleteOne(a => a.Id == id);
                if (result.DeletedCount > 0)
                {
                    TempData["SuccessMessage"] = "Duyuru başarıyla silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Duyuru bulunamadı veya silinemedi.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Duyuru silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
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