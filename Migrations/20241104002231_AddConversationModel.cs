using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConversationID",
                table: "Message",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ConversationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User1ID = table.Column<int>(type: "int", nullable: false),
                    User2ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ConversationID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_ConversationID",
                table: "Message",
                column: "ConversationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Conversations_ConversationID",
                table: "Message",
                column: "ConversationID",
                principalTable: "Conversations",
                principalColumn: "ConversationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Conversations_ConversationID",
                table: "Message");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Message_ConversationID",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ConversationID",
                table: "Message");
        }
    }
}
