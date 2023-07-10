using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SecondLastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    WalkingDistance = table.Column<double>(type: "float", nullable: true, defaultValueSql: "'0'"),
                    ProfilePicture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User = table.Column<int>(type: "int", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    IsHome = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "'0'"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.Id);
                    table.ForeignKey(
                        name: "locations_ibfk_1",
                        column: x => x.User,
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "soscontact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_soscontact", x => x.Id);
                    table.ForeignKey(
                        name: "soscontact_ibfk_1",
                        column: x => x.User,
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "vehicle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Plate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle", x => x.Id);
                    table.ForeignKey(
                        name: "vehicle_ibfk_1",
                        column: x => x.User,
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ride",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Driver = table.Column<int>(type: "int", nullable: false),
                    Vehicle = table.Column<int>(type: "int", nullable: false),
                    FinalLocation = table.Column<int>(type: "int", nullable: false),
                    LatitudeOrigin = table.Column<float>(type: "real", nullable: false),
                    LongitudeOrigin = table.Column<float>(type: "real", nullable: false),
                    SeatQuantity = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "'0'"),
                    TimeCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'0001-01-01 00:00:00'"),
                    TimeEnded = table.Column<DateTime>(type: "datetime", nullable: true),
                    TimeStarted = table.Column<DateTime>(type: "datetime", nullable: true),
                    TimeCanceled = table.Column<DateTime>(type: "datetime", nullable: true)
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
                        column: x => x.FinalLocation,
                        principalTable: "locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "chatmessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ride = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TimeSent = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatmessage", x => x.Id);
                    table.ForeignKey(
                        name: "chatmessage_ibfk_1",
                        column: x => x.Ride,
                        principalTable: "ride",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "chatmessage_ibfk_2",
                        column: x => x.User,
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "evaluation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ride = table.Column<int>(type: "int", nullable: false),
                    Evaluator = table.Column<int>(type: "int", nullable: false),
                    Evaluated = table.Column<int>(type: "int", nullable: false),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValueSql: "''")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evaluation", x => x.Id);
                    table.ForeignKey(
                        name: "evaluation_ibfk_1",
                        column: x => x.Ride,
                        principalTable: "ride",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "evaluation_ibfk_2",
                        column: x => x.Evaluator,
                        principalTable: "user",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "evaluation_ibfk_3",
                        column: x => x.Evaluated,
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "passenger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ride = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<int>(type: "int", nullable: false),
                    FinalLocation = table.Column<int>(type: "int", nullable: false),
                    LatitudeOrigin = table.Column<float>(type: "real", nullable: false),
                    LongitudeOrigin = table.Column<float>(type: "real", nullable: false),
                    TimeSolicited = table.Column<DateTime>(type: "datetime", nullable: false),
                    TimeAccepted = table.Column<DateTime>(type: "datetime", nullable: true),
                    TimeIgnored = table.Column<DateTime>(type: "datetime", nullable: true),
                    TimeCancelled = table.Column<DateTime>(type: "datetime", nullable: true),
                    TimeFinished = table.Column<DateTime>(type: "datetime", nullable: true)
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
                        column: x => x.User,
                        principalTable: "user",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "passenger_ibfk_3",
                        column: x => x.FinalLocation,
                        principalTable: "locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ChatRide",
                table: "chatmessage",
                column: "Ride");

            migrationBuilder.CreateIndex(
                name: "ChatUser",
                table: "chatmessage",
                column: "User");

            migrationBuilder.CreateIndex(
                name: "Evaluated",
                table: "evaluation",
                column: "Evaluated");

            migrationBuilder.CreateIndex(
                name: "EvaluatedRide",
                table: "evaluation",
                column: "Ride");

            migrationBuilder.CreateIndex(
                name: "Evaluator",
                table: "evaluation",
                column: "Evaluator");

            migrationBuilder.CreateIndex(
                name: "User1",
                table: "locations",
                column: "User");

            migrationBuilder.CreateIndex(
                name: "InitialLocation",
                table: "passenger",
                column: "FinalLocation");

            migrationBuilder.CreateIndex(
                name: "Passenger",
                table: "passenger",
                column: "User");

            migrationBuilder.CreateIndex(
                name: "Ride",
                table: "passenger",
                column: "Ride");

            migrationBuilder.CreateIndex(
                name: "Destination",
                table: "ride",
                column: "FinalLocation");

            migrationBuilder.CreateIndex(
                name: "Driver",
                table: "ride",
                column: "Driver");

            migrationBuilder.CreateIndex(
                name: "Vehicle",
                table: "ride",
                column: "Vehicle");

            migrationBuilder.CreateIndex(
                name: "IdUser",
                table: "soscontact",
                column: "User");

            migrationBuilder.CreateIndex(
                name: "IdUser1",
                table: "vehicle",
                column: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chatmessage");

            migrationBuilder.DropTable(
                name: "evaluation");

            migrationBuilder.DropTable(
                name: "passenger");

            migrationBuilder.DropTable(
                name: "soscontact");

            migrationBuilder.DropTable(
                name: "ride");

            migrationBuilder.DropTable(
                name: "vehicle");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
