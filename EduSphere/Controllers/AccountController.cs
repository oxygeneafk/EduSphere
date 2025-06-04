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
            ViewBag.Error = "Username or password is incorrect";
            return View();
        }
    }

    [HttpPost]
    public IActionResult Register(string name, string lastname, string username, string email, string phone, string address, string password)
    {
        if (_context.Users.Any(u => u.email == email))
        {
            ViewBag.Error = "This email address is already in use.";
            return View();
        }

        if (_context.Users.Any(u => u.phonenumber == phone))
        {
            ViewBag.Error = "This phone number is already in use.";
            return View();
        }

        var user = new User
        {
            name = name,
            lastname = lastname,
            username = username,
            email = email,
            phonenumber = phone,
            adress = address,
            password = password
        };//degişiklik hatip tararfıından sex sex sex sex hayır hayır hayır ardah 3131313131

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Login");
    }
}
