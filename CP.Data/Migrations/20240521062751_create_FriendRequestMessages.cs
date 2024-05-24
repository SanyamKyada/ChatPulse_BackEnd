using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP.Data.Migrations
{
    /// <inheritdoc />
    public partial class create_FriendRequestMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendRequestMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendRequestId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequestMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendRequestMessages_FriendRequests_FriendRequestId",
                        column: x => x.FriendRequestId,
                        principalTable: "FriendRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequestMessages_FriendRequestId",
                table: "FriendRequestMessages",
                column: "FriendRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendRequestMessages");
        }
    }
}
