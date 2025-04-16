using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

<<<<<<<< HEAD:Core/Migrations/20250401125521_init.cs
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

========
>>>>>>>> d9f3d17 (AuthService):Core/Migrations/20250322095008_init.cs
namespace Core_Api.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyCourses",
                columns: table => new
                {
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    Course = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyCourses", x => x.Currency);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
<<<<<<<< HEAD:Core/Migrations/20250401125521_init.cs
========
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
>>>>>>>> d9f3d17 (AuthService):Core/Migrations/20250322095008_init.cs
                    Name = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    Balance = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OperationCategory = table.Column<int>(type: "integer", nullable: false),
                    OperationType = table.Column<int>(type: "integer", nullable: true),
                    TargetAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConvertedAmount = table.Column<int>(type: "integer", nullable: true),
                    SenderAccountId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_Accounts_SenderAccountId",
                        column: x => x.SenderAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Operations_Accounts_TargetAccountId",
                        column: x => x.TargetAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                column: "Id",
                values: new object[]
                {
                    new Guid("00000000-0000-0000-0000-000000000000"),
                    new Guid("6e9e5d77-d218-49aa-80a9-3a1f0dba62db")
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Balance", "ClientId", "Currency", "IsClosed", "Name", "Number" },
                values: new object[,]
                {
                    { new Guid("6b1f2247-f43d-44d7-be26-4c458055b7cd"), 10000000, new Guid("00000000-0000-0000-0000-000000000000"), 2, false, "Банковский Евровый", null },
                    { new Guid("cedf298f-65cd-496e-a1ef-75de39dc0891"), 10000000, new Guid("00000000-0000-0000-0000-000000000000"), 1, false, "Банковский Долларовый", null },
                    { new Guid("ffa09d4d-ba75-4382-84fd-453cdf7323ce"), 1000000000, new Guid("00000000-0000-0000-0000-000000000000"), 0, false, "Банковский Рублевый", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ClientId",
                table: "Accounts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_SenderAccountId",
                table: "Operations",
                column: "SenderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_TargetAccountId",
                table: "Operations",
                column: "TargetAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyCourses");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
