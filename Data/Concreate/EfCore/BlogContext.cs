using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concreate.EfCore
{
    public class BlogContext:DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
        }
          
        public virtual DbSet<User> Users => Set<User>();
        public virtual DbSet<Post> Posts => Set<Post>();
        public virtual DbSet<Comment> Comments => Set<Comment>();
        public virtual DbSet<Tag> Tags => Set<Tag>();
        public virtual DbSet<Reply> Replies => Set<Reply>();

    }
}
