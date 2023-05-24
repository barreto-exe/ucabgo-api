using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class AddTimeCanceledToRide : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeCanceled",
                table: "ride",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeCanceled",
                table: "ride");
        }
    }
}
