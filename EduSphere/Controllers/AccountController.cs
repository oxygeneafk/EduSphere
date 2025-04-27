using Microsoft.AspNetCore.Mvc;
using EduSphere.Data;
using System.Linq;
using EduSphere.Models;


public class AccountController : Controller
{
    private readonly EduSphereContext _context;

    public AccountController(EduSphereContext context)
    {
        _context = context;
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
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.username == username && u.password == password);

        if (user != null)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.Error = "Kullanıcı adı veya şifre yanlış";
            return View();
        }
    }

    [HttpPost]
    public IActionResult Register(string name, string lastname, string username, string email, string phone, string address, string password)
    {
        // Aynı e-posta veya telefon numarasını kontrol et
        if (_context.Users.Any(u => u.email == email))
        {
            ViewBag.Error = "Bu e-posta adresi zaten kullanılıyor.";
            return View();
        }

        if (_context.Users.Any(u => u.phonenumber == phone))
        {
            ViewBag.Error = "Bu telefon numarası zaten kullanılıyor.";
            return View();
        }

        // Yeni kullanıcı oluştur ve veritabanına kaydet
        var user = new User
        {
            name = name,
            lastname = lastname,
            username = username,
            email = email,
            phonenumber = phone,
            adress = address,
            password = password
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Login");
    }
}
