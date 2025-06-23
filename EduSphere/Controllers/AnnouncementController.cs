using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduSphere.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly IMongoCollection<Announcement> _announcementCollection;
        private readonly ILogger<AnnouncementController> _logger;

        public AnnouncementController(IConfiguration configuration, ILogger<AnnouncementController> logger)
        {
            _logger = logger;
            try
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("EduSphere");
                _announcementCollection = database.GetCollection<Announcement>("Announcements");

                // Connection test
                var testFilter = Builders<Announcement>.Filter.Empty;
                var testCount = _announcementCollection.CountDocuments(testFilter);
                _logger.LogInformation($"MongoDB connection successful. Announcement count: {testCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"MongoDB connection failed: {ex.Message}");
                throw;
            }
        }

        // YÖNETİCİ DUYURU LİSTESİ (admin veya yönetim için)
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _announcementCollection.Find(_ => true)
                            .SortByDescending(a => a.CreatedAt)
                            .ToListAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching announcements: {ex.Message}");
                TempData["ErrorMessage"] = "Duyurular yüklenirken hata oluştu.";
                return View(new List<Announcement>());
            }
        }

        // DUYURU DETAY GÖRÜNTÜLEME
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Duyuru ID'si gerekli.");
            }

            try
            {
                var announcement = await _announcementCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
                if (announcement == null)
                {
                    return NotFound("Duyuru bulunamadı.");
                }

                return View(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching announcement details: {ex.Message}");
                return NotFound("Duyuru bulunamadı.");
            }
        }

        // DUYURU EKLEME FORMU (admin için)
        public IActionResult Create()
        {
            return View();
        }

        // DUYURU EKLEME POST (admin için)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Announcement model)
        {
            try
            {
                // Model validation
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    ModelState.AddModelError("Title", "Başlık gereklidir.");
                }

                if (string.IsNullOrWhiteSpace(model.Content))
                {
                    ModelState.AddModelError("Content", "İçerik gereklidir.");
                }

                // Türkçe karakter kontrolü ve temizleme
                if (!string.IsNullOrEmpty(model.Title))
                {
                    model.Title = model.Title.Trim().Normalize();
                }

                if (!string.IsNullOrEmpty(model.Content))
                {
                    model.Content = model.Content.Trim().Normalize();
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var author = User?.Identity?.IsAuthenticated == true
                    ? User.Identity.Name ?? "Admin"
                    : "Admin";

                var announcement = new Announcement
                {
                    // Id'yi null bırak, MongoDB otomatik generate etsin
                    Title = model.Title,
                    Content = model.Content,
                    CreatedAt = DateTime.UtcNow,
                    Author = author
                };

                // Async operation kullan
                await _announcementCollection.InsertOneAsync(announcement);

                _logger.LogInformation($"Announcement created successfully by {author}");
                TempData["SuccessMessage"] = "Duyuru başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Detaylı error logging
                _logger.LogError($"Announcement Create Error: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");

                TempData["ErrorMessage"] = $"Duyuru oluşturulurken bir hata oluştu: {ex.Message}";
                return View(model);
            }
        }

        // DUYURU DÜZENLEME FORMU
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Duyuru ID'si gerekli.";
                return RedirectToAction("Index");
            }

            try
            {
                var announcement = await _announcementCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
                if (announcement == null)
                {
                    TempData["ErrorMessage"] = "Duyuru bulunamadı.";
                    return RedirectToAction("Index");
                }

                return View(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching announcement for edit: {ex.Message}");
                TempData["ErrorMessage"] = "Duyuru yüklenirken hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        // DUYURU DÜZENLEME POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Announcement model)
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

                // Türkçe karakter kontrolü ve temizleme
                if (!string.IsNullOrEmpty(model.Title))
                {
                    model.Title = model.Title.Trim().Normalize();
                }

                if (!string.IsNullOrEmpty(model.Content))
                {
                    model.Content = model.Content.Trim().Normalize();
                }

                if (!ModelState.IsValid)
                {
                    model.Id = id;
                    return View(model);
                }

                var existingAnnouncement = await _announcementCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
                if (existingAnnouncement == null)
                {
                    TempData["ErrorMessage"] = "Duyuru bulunamadı.";
                    return RedirectToAction("Index");
                }

                var update = Builders<Announcement>.Update
                    .Set(a => a.Title, model.Title)
                    .Set(a => a.Content, model.Content);

                await _announcementCollection.UpdateOneAsync(a => a.Id == id, update);

                _logger.LogInformation($"Announcement {id} updated successfully");
                TempData["SuccessMessage"] = "Duyuru başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating announcement: {ex.Message}");
                TempData["ErrorMessage"] = "Duyuru güncellenirken bir hata oluştu: " + ex.Message;
                model.Id = id;
                return View(model);
            }
        }

        // DUYURU SİLME ONAY SAYFASI
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Duyuru ID'si gerekli.";
                return RedirectToAction("Index");
            }

            try
            {
                var announcement = await _announcementCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
                if (announcement == null)
                {
                    TempData["ErrorMessage"] = "Duyuru bulunamadı.";
                    return RedirectToAction("Index");
                }

                return View(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching announcement for delete: {ex.Message}");
                TempData["ErrorMessage"] = "Duyuru yüklenirken hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        // DUYURU SİLME ONAY POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Duyuru ID'si gerekli.";
                    return RedirectToAction("Index");
                }

                var result = await _announcementCollection.DeleteOneAsync(a => a.Id == id);
                if (result.DeletedCount > 0)
                {
                    _logger.LogInformation($"Announcement {id} deleted successfully");
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
                _logger.LogError($"Error deleting announcement: {ex.Message}");
                TempData["ErrorMessage"] = "Duyuru silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // KULLANICIYA AÇIK DUYURULAR (public liste)
        public async Task<IActionResult> Public()
        {
            try
            {
                var list = await _announcementCollection.Find(_ => true)
                            .SortByDescending(a => a.CreatedAt)
                            .ToListAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching public announcements: {ex.Message}");
                return View(new List<Announcement>());
            }
        }
    }
}