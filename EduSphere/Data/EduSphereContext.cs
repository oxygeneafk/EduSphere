using Microsoft.EntityFrameworkCore;
using EduSphere.Models;

namespace EduSphere.Data
{
    public class EduSphereContext : DbContext
    {
        public EduSphereContext(DbContextOptions<EduSphereContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
    }

}
