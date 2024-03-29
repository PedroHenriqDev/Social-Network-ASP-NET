﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialWeave.Data;

#nullable disable

namespace SocialWeave.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240321185331_Second")]
    partial class Second
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SocialWeave.Models.AbstractClasses.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostType")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("nvarchar(21)");

                    b.Property<double?>("Score")
                        .HasColumnType("float");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");

                    b.HasDiscriminator<string>("PostType").HasValue("Post");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Admiration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserAdmiredId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserAdmirerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserAdmiredId");

                    b.HasIndex("UserAdmirerId");

                    b.ToTable("Admirations");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Like", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CommentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("InvolvedUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("WasSeen")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("InvolvedUserId");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.SavedPost", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "PostId");

                    b.HasIndex("PostId");

                    b.ToTable("SavedPost");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateCreation")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PictureProfile")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.PostWithImage", b =>
                {
                    b.HasBaseType("SocialWeave.Models.AbstractClasses.Post");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.HasDiscriminator().HasValue("PostWithImage");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.PostWithoutImage", b =>
                {
                    b.HasBaseType("SocialWeave.Models.AbstractClasses.Post");

                    b.HasDiscriminator().HasValue("PostWithoutImage");
                });

            modelBuilder.Entity("SocialWeave.Models.AbstractClasses.Post", b =>
                {
                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Admiration", b =>
                {
                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "UserAdmired")
                        .WithMany()
                        .HasForeignKey("UserAdmiredId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "UserAdmirer")
                        .WithMany()
                        .HasForeignKey("UserAdmirerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("UserAdmired");

                    b.Navigation("UserAdmirer");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Comment", b =>
                {
                    b.HasOne("SocialWeave.Models.AbstractClasses.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId");

                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Like", b =>
                {
                    b.HasOne("SocialWeave.Models.ConcreteClasses.Comment", "Comment")
                        .WithMany("Likes")
                        .HasForeignKey("CommentId");

                    b.HasOne("SocialWeave.Models.AbstractClasses.Post", "Post")
                        .WithMany("Likes")
                        .HasForeignKey("PostId");

                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Notification", b =>
                {
                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "InvolvedUser")
                        .WithMany()
                        .HasForeignKey("InvolvedUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("InvolvedUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.SavedPost", b =>
                {
                    b.HasOne("SocialWeave.Models.AbstractClasses.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "User")
                        .WithMany("SavedPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.User", b =>
                {
                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", null)
                        .WithMany("Admirations")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SocialWeave.Models.AbstractClasses.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Comment", b =>
                {
                    b.Navigation("Likes");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.User", b =>
                {
                    b.Navigation("Admirations");

                    b.Navigation("Posts");

                    b.Navigation("SavedPosts");
                });
#pragma warning restore 612, 618
        }
    }
}
