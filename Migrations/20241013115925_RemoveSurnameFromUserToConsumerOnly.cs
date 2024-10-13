using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSurnameFromUserToConsumerOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Surname",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Consumer",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Consumer");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "User",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "");
        }
    }
}
