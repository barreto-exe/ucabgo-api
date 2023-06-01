using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class AddEvaluations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evaluation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Ride = table.Column<int>(type: "int(11)", nullable: false),
                    Evaluator = table.Column<int>(type: "int(11)", nullable: false),
                    Evaluated = table.Column<int>(type: "int(11)", nullable: false),
                    Stars = table.Column<int>(type: "int(11)", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evaluation");
        }
    }
}
