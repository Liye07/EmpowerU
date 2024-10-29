using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "User");

            // Re-add ProfilePicture column with varbinary(max) type
            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "User",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

    }
}
