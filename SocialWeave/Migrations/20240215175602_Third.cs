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
            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.CreateTable(
                name: "Admirations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAdmirerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAdmiredId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admirations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admirations_Users_UserAdmiredId",
                        column: x => x.UserAdmiredId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Admirations_Users_UserAdmirerId",
                        column: x => x.UserAdmirerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admirations_UserAdmiredId",
                table: "Admirations",
                column: "UserAdmiredId");

            migrationBuilder.CreateIndex(
                name: "IX_Admirations_UserAdmirerId",
                table: "Admirations",
                column: "UserAdmirerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admirations");

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserConnectedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserReceivedConnectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connections_Users_UserConnectedId",
                        column: x => x.UserConnectedId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Connections_Users_UserReceivedConnectionId",
                        column: x => x.UserReceivedConnectionId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_UserConnectedId",
                table: "Connections",
                column: "UserConnectedId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_UserReceivedConnectionId",
                table: "Connections",
                column: "UserReceivedConnectionId");
        }
    }
}
