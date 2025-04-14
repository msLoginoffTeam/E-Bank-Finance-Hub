using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User_Api.Migrations
{
    /// <inheritdoc />
    public partial class DeviceToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceToken",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                column: "DeviceToken",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6e9e5d77-d218-49aa-80a9-3a1f0dba62db"),
                column: "DeviceToken",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceToken",
                table: "Users");
        }
    }
}
