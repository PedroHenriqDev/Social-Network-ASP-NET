using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialWeave.Migrations
{
    /// <inheritdoc />
    public partial class Eight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dislike_Posts_PostWithImageId",
                table: "Dislike");

            migrationBuilder.DropIndex(
                name: "IX_Dislike_PostWithImageId",
                table: "Dislike");

            migrationBuilder.DropColumn(
                name: "PostWithImageId",
                table: "Dislike");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PostWithImageId",
                table: "Dislike",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dislike_PostWithImageId",
                table: "Dislike",
                column: "PostWithImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dislike_Posts_PostWithImageId",
                table: "Dislike",
                column: "PostWithImageId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
