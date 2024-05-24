using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP.Data.Migrations
{
    /// <inheritdoc />
    public partial class alter_FriendRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasWaved",
                table: "FriendRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasWaved",
                table: "FriendRequests");
        }
    }
}
