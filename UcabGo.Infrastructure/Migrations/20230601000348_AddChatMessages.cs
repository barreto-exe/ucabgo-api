using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    public partial class AddChatMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chatmessage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Ride = table.Column<int>(type: "int(11)", nullable: false),
                    User = table.Column<int>(type: "int(11)", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
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
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateIndex(
                name: "ChatRide",
                table: "chatmessage",
                column: "Ride");

            migrationBuilder.CreateIndex(
                name: "ChatUser",
                table: "chatmessage",
                column: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chatmessage");
        }
    }
}
