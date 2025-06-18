using EduSphere.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Controllers
{
    public class AdminController : Controller
    {
        // GET: /Admin/Login
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (model.UserName == "admin" && model.Password == "123")
            {
                // İleride session/cookie ile geliştirirsin
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış.");
            return View(model);
        }

        public IActionResult Index()
        {
            ViewBag.Active = "dashboard";
            return View();
        }
    }
}