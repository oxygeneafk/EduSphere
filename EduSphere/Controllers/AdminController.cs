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
                // Burada admin için session/cookie vs. başlatabilirsin
                return RedirectToAction("Dashboard");
            }
            ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış.");
            return View(model);
        }

        // (İstersen Index action da ekle, istersen ekleme)
        public IActionResult Dashboard()
        {
            // Admin ana sayfası, kullanıcı yönetimine yönlendir
            return View();
        }
    }
}