using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class RemoveUserColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondLastName",
                table: "user");

            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "user");

            migrationBuilder.AlterDatabase(
                oldCollation: "SQL_Latin1_General_CP1_CI_AS");

            migrationBuilder.AlterColumn<double>(
                name: "WalkingDistance",
                table: "user",
                type: "float",
                nullable: true,
                defaultValueSql: "('0')",
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldDefaultValueSql: "'0'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "ride",
                type: "datetime",
                nullable: false,
                defaultValueSql: "('0001-01-01 00:00:00')",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "'0001-01-01 00:00:00'");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "ride",
                type: "bit",
                nullable: false,
                defaultValueSql: "('0')",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "'0'");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHome",
                table: "locations",
                type: "bit",
                nullable: false,
                defaultValueSql: "('0')",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "'0'");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "locations",
                type: "bit",
                nullable: false,
                defaultValueSql: "('0')",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "'0'");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "evaluation",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValueSql: "('')",
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5,
                oldDefaultValueSql: "''");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase(
                collation: "SQL_Latin1_General_CP1_CI_AS");

            migrationBuilder.AlterColumn<double>(
                name: "WalkingDistance",
                table: "user",
                type: "float",
                nullable: true,
                defaultValueSql: "'0'",
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldDefaultValueSql: "('0')");

            migrationBuilder.AddColumn<string>(
                name: "SecondLastName",
                table: "user",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "user",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeCreated",
                table: "ride",
                type: "datetime",
                nullable: false,
                defaultValueSql: "'0001-01-01 00:00:00'",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "('0001-01-01 00:00:00')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "ride",
                type: "bit",
                nullable: false,
                defaultValueSql: "'0'",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "('0')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHome",
                table: "locations",
                type: "bit",
                nullable: false,
                defaultValueSql: "'0'",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "('0')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "locations",
                type: "bit",
                nullable: false,
                defaultValueSql: "'0'",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "('0')");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "evaluation",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5,
                oldDefaultValueSql: "('')");
        }
    }
}
