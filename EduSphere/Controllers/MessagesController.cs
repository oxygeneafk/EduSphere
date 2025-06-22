using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;

[Authorize]
public class MessagesController : Controller
{
    private readonly IMongoCollection<Message> _messagesCollection;
    private readonly IMongoCollection<ChatUser> _usersCollection;

    public MessagesController(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase("EduSphere");
        _messagesCollection = database.GetCollection<Message>("Messages");
        _usersCollection = database.GetCollection<ChatUser>("Users");
    }

    // Ana chat ekranı: kullanıcıları arayabilir ve seçebilir
    [HttpGet]
    public IActionResult Index(string search = "")
    {
        string currentUsername = User.FindFirst("Username")?.Value ?? "";
        List<ChatUser> users;

        if (!string.IsNullOrEmpty(search))
        {
            users = _usersCollection
                .Find(u => u.Username != currentUsername &&
                          (u.Username.ToLower().Contains(search.ToLower()) ||
                           u.Name.ToLower().Contains(search.ToLower())))
                .ToList();
        }
        else
        {
            users = _usersCollection
                .Find(u => u.Username != currentUsername)
                .Limit(20)
                .ToList();
        }

        ViewBag.CurrentUsername = currentUsername;
        return View(users);
    }

    // Belirli bir kullanıcıyla mesajlaşmak için chat ekranı
    [HttpGet]
    public IActionResult Chat(string toUsername)
    {
        string currentUsername = User.FindFirst("Username")?.Value ?? "";
        if (string.IsNullOrEmpty(toUsername) || toUsername == currentUsername)
        {
            return RedirectToAction("Index");
        }

        var messages = _messagesCollection
            .Find(m =>
                (m.FromUsername == currentUsername && m.ToUsername == toUsername) ||
                (m.FromUsername == toUsername && m.ToUsername == currentUsername))
            .SortBy(m => m.SentAt)
            .ToList();

        ViewBag.ToUsername = toUsername;
        ViewBag.CurrentUsername = currentUsername;
        return View(messages);
    }

    // Mesaj gönderme (AJAX ile de kullanılabilir) + medya desteği
    [HttpPost]
    public IActionResult Send(string toUsername, string content, IFormFile media)
    {
        string fromUsername = User.FindFirst("Username")?.Value ?? "";

        // Hem mesaj hem medya boşsa gönderme!
        if (string.IsNullOrWhiteSpace(toUsername) ||
            (string.IsNullOrWhiteSpace(content) && (media == null || media.Length == 0)))
        {
            return BadRequest();
        }

        string mediaUrl = null;
        if (media != null && media.Length > 0)
        {
            var ext = Path.GetExtension(media.FileName).ToLower();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".webm" };

            if (allowed.Contains(ext))
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "chat");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    media.CopyTo(stream);
                }

                mediaUrl = "/uploads/chat/" + fileName;
            }
            else
            {
                // Geçersiz dosya tipi
                return BadRequest("Sadece resim veya video dosyası yükleyebilirsiniz.");
            }
        }

        var message = new Message
        {
            FromUsername = fromUsername,
            ToUsername = toUsername,
            Content = content,
            MediaUrl = mediaUrl,
            SentAt = DateTime.UtcNow
        };

        _messagesCollection.InsertOne(message);

        // Yönlendirme chat ekranına
        return RedirectToAction("Chat", new { toUsername });
    }
}