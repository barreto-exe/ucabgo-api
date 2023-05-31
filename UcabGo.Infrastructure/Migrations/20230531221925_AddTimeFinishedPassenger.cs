using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class AddTimeFinishedPassenger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "ride",
                type: "datetime",
                nullable: false,
                defaultValueSql: "'0001-01-01 00:00:00'",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeFinished",
                table: "passenger",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeFinished",
                table: "passenger");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "ride",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "'0001-01-01 00:00:00'");
        }
    }
}
