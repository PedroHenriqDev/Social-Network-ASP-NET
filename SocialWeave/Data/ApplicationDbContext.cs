using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialWeave.Models.AbstractClasses;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Data
{
    /// <summary>
    /// Represents the application's database context, providing access to the database tables.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for configuring the DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Default constructor for ApplicationDbContext.
        /// </summary>
        public ApplicationDbContext()
        {
        }

        /// <summary>
        /// Represents the Users table in the database.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Represents the Notifications table in the database.
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Represents the Posts table in the database.
        /// </summary>
        public DbSet<Post> Posts { get; set; }

        public DbSet<SavedPost> SavedPosts {  get; set; }

        /// <summary>
        /// Represents the Likes table in the database.
        /// </summary>
        public DbSet<Like> Likes { get; set; }

        /// <summary>
        /// Represents the Admirations table in the database.
        /// </summary>
        public DbSet<Admiration> Admirations { get; set; }

        /// <summary>
        /// Represents the Comments table in the database.
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// Configures the entity models and relationships for the database.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the database.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<SavedPost>()
                .HasKey(sp => new { sp.UserId, sp.PostId });

            modelBuilder.Entity<SavedPost>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.SavedPosts)
                .HasForeignKey(sp => sp.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<SavedPost>()
                .HasOne(sp => sp.Post)
                .WithMany()
                .HasForeignKey(sp => sp.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Notification entity
            modelBuilder.Entity<Notification>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Notification>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(c => c.InvolvedUser)
                .WithMany()
                .HasForeignKey(c => c.InvolvedUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Post entity
            modelBuilder.Entity<Post>()
                .HasDiscriminator<string>("PostType")
                .HasValue<PostWithoutImage>("PostWithoutImage")
                .HasValue<PostWithImage>("PostWithImage");

            // Configure the Admiration entity
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
