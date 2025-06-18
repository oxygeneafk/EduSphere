using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Controllers
{
    public class AcademyController : Controller
    {
        public IActionResult ExamCalendar() => View();
        public IActionResult LessonSchedule() => View();
        public IActionResult ExamResults() => View();
        public IActionResult Teachers() => View();
        public IActionResult Department() => View();
    }
}