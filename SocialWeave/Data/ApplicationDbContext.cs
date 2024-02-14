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

        public ApplicationDbContext() 
        {
        }               

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasDiscriminator<string>("PostType")
                .HasValue<PostWithoutImage>("PostWithoutImage")
                .HasValue<PostWithImage>("PostWithImage");

            modelBuilder.Entity<Connection>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Connection>()
                .HasOne(c => c.UserConnected)
                .WithMany()
                .HasForeignKey(c => c.UserReceivedConnectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Connection>()
                .HasOne(c => c.UserReceivedConnection)
                .WithMany()
                .HasForeignKey(c => c.UserReceivedConnectionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
