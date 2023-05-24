using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class AddWalkingDistance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "WalkingDistance",
                table: "user",
                type: "double",
                nullable: true,
                defaultValueSql: "'0'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WalkingDistance",
                table: "user");
        }
    }
}
