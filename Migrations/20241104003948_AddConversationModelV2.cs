using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationModelV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Conversations_ConversationID",
                table: "Message");

            migrationBuilder.AlterColumn<int>(
                name: "ConversationID",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Conversations_ConversationID",
                table: "Message",
                column: "ConversationID",
                principalTable: "Conversations",
                principalColumn: "ConversationID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Conversations_ConversationID",
                table: "Message");

            migrationBuilder.AlterColumn<int>(
                name: "ConversationID",
                table: "Message",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Conversations_ConversationID",
                table: "Message",
                column: "ConversationID",
                principalTable: "Conversations",
                principalColumn: "ConversationID");
        }
    }
}
