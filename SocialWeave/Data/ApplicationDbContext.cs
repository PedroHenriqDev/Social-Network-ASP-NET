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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Dislike> Dislikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasDiscriminator<string>("PostType")
                .HasValue<PostWithoutImage>("PostWithoutImage")
                .HasValue<PostWithImage>("PostWithImage");
        }
    }
}
