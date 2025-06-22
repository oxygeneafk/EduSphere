using Microsoft.AspNetCore.Mvc;
using EduSphere.Models;
using MongoDB.Driver;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

namespace EduSphere.Controllers
{
    public class AcademyController : Controller
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Schedule> _schedulesCollection;
        private readonly IMongoCollection<Exam> _examsCollection;
        private readonly IMongoCollection<ExamResult> _examResultsCollection;

        public AcademyController(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("EduSphere");
            _usersCollection = database.GetCollection<User>("Users");
            _schedulesCollection = database.GetCollection<Schedule>("Schedules");
            _examsCollection = database.GetCollection<Exam>("Exams");
            _examResultsCollection = database.GetCollection<ExamResult>("ExamResults");
        }

        public IActionResult ExamCalendar()
        {
            var username = GetLoggedUsername();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _usersCollection.Find(u => u.Username == username).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcının departmanına ait sınav takvimini getir
            var exams = _examsCollection.Find(e => e.Department == user.Department)
                                       .SortBy(e => e.ExamDate)
                                       .ThenBy(e => e.ExamTime)
                                       .ToList();

            // ExamCalendarViewModel'e dönüştür
            var examCalendarList = exams.Select(e => new ExamCalendarViewModel
            {
                CourseName = e.CourseName,
                Date = e.ExamDate,
                Time = e.ExamTime,
                ExamType = e.ExamType,
                Classroom = e.Classroom,
                Instructor = e.Instructor,
                Duration = e.Duration
            }).ToList();

            ViewBag.UserDepartment = user.Department;
            ViewBag.UserName = $"{user.Name} {user.Lastname}";

            return View(examCalendarList);
        }



        // Ders Programı - Kullanıcının departmanına göre
        public IActionResult LessonSchedule()
        {
            var username = GetLoggedUsername();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _usersCollection.Find(u => u.Username == username).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcının departmanına ait ders programını getir
            var schedule = _schedulesCollection.Find(s => s.Department == user.Department).FirstOrDefault();

            // Eğer kullanıcının departmanı varsa, LessonScheduleViewModel'e dönüştür
            var lessonScheduleList = new List<LessonScheduleViewModel>();

            if (schedule != null)
            {
                foreach (var day in schedule.Days)
                {
                    foreach (var course in day.Courses)
                    {
                        lessonScheduleList.Add(new LessonScheduleViewModel
                        {
                            CourseName = course.CourseName,
                            Day = day.Day,
                            Time = $"{course.StartTime} - {course.EndTime}",
                            TeacherName = course.Instructor
                        });
                    }
                }
            }

            ViewBag.UserDepartment = user.Department;
            ViewBag.UserName = $"{user.Name} {user.Lastname}";

            return View(lessonScheduleList);
        }

        public IActionResult ExamResults()
        {
            var username = GetLoggedUsername();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _usersCollection.Find(u => u.Username == username).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcının sınav sonuçlarını getir
            var examResults = _examResultsCollection.Find(e => e.StudentUsername == username)
                                                  .SortByDescending(e => e.ExamDate)
                                                  .ThenBy(e => e.CourseName)
                                                  .ToList();

            // ExamResultViewModel'e dönüştür
            var examResultsList = examResults.Select(e => new ExamResultViewModel
            {
                CourseName = e.CourseName,
                ExamType = e.ExamType,
                ExamDate = e.ExamDate,
                Score = e.Score,
                LetterGrade = e.LetterGrade,
                IsPassed = e.IsPassed
            }).ToList();

            ViewBag.UserDepartment = user.Department;
            ViewBag.UserName = $"{user.Name} {user.Lastname}";

            return View(examResultsList);
        }
        public IActionResult Teachers() => View();
        public IActionResult Department() => View();

        // Yardımcı metod - Giriş yapan kullanıcı adını al
        private string GetLoggedUsername()
        {
            return User?.Identity?.IsAuthenticated == true
                ? User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value ?? ""
                : "";
        }
    }
}