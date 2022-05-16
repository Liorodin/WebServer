using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServer.Migrations
{
    public partial class intt10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_MessageList_MessageListId",
                table: "Message");

            migrationBuilder.AlterColumn<int>(
                name: "MessageListId",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_MessageList_MessageListId",
                table: "Message",
                column: "MessageListId",
                principalTable: "MessageList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_MessageList_MessageListId",
                table: "Message");

            migrationBuilder.AlterColumn<int>(
                name: "MessageListId",
                table: "Message",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_MessageList_MessageListId",
                table: "Message",
                column: "MessageListId",
                principalTable: "MessageList",
                principalColumn: "Id");
        }
    }
}
