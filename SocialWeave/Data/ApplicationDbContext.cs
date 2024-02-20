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
        public DbSet<Admiration> Admirations { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasDiscriminator<string>("PostType")
                .HasValue<PostWithoutImage>("PostWithoutImage")
                .HasValue<PostWithImage>("PostWithImage");

            modelBuilder.Entity<Admiration>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Admiration>()
                .HasOne(c => c.UserAdmired)
                .WithMany()
                .HasForeignKey(c => c.UserAdmiredId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Admiration>()
                .HasOne(c => c.UserAdmirer)
                .WithMany()
                .HasForeignKey(c => c.UserAdmirerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
