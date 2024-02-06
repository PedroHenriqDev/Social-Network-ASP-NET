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
    [Migration("20240206173038_Teen")]
    partial class Teen
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
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

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");

                    b.HasDiscriminator<string>("PostType").HasValue("Post");

                    b.UseTphMappingStrategy();
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
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Dislike", b =>
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

                    b.ToTable("dislikes");
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

                    b.ToTable("likes");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateCreation")
                        .HasColumnType("datetime2");

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

                    b.Property<string>("ResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

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

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Comment", b =>
                {
                    b.HasOne("SocialWeave.Models.AbstractClasses.Post", null)
                        .WithMany("Comments")
                        .HasForeignKey("PostId");

                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Dislike", b =>
                {
                    b.HasOne("SocialWeave.Models.ConcreteClasses.Comment", null)
                        .WithMany("Dislikes")
                        .HasForeignKey("CommentId");

                    b.HasOne("SocialWeave.Models.AbstractClasses.Post", null)
                        .WithMany("Dislikes")
                        .HasForeignKey("PostId");

                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Like", b =>
                {
                    b.HasOne("SocialWeave.Models.ConcreteClasses.Comment", null)
                        .WithMany("Likes")
                        .HasForeignKey("CommentId");

                    b.HasOne("SocialWeave.Models.AbstractClasses.Post", null)
                        .WithMany("Like")
                        .HasForeignKey("PostId");

                    b.HasOne("SocialWeave.Models.ConcreteClasses.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SocialWeave.Models.AbstractClasses.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Dislikes");

                    b.Navigation("Like");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.Comment", b =>
                {
                    b.Navigation("Dislikes");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("SocialWeave.Models.ConcreteClasses.User", b =>
                {
                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
