using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contact_ChatId",
                table: "Contact");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_ChatId",
                table: "Contact",
                column: "ChatId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contact_ChatId",
                table: "Contact");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_ChatId",
                table: "Contact",
                column: "ChatId");
        }
    }
}
