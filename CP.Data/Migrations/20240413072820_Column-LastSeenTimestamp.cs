using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP.Data.Migrations
{
    /// <inheritdoc />
    public partial class ColumnLastSeenTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeenTimestamp",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSeenTimestamp",
                table: "AspNetUsers");
        }
    }
}
