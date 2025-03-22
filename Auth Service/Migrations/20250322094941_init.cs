using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth_Service.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAuths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientAuths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAuths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientAuths_UserAuths_Id",
                        column: x => x.Id,
                        principalTable: "UserAuths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAuths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAuths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAuths_UserAuths_Id",
                        column: x => x.Id,
                        principalTable: "UserAuths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserAuths",
                columns: new[] { "Id", "Password" },
                values: new object[] { new Guid("4e9e5d77-d218-49aa-80a9-3a1f0dba62db"), "240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9" });

            migrationBuilder.InsertData(
                table: "EmployeeAuths",
                columns: new[] { "Id", "RefreshToken" },
                values: new object[] { new Guid("4e9e5d77-d218-49aa-80a9-3a1f0dba62db"), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientAuths");

            migrationBuilder.DropTable(
                name: "EmployeeAuths");

            migrationBuilder.DropTable(
                name: "UserAuths");
        }
    }
}
