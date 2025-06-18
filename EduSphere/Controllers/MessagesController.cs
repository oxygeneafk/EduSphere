using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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

    // Mesaj gönderme (AJAX ile de kullanılabilir)
    [HttpPost]
    public IActionResult Send(string toUsername, string content)
    {
        string fromUsername = User.FindFirst("Username")?.Value ?? "";

        if (string.IsNullOrWhiteSpace(toUsername) || string.IsNullOrWhiteSpace(content))
        {
            return BadRequest();
        }

        var message = new Message
        {
            FromUsername = fromUsername,
            ToUsername = toUsername,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        _messagesCollection.InsertOne(message);

        // Yönlendirme chat ekranına
        return RedirectToAction("Chat", new { toUsername });
    }
}