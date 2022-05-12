using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServer.Migrations
{
    public partial class init14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contactsusername",
                table: "User",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    username = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.username);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Contactsusername",
                table: "User",
                column: "Contactsusername");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Contacts_Contactsusername",
                table: "User",
                column: "Contactsusername",
                principalTable: "Contacts",
                principalColumn: "username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Contacts_Contactsusername",
                table: "User");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_User_Contactsusername",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Contactsusername",
                table: "User");
        }
    }
}
