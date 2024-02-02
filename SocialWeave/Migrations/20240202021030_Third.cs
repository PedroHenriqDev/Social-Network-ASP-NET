using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialWeave.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Like",
                table: "Comment");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dislike_Posts_PostWithImageId",
                table: "Dislike");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Dislike_PostWithImageId",
                table: "Dislike");

            migrationBuilder.DropColumn(
                name: "PostWithImageId",
                table: "Dislike");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Like",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
