using BlogPost.Models;
using Microsoft.EntityFrameworkCore;
namespace BlogPost.Data
{
    public class BlogDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Определение связи один-ко-многим между Author и Post
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(a => a.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление

            // Добавление начальных данных для авторов
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Name = "Faridun Ikromzoda",
                    Email="faridun12@gmail.com",
                    Password="12345"
                },
                new User 
                { 
                    Id = 2, 
                    Name = "John Sharp",
                    Email="John@gmail.com", 
                    Password="1234" 
                }
            );

            // Добавление начальных данных для постов
            modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    Id = 1,
                    Title = "First Post",
                    Content = "This is the content of the first post.",
                    CreatedAt = DateTime.Now,
                    UserId = 1
                },
                new Post
                {
                    Id = 2,
                    Title = "Second Post",
                    Content = "This is the content of the second post.",
                    CreatedAt = DateTime.Now,
                    UserId = 2 
                }
            );

            base.OnModelCreating(modelBuilder);
        }

    }
}
