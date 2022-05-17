using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServer.Migrations
{
    public partial class intt13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_ToUsername",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_ToUsername",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ToUsername",
                table: "Message");

            migrationBuilder.AddColumn<string>(
                name: "To",
                table: "Message",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "To",
                table: "Message");

            migrationBuilder.AddColumn<string>(
                name: "ToUsername",
                table: "Message",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ToUsername",
                table: "Message",
                column: "ToUsername");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_ToUsername",
                table: "Message",
                column: "ToUsername",
                principalTable: "User",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
