using System.ComponentModel.DataAnnotations;

namespace EduSphere.Models
{
    public class Post
    {
        public int Id { get; set; } 
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
        public string UserId { get; set; } 
        public string Author { get; set; }
    }
}