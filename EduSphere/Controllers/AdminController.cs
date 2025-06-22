using EduSphere.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System;

namespace EduSphere.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Schedule> _schedules;
        private readonly IMongoCollection<Exam> _exams;
        private readonly IMongoCollection<ExamResult> _examResults; // YENİ EKLENEN
        private readonly List<string> _departments = new List<string>
        {
            "Computer Engineering",
            "Computer Programming",
            "Electricity Electronic Engineering",
            "Machinery Engineering",
            "Civil Engineering",
            "Industrial Engineering"
        };

        public AdminController(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("EduSphere");
            _users = database.GetCollection<User>("Users");
            _schedules = database.GetCollection<Schedule>("Schedules");
            _exams = database.GetCollection<Exam>("Exams");
            _examResults = database.GetCollection<ExamResult>("ExamResults"); // YENİ EKLENEN
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

        // === DERS PROGRAMI YÖNETİMİ ===

        // List schedules - Announcement Index mantığı gibi
        public IActionResult Schedules()
        {
            ViewBag.Active = "schedules";
            var schedules = _schedules.Find(s => true)
                          .SortByDescending(s => s.UpdatedAt)
                          .ToList();
            return View(schedules);
        }

        // Create schedule form - Register screen departman seçimi mantığı
        public IActionResult CreateSchedule()
        {
            ViewBag.Departments = _departments;
            ViewBag.Active = "schedules";
            return View();
        }

        // Create schedule POST - Announcement Create mantığı
        [HttpPost]
        public IActionResult CreateSchedule(string department)
        {
            if (string.IsNullOrEmpty(department))
            {
                ViewBag.Error = "Lütfen bir departman seçiniz.";
                ViewBag.Departments = _departments;
                ViewBag.Active = "schedules";
                return View();
            }

            // Check if schedule already exists
            var existingSchedule = _schedules.Find(s => s.Department == department).FirstOrDefault();
            if (existingSchedule != null)
            {
                return RedirectToAction("EditSchedule", new { department = department });
            }

            var schedule = new Schedule
            {
                Department = department,
                Days = new List<DaySchedule>
                {
                    new DaySchedule { Day = "Pazartesi", Courses = new List<CourseSchedule>() },
                    new DaySchedule { Day = "Salı", Courses = new List<CourseSchedule>() },
                    new DaySchedule { Day = "Çarşamba", Courses = new List<CourseSchedule>() },
                    new DaySchedule { Day = "Perşembe", Courses = new List<CourseSchedule>() },
                    new DaySchedule { Day = "Cuma", Courses = new List<CourseSchedule>() }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };

            _schedules.InsertOne(schedule);
            return RedirectToAction("EditSchedule", new { department = department });
        }

        // Edit schedule (GET)
        public IActionResult EditSchedule(string department)
        {
            if (string.IsNullOrEmpty(department))
            {
                return RedirectToAction("Schedules");
            }

            ViewBag.Departments = _departments;
            ViewBag.Active = "schedules";

            var schedule = _schedules.Find(s => s.Department == department).FirstOrDefault();

            if (schedule == null)
            {
                schedule = new Schedule
                {
                    Department = department,
                    Days = new List<DaySchedule>
                    {
                        new DaySchedule { Day = "Pazartesi", Courses = new List<CourseSchedule>() },
                        new DaySchedule { Day = "Salı", Courses = new List<CourseSchedule>() },
                        new DaySchedule { Day = "Çarşamba", Courses = new List<CourseSchedule>() },
                        new DaySchedule { Day = "Perşembe", Courses = new List<CourseSchedule>() },
                        new DaySchedule { Day = "Cuma", Courses = new List<CourseSchedule>() }
                    }
                };
            }

            return View(schedule);
        }

        // Edit schedule (POST)
        [HttpPost]
        public IActionResult EditSchedule(Schedule model)
        {
            if (string.IsNullOrEmpty(model.Department))
            {
                ViewBag.Error = "Departman bilgisi gereklidir.";
                ViewBag.Departments = _departments;
                ViewBag.Active = "schedules";
                return View(model);
            }

            model.UpdatedAt = DateTime.UtcNow;

            var filter = Builders<Schedule>.Filter.Eq(s => s.Department, model.Department);
            var existing = _schedules.Find(filter).FirstOrDefault();

            if (existing == null)
            {
                model.CreatedAt = DateTime.UtcNow;
                model.CreatedBy = "Admin";
                _schedules.InsertOne(model);
            }
            else
            {
                model.Id = existing.Id;
                model.CreatedAt = existing.CreatedAt;
                model.CreatedBy = existing.CreatedBy;
                _schedules.ReplaceOne(filter, model);
            }

            return RedirectToAction("Schedules");
        }

        // Delete schedule
        [HttpPost]
        public IActionResult DeleteSchedule(string department)
        {
            if (!string.IsNullOrEmpty(department))
            {
                _schedules.DeleteOne(s => s.Department == department);
            }
            return RedirectToAction("Schedules");
        }

        // Add course to specific day
        [HttpPost]
        public IActionResult AddCourse(string department, string day, string courseName, string startTime, string endTime, string instructor, string classroom)
        {
            var schedule = _schedules.Find(s => s.Department == department).FirstOrDefault();
            if (schedule != null)
            {
                var daySchedule = schedule.Days.FirstOrDefault(d => d.Day == day);
                if (daySchedule != null)
                {
                    var newCourse = new CourseSchedule
                    {
                        CourseName = courseName,
                        StartTime = startTime,
                        EndTime = endTime,
                        Instructor = instructor,
                        Classroom = classroom
                    };

                    daySchedule.Courses.Add(newCourse);
                    schedule.UpdatedAt = DateTime.UtcNow;

                    var filter = Builders<Schedule>.Filter.Eq(s => s.Department, department);
                    _schedules.ReplaceOne(filter, schedule);
                }
            }
            return RedirectToAction("EditSchedule", new { department = department });
        }

        // Remove course from specific day
        [HttpPost]
        public IActionResult RemoveCourse(string department, string day, int courseIndex)
        {
            var schedule = _schedules.Find(s => s.Department == department).FirstOrDefault();
            if (schedule != null)
            {
                var daySchedule = schedule.Days.FirstOrDefault(d => d.Day == day);
                if (daySchedule != null && courseIndex >= 0 && courseIndex < daySchedule.Courses.Count)
                {
                    daySchedule.Courses.RemoveAt(courseIndex);
                    schedule.UpdatedAt = DateTime.UtcNow;

                    var filter = Builders<Schedule>.Filter.Eq(s => s.Department, department);
                    _schedules.ReplaceOne(filter, schedule);
                }
            }
            return RedirectToAction("EditSchedule", new { department = department });
        }
        // List exams - Announcement Index mantığı gibi
        public IActionResult Exams()
        {
            ViewBag.Active = "exams";
            var exams = _exams.Find(e => true)
                      .SortBy(e => e.ExamDate)
                      .ThenBy(e => e.ExamTime)
                      .ToList();
            return View(exams);
        }

        // Create exam form - Register screen departman seçimi mantığı
        public IActionResult CreateExam()
        {
            ViewBag.Departments = _departments;
            ViewBag.Active = "exams";

            // Tüm departmanların derslerini getir
            var allCourses = new Dictionary<string, List<string>>();
            foreach (var dept in _departments)
            {
                var schedule = _schedules.Find(s => s.Department == dept).FirstOrDefault();
                var courses = new List<string>();
                if (schedule != null)
                {
                    courses = schedule.Days
                        .SelectMany(d => d.Courses)
                        .Select(c => c.CourseName)
                        .Distinct()
                        .ToList();
                }
                allCourses[dept] = courses;
            }

            ViewBag.DepartmentCourses = allCourses;
            return View();
        }

        // Create exam POST - Announcement Create mantığı
        [HttpPost]
        public IActionResult CreateExam(string department, string courseName, DateTime examDate,
            TimeSpan examTime, string examType, string classroom, string instructor, int duration)
        {
            if (string.IsNullOrEmpty(department) || string.IsNullOrEmpty(courseName))
            {
                ViewBag.Error = "Lütfen tüm alanları doldurunuz.";
                ViewBag.Departments = _departments;
                ViewBag.Active = "exams";
                return View();
            }

            var exam = new Exam
            {
                Department = department,
                CourseName = courseName,
                ExamDate = examDate,
                ExamTime = examTime,
                ExamType = examType,
                Classroom = classroom,
                Instructor = instructor,
                Duration = duration,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };

            _exams.InsertOne(exam);
            return RedirectToAction("Exams");
        }

        // Edit exam (GET)
        public IActionResult EditExam(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Exams");
            }

            ViewBag.Departments = _departments;
            ViewBag.Active = "exams";

            var exam = _exams.Find(e => e.Id == id).FirstOrDefault();
            if (exam == null)
            {
                return RedirectToAction("Exams");
            }

            // Bu departmanın derslerini getir
            var schedule = _schedules.Find(s => s.Department == exam.Department).FirstOrDefault();
            var courses = new List<string>();
            if (schedule != null)
            {
                courses = schedule.Days
                    .SelectMany(d => d.Courses)
                    .Select(c => c.CourseName)
                    .Distinct()
                    .ToList();
            }
            ViewBag.DepartmentCourses = courses;

            return View(exam);
        }

        // Edit exam (POST)
        [HttpPost]
        public IActionResult EditExam(Exam model)
        {
            if (string.IsNullOrEmpty(model.Department) || string.IsNullOrEmpty(model.CourseName))
            {
                ViewBag.Error = "Lütfen tüm alanları doldurunuz.";
                ViewBag.Departments = _departments;
                ViewBag.Active = "exams";
                return View(model);
            }

            model.UpdatedAt = DateTime.UtcNow;

            var filter = Builders<Exam>.Filter.Eq(e => e.Id, model.Id);
            var existing = _exams.Find(filter).FirstOrDefault();

            if (existing != null)
            {
                model.CreatedAt = existing.CreatedAt;
                model.CreatedBy = existing.CreatedBy;
                _exams.ReplaceOne(filter, model);
            }

            return RedirectToAction("Exams");
        }

        // Delete exam
        [HttpPost]
        public IActionResult DeleteExam(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                _exams.DeleteOne(e => e.Id == id);
            }
            return RedirectToAction("Exams");
        }

        // === SINAV SONUÇLARI YÖNETİMİ ===

        // List exam results
        public IActionResult ExamResults()
        {
            ViewBag.Active = "examresults";
            var results = _examResults.Find(r => true)
                        .SortByDescending(r => r.CreatedAt)
                        .ToList();
            return View(results);
        }

        // Create exam result form
        public IActionResult CreateExamResult()
        {
            ViewBag.Active = "examresults";
            ViewBag.Departments = _departments;

            // Geçmiş sınavları getir (bugünden önce olan sınavlar)
            var pastExams = _exams.Find(e => e.ExamDate < DateTime.UtcNow.Date)
                          .SortByDescending(e => e.ExamDate)
                          .ToList();
            ViewBag.PastExams = pastExams;

            return View();
        }

        // Create exam result POST
        [HttpPost]
        public IActionResult CreateExamResult(string examId, string studentUsername, double score)
        {
            if (string.IsNullOrEmpty(examId) || string.IsNullOrEmpty(studentUsername))
            {
                ViewBag.Error = "Lütfen tüm alanları doldurunuz.";
                return RedirectToAction("CreateExamResult");
            }

            // Sınavı getir
            var exam = _exams.Find(e => e.Id == examId).FirstOrDefault();
            if (exam == null)
            {
                ViewBag.Error = "Sınav bulunamadı.";
                return RedirectToAction("CreateExamResult");
            }

            // Öğrenciyi getir
            var student = _users.Find(u => u.Username == studentUsername).FirstOrDefault();
            if (student == null)
            {
                ViewBag.Error = "Öğrenci bulunamadı.";
                return RedirectToAction("CreateExamResult");
            }

            // Bu sınav için bu öğrencinin sonucu var mı kontrol et
            var existingResult = _examResults.Find(r => r.ExamId == examId && r.StudentUsername == studentUsername).FirstOrDefault();
            if (existingResult != null)
            {
                ViewBag.Error = "Bu öğrenci için bu sınavın sonucu zaten girilmiş.";
                return RedirectToAction("CreateExamResult");
            }

            // Harf notunu hesapla
            string letterGrade = CalculateLetterGrade(score);
            bool isPassed = score >= 60;

            var examResult = new ExamResult
            {
                StudentId = student.Id,
                StudentUsername = student.Username,
                StudentName = $"{student.Name} {student.Lastname}",
                StudentDepartment = student.Department,
                ExamId = exam.Id,
                CourseName = exam.CourseName,
                ExamType = exam.ExamType,
                ExamDate = exam.ExamDate,
                Score = score,
                LetterGrade = letterGrade,
                IsPassed = isPassed,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };

            _examResults.InsertOne(examResult);
            return RedirectToAction("ExamResults");
        }

        // Edit exam result (GET)
        public IActionResult EditExamResult(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("ExamResults");
            }

            ViewBag.Active = "examresults";
            var result = _examResults.Find(r => r.Id == id).FirstOrDefault();
            if (result == null)
            {
                return RedirectToAction("ExamResults");
            }

            return View(result);
        }

        // Edit exam result (POST)
        [HttpPost]
        public IActionResult EditExamResult(ExamResult model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                ViewBag.Error = "Geçersiz işlem.";
                return View(model);
            }

            // Harf notunu yeniden hesapla
            model.LetterGrade = CalculateLetterGrade(model.Score);
            model.IsPassed = model.Score >= 60;
            model.UpdatedAt = DateTime.UtcNow;

            var filter = Builders<ExamResult>.Filter.Eq(r => r.Id, model.Id);
            var existing = _examResults.Find(filter).FirstOrDefault();

            if (existing != null)
            {
                model.CreatedAt = existing.CreatedAt;
                model.CreatedBy = existing.CreatedBy;
                _examResults.ReplaceOne(filter, model);
            }

            return RedirectToAction("ExamResults");
        }

        // Delete exam result
        [HttpPost]
        public IActionResult DeleteExamResult(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                _examResults.DeleteOne(r => r.Id == id);
            }
            return RedirectToAction("ExamResults");
        }

        // Get students by department (AJAX endpoint)
        [HttpGet]
        public IActionResult GetStudentsByDepartment(string department)
        {
            if (string.IsNullOrEmpty(department))
            {
                return Json(new List<object>());
            }

            var students = _users.Find(u => u.Department == department && u.Role == "Student")
                          .ToList()
                          .Select(s => new {
                              username = s.Username,
                              name = $"{s.Name} {s.Lastname}"
                          })
                          .OrderBy(s => s.name)
                          .ToList();

            return Json(students);
        }

        // Get exams by department (AJAX endpoint)
        [HttpGet]
        public IActionResult GetExamsByDepartment(string department)
        {
            if (string.IsNullOrEmpty(department))
            {
                return Json(new List<object>());
            }

            var exams = _exams.Find(e => e.Department == department && e.ExamDate < DateTime.UtcNow.Date)
                      .ToList()
                      .Select(e => new {
                          id = e.Id,
                          display = $"{e.CourseName} - {e.ExamType} ({e.ExamDate:dd.MM.yyyy})"
                      })
                      .OrderByDescending(e => e.display)
                      .ToList();

            return Json(exams);
        }

        // Get courses by department (AJAX endpoint)
        [HttpGet]
        public IActionResult GetCoursesByDepartment(string department)
        {
            if (string.IsNullOrEmpty(department))
            {
                return Json(new List<string>());
            }

            var schedule = _schedules.Find(s => s.Department == department).FirstOrDefault();
            var courses = new List<string>();

            if (schedule != null)
            {
                courses = schedule.Days
                    .SelectMany(d => d.Courses)
                    .Select(c => c.CourseName)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
            }

            return Json(courses);
        }

        // Harf notu hesaplama metodu
        private string CalculateLetterGrade(double score)
        {
            if (score >= 90) return "AA";
            if (score >= 85) return "BA";
            if (score >= 80) return "BB";
            if (score >= 75) return "CB";
            if (score >= 70) return "CC";
            if (score >= 65) return "DC";
            if (score >= 60) return "DD";
            if (score >= 50) return "FD";
            return "FF";
        }
    }
}