using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_contraint_UQ_SenderReceiverUserIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_SenderUserId",
                table: "FriendRequests");

            migrationBuilder.CreateIndex(
                name: "UQ_SenderReceiverUserIds",
                table: "FriendRequests",
                columns: new[] { "SenderUserId", "ReceiverUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_SenderReceiverUserIds",
                table: "FriendRequests");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_SenderUserId",
                table: "FriendRequests",
                column: "SenderUserId");
        }
    }
}
