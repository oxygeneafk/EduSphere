using EduSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EduSphere.Data;

[Authorize]
public class PostController : Controller
{
    private readonly EduSphereContext _context;

    public PostController(EduSphereContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var posts = _context.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return View(posts);
    }

    [HttpPost]
    public IActionResult Create(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            ViewBag.Error = "Text Content cannot be empty!";
            return RedirectToAction("Index");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = User.Identity.Name;

        var post = new Post
        {
            Content = content,
            UserId = userId,
            Author = username
        };

        _context.Posts.Add(post);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}