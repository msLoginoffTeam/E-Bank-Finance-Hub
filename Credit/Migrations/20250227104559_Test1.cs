using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditService_Patterns.Migrations
{
    /// <inheritdoc />
    public partial class Test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanName = table.Column<string>(type: "text", nullable: false),
                    PlanPercent = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Credit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemainingAmount = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credit_Plan_CreditPlanId",
                        column: x => x.CreditPlanId,
                        principalTable: "Plan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientCreditId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentAmount = table.Column<float>(type: "real", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Credit_ClientCreditId",
                        column: x => x.ClientCreditId,
                        principalTable: "Credit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Credit_CreditPlanId",
                table: "Credit",
                column: "CreditPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ClientCreditId",
                table: "Payment",
                column: "ClientCreditId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Credit");

            migrationBuilder.DropTable(
                name: "Plan");
        }
    }
}
