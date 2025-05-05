using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraceService.Migrations
{
    /// <inheritdoc />
    public partial class TraceUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Ms",
                table: "Traces",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Traces",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "Traces",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ms",
                table: "Traces");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Traces");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "Traces");
        }
    }
}
