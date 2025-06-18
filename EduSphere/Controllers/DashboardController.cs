using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using System.Collections.Generic;

public class DashboardController : Controller
{
    private readonly IMongoCollection<User> _usersCollection;

    public DashboardController(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase("EduSphere");
        _usersCollection = database.GetCollection<User>("Users");
    }

    // Kullanıcıları listele
    public IActionResult Index()
    {
        var users = _usersCollection.Find(_ => true).ToList();
        return View(users);
    }

    // Kullanıcı Düzenle GET
    [HttpGet]
    public IActionResult Edit(string id)
    {
        var user = _usersCollection.Find(u => u.Id == id).FirstOrDefault();
        if (user == null) return NotFound();
        return View(user);
    }

    // Kullanıcı Düzenle POST
    [HttpPost]
    public IActionResult Edit(string id, User editedUser)
    {
        var update = Builders<User>.Update
            .Set(u => u.Name, editedUser.Name)
            .Set(u => u.Lastname, editedUser.Lastname)
            .Set(u => u.Username, editedUser.Username)
            .Set(u => u.Email, editedUser.Email)
            .Set(u => u.PhoneNumber, editedUser.PhoneNumber)
            .Set(u => u.Adress, editedUser.Adress)
            .Set(u => u.Department, editedUser.Department)
            .Set(u => u.Password, editedUser.Password);

        _usersCollection.UpdateOne(u => u.Id == id, update);
        return RedirectToAction("Index");
    }

    // Kullanıcı Sil
    [HttpPost]
    public IActionResult Delete(string id)
    {
        _usersCollection.DeleteOne(u => u.Id == id);
        return RedirectToAction("Index");
    }
}