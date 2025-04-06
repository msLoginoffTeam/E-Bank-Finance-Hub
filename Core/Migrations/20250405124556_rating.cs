using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Core_Api.Migrations
{
    /// <inheritdoc />
    public partial class rating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Operations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Clients",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropColumn(
                name: "Type",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Clients");
        }
    }
}
