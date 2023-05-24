using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class AddRideRelatedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "IsActive",
                table: "destinations",
                type: "bit(1)",
                nullable: true,
                defaultValueSql: "b'0'",
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User = table.Column<int>(type: "int(11)", nullable: false),
                    Alias = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    Zone = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    Detail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    Latitude = table.Column<float>(type: "float", nullable: false),
                    Longitude = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.Id);
                    table.ForeignKey(
                        name: "locations_ibfk_1",
                        column: x => x.User,
                        principalTable: "user",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "ride",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Driver = table.Column<int>(type: "int(11)", nullable: false),
                    Vehicle = table.Column<int>(type: "int(11)", nullable: false),
                    Destination = table.Column<int>(type: "int(11)", nullable: false),
                    SeatQuantity = table.Column<int>(type: "int(11)", nullable: false),
                    LatitudeOrigin = table.Column<float>(type: "float", nullable: false),
                    LongitudeOrigin = table.Column<float>(type: "float", nullable: false),
                    IsAvailable = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ride", x => x.Id);
                    table.ForeignKey(
                        name: "ride_ibfk_1",
                        column: x => x.Driver,
                        principalTable: "user",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "ride_ibfk_2",
                        column: x => x.Vehicle,
                        principalTable: "vehicle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "ride_ibfk_3",
                        column: x => x.Destination,
                        principalTable: "destinations",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "passenger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Ride = table.Column<int>(type: "int(11)", nullable: false),
                    InitialLocation = table.Column<int>(type: "int(11)", nullable: false),
                    Passenger = table.Column<int>(type: "int(11)", nullable: false),
                    IsAccepted = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'0'"),
                    TimeSolicited = table.Column<DateTime>(type: "datetime", nullable: false),
                    TimeAccepted = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passenger", x => x.Id);
                    table.ForeignKey(
                        name: "passenger_ibfk_1",
                        column: x => x.Ride,
                        principalTable: "ride",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "passenger_ibfk_2",
                        column: x => x.Passenger,
                        principalTable: "user",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "passenger_ibfk_3",
                        column: x => x.InitialLocation,
                        principalTable: "locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateIndex(
                name: "User1",
                table: "locations",
                column: "User");

            migrationBuilder.CreateIndex(
                name: "InitialLocation",
                table: "passenger",
                column: "InitialLocation");

            migrationBuilder.CreateIndex(
                name: "Passenger",
                table: "passenger",
                column: "Passenger");

            migrationBuilder.CreateIndex(
                name: "Ride",
                table: "passenger",
                column: "Ride");

            migrationBuilder.CreateIndex(
                name: "Destination",
                table: "ride",
                column: "Destination");

            migrationBuilder.CreateIndex(
                name: "Driver",
                table: "ride",
                column: "Driver");

            migrationBuilder.CreateIndex(
                name: "Vehicle",
                table: "ride",
                column: "Vehicle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "passenger");

            migrationBuilder.DropTable(
                name: "ride");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsActive",
                table: "destinations",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bit(1)",
                oldNullable: true,
                oldDefaultValueSql: "b'0'");
        }
    }
}
