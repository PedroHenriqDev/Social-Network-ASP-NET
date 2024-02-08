using Microsoft.EntityFrameworkCore;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; } // DbSet para Feedback

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasDiscriminator<string>("PostType")
                .HasValue<PostWithoutImage>("PostWithoutImage")
                .HasValue<PostWithImage>("PostWithImage");

            modelBuilder.Entity<Feedback>()
                .HasDiscriminator<string>("FeedbackType")
                .HasValue<Comment>("Comment")
                .HasValue<Like>("Like");
        }
    }
}
