using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class AddTimeIgnoredPassanger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "passenger");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeIgnored",
                table: "passenger",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeIgnored",
                table: "passenger");

            migrationBuilder.AddColumn<ulong>(
                name: "IsAccepted",
                table: "passenger",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");
        }
    }
}
