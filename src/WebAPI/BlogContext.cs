using Microsoft.EntityFrameworkCore;

namespace WebAPI;

public class BlogContext(DbContextOptions<BlogContext> options) : DbContext(options)
{
    public DbSet<Blog> Blogs { get; set; }
}
