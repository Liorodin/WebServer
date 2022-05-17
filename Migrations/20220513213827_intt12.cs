using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServer.Migrations
{
    public partial class intt12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_FromUsername",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "FromUsername",
                table: "Message",
                newName: "ToUsername");

            migrationBuilder.RenameIndex(
                name: "IX_Message_FromUsername",
                table: "Message",
                newName: "IX_Message_ToUsername");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_ToUsername",
                table: "Message",
                column: "ToUsername",
                principalTable: "User",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_ToUsername",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "ToUsername",
                table: "Message",
                newName: "FromUsername");

            migrationBuilder.RenameIndex(
                name: "IX_Message_ToUsername",
                table: "Message",
                newName: "IX_Message_FromUsername");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_FromUsername",
                table: "Message",
                column: "FromUsername",
                principalTable: "User",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
