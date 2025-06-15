using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly IMongoCollection<User> _usersCollection;

    public AccountController(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase("EduSphere");
        _usersCollection = database.GetCollection<User>("Users");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _usersCollection
            .Find(u => u.Username == username && u.Password == password)
            .FirstOrDefault();

        if (user != null)
        {
            // Kullanıcıyı authenticate et (cookie ile)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name ?? user.Username ?? "Anonim"),
                new Claim("Username", user.Username ?? ""),
                new Claim("Name", user.Name ?? ""),
                new Claim("Lastname", user.Lastname ?? ""),
                new Claim("Email", user.Email ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Timeline");
        }
        else
        {
            ViewBag.Error = "Username or password is incorrect";
            return View();
        }
    }

    [HttpPost]
    public IActionResult Register(string name, string lastname, string username, string email, string phone, string address, string password)
    {
        // Email kontrolü
        var emailExists = _usersCollection.Find(u => u.Email == email).Any();
        if (emailExists)
        {
            ViewBag.Error = "This email address is already in use.";
            return View();
        }

        // Telefon kontrolü
        var phoneExists = _usersCollection.Find(u => u.PhoneNumber == phone).Any();
        if (phoneExists)
        {
            ViewBag.Error = "This phone number is already in use.";
            return View();
        }

        var user = new User
        {
            Name = name,
            Lastname = lastname,
            Username = username,
            Email = email,
            PhoneNumber = phone,
            Adress = address,
            Password = password
        };

        _usersCollection.InsertOne(user);

        return RedirectToAction("Login");
    }
}