using EduSphere.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;

namespace EduSphere.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Schedule> _schedules;
        private readonly List<string> _departments = new List<string>
        {
            "Computer Engineering","Computer Programming","Electricity Electironic Engineering","Machinery Engineering","Civil Engineering", "Industiral Engineering",
        };

        public AdminController(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("EduSphere");
            _users = database.GetCollection<User>("Users");
            _schedules = database.GetCollection<Schedule>("Schedules");
        }

        // Admin login screen (GET)
        public IActionResult Login() => View();

        // Admin login check (POST)
        [HttpPost]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (model.UserName == "admin" && model.Password == "123")
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Username or password is incorrect.");
            return View(model);
        }

        // Dashboard
        public IActionResult Index()
        {
            ViewBag.Active = "dashboard";
            return View();
        }

        // List users
        public IActionResult Users()
        {
            ViewBag.Active = "users";
            var users = _users.Find(u => true).ToList();
            return View(users);
        }

        // Edit user (GET)
        public IActionResult EditUser(string id)
        {
            var user = _users.Find(u => u.Id == id).FirstOrDefault();
            if (user == null) return NotFound();
            return View(user);
        }

        // Edit user (POST)
        [HttpPost]
        public IActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                _users.ReplaceOne(u => u.Id == user.Id, user);
                return RedirectToAction("Users");
            }
            return View(user);
        }

        // Delete user confirmation (GET)
        public IActionResult DeleteUser(string id)
        {
            var user = _users.Find(u => u.Id == id).FirstOrDefault();
            if (user == null) return NotFound();
            return View(user);
        }

        // Delete user (POST)
        [HttpPost, ActionName("DeleteUser")]
        public IActionResult DeleteUserConfirmed(string id)
        {
            _users.DeleteOne(u => u.Id == id);
            return RedirectToAction("Users");
        }

        // List schedules
        public IActionResult Schedules()
        {
            ViewBag.Active = "schedules";
            var schedules = _schedules.Find(s => true).ToList();
            return View(schedules);
        }

        // Add/Edit schedule (GET)
        public IActionResult EditSchedule(string department)
        {
            ViewBag.Departments = _departments;
            Schedule schedule = null;
            if (!string.IsNullOrEmpty(department))
                schedule = _schedules.Find(s => s.Department == department).FirstOrDefault();

            if (schedule == null)
            {
                schedule = new Schedule
                {
                    Department = department,
                    Days = new List<DaySchedule>
                    {
                        new DaySchedule{ Day="Monday", Courses=new List<string>() },
                        new DaySchedule{ Day="Tuesday", Courses=new List<string>() },
                        new DaySchedule{ Day="Wednesday", Courses=new List<string>() },
                        new DaySchedule{ Day="Thursday", Courses=new List<string>() },
                        new DaySchedule{ Day="Friday", Courses=new List<string>() },
                    }
                };
            }
            return View(schedule);
        }

        // Add/Edit schedule (POST)
        [HttpPost]
        public IActionResult EditSchedule(Schedule model, List<string> Day, List<string> CoursesStr)
        {
            var daysList = new List<DaySchedule>();
            for (int i = 0; i < Day.Count; i++)
            {
                var courses = (CoursesStr[i] ?? "")
                    .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                daysList.Add(new DaySchedule { Day = Day[i], Courses = courses });
            }

            model.Days = daysList;

            var filter = Builders<Schedule>.Filter.Eq(s => s.Department, model.Department);
            var existing = _schedules.Find(filter).FirstOrDefault();

            if (existing == null)
                _schedules.InsertOne(model);
            else
                _schedules.ReplaceOne(filter, model);

            return RedirectToAction("Schedules");
        }
    }
}