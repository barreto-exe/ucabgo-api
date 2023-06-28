using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class MergeLocationAndDestination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ride_ibfk_3",
                table: "ride");

            migrationBuilder.DropTable(
                name: "destinations");

            migrationBuilder.RenameColumn(
                name: "Destination",
                table: "ride",
                newName: "FinalLocation");

            migrationBuilder.RenameColumn(
                name: "InitialLocation",
                table: "passenger",
                newName: "FinalLocation");

            migrationBuilder.AddColumn<float>(
                name: "LatitudeOrigin",
                table: "passenger",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "LongitudeOrigin",
                table: "passenger",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "evaluation",
                type: "varchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValueSql: "''",
                collation: "latin1_swedish_ci",
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldMaxLength: 5)
                .Annotation("MySql:CharSet", "latin1")
                .OldAnnotation("MySql:CharSet", "latin1")
                .OldAnnotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.AddForeignKey(
                name: "ride_ibfk_3",
                table: "ride",
                column: "FinalLocation",
                principalTable: "locations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ride_ibfk_3",
                table: "ride");

            migrationBuilder.DropColumn(
                name: "LatitudeOrigin",
                table: "passenger");

            migrationBuilder.DropColumn(
                name: "LongitudeOrigin",
                table: "passenger");

            migrationBuilder.RenameColumn(
                name: "FinalLocation",
                table: "ride",
                newName: "Destination");

            migrationBuilder.RenameColumn(
                name: "FinalLocation",
                table: "passenger",
                newName: "InitialLocation");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "evaluation",
                type: "varchar(5)",
                maxLength: 5,
                nullable: false,
                collation: "latin1_swedish_ci",
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldMaxLength: 5,
                oldDefaultValueSql: "''")
                .Annotation("MySql:CharSet", "latin1")
                .OldAnnotation("MySql:CharSet", "latin1")
                .OldAnnotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "destinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int(11)", nullable: false, comment: " ")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User = table.Column<int>(type: "int(11)", nullable: false),
                    Alias = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    Detail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    IsDeleted = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    Latitude = table.Column<float>(type: "float", nullable: false),
                    Longitude = table.Column<float>(type: "float", nullable: false),
                    Zone = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_destinations", x => x.Id);
                    table.ForeignKey(
                        name: "destinations_ibfk_1",
                        column: x => x.User,
                        principalTable: "user",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateIndex(
                name: "User",
                table: "destinations",
                column: "User");

            migrationBuilder.AddForeignKey(
                name: "ride_ibfk_3",
                table: "ride",
                column: "Destination",
                principalTable: "destinations",
                principalColumn: "Id");
        }
    }
}
