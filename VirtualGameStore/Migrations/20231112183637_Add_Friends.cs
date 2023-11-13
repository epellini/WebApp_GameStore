using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualGameStore.Migrations
{
    public partial class Add_Friends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendConnects",
                columns: table => new
                {
                    FriendConnectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FriendId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateConnected = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendConnects", x => x.FriendConnectId);
                    table.ForeignKey(
                        name: "FK_FriendConnects_AspNetUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FriendConnects_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendConnects_FriendId",
                table: "FriendConnects",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendConnects_UserId",
                table: "FriendConnects",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendConnects");
        }
    }
}
