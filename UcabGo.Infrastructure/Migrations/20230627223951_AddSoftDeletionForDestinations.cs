using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class AddSoftDeletionForDestinations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "IsDeleted",
                table: "locations",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");

            migrationBuilder.AddColumn<ulong>(
                name: "IsDeleted",
                table: "destinations",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "destinations");
        }
    }
}
