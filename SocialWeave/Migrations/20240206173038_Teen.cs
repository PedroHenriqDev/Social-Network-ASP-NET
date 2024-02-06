using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialWeave.Migrations
{
    /// <inheritdoc />
    public partial class Teen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Posts_PostId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Dislike_Comment_CommentId",
                table: "Dislike");

            migrationBuilder.DropForeignKey(
                name: "FK_Dislike_Posts_PostId",
                table: "Dislike");

            migrationBuilder.DropForeignKey(
                name: "FK_Dislike_Users_UserId",
                table: "Dislike");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Comment_CommentId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Posts_PostId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Users_UserId",
                table: "Like");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Like",
                table: "Like");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dislike",
                table: "Dislike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Like",
                newName: "Likes");

            migrationBuilder.RenameTable(
                name: "Dislike",
                newName: "Dislikes");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Like_UserId",
                table: "Likes",
                newName: "IX_Likes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Like_PostId",
                table: "Likes",
                newName: "IX_Likes_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Like_CommentId",
                table: "Likes",
                newName: "IX_Likes_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Dislike_UserId",
                table: "Dislikes",
                newName: "IX_Dislikes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Dislike_PostId",
                table: "Dislikes",
                newName: "IX_Dislikes_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Dislike_CommentId",
                table: "Dislikes",
                newName: "IX_Dislikes_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_PostId",
                table: "Comments",
                newName: "IX_Comments_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dislikes",
                table: "Dislikes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dislikes_Comments_CommentId",
                table: "Dislikes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_disLikes_Posts_PostId",
                table: "disLikes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_disLikes_Users_UserId",
                table: "disLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Comments_CommentId",
                table: "Likes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Posts_PostId",
                table: "Likes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_dislikes_Comments_CommentId",
                table: "dislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_dislikes_Posts_PostId",
                table: "dislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_dislikes_Users_UserId",
                table: "dislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_Comments_CommentId",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_Posts_PostId",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "FK_likes_Users_UserId",
                table: "likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_likes",
                table: "likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dislikes",
                table: "dislikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "likes",
                newName: "Like");

            migrationBuilder.RenameTable(
                name: "dislikes",
                newName: "Dislike");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_likes_UserId",
                table: "Like",
                newName: "IX_Like_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_likes_PostId",
                table: "Like",
                newName: "IX_Like_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_likes_CommentId",
                table: "Like",
                newName: "IX_Like_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_dislikes_UserId",
                table: "Dislike",
                newName: "IX_Dislike_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_dislikes_PostId",
                table: "Dislike",
                newName: "IX_Dislike_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_dislikes_CommentId",
                table: "Dislike",
                newName: "IX_Dislike_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_PostId",
                table: "Comment",
                newName: "IX_Comment_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Like",
                table: "Like",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dislike",
                table: "Dislike",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Posts_PostId",
                table: "Comment",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dislike_Comment_CommentId",
                table: "Dislike",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dislike_Posts_PostId",
                table: "Dislike",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dislike_Users_UserId",
                table: "Dislike",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Comment_CommentId",
                table: "Like",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Posts_PostId",
                table: "Like",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Users_UserId",
                table: "Like",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
