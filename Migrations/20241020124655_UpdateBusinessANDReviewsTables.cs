using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBusinessANDReviewsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Business_BusinessID",
                table: "Review");

            migrationBuilder.RenameColumn(
                name: "BusinessID",
                table: "Review",
                newName: "BusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_BusinessID",
                table: "Review",
                newName: "IX_Review_BusinessId");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessId",
                table: "Review",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Business_BusinessId",
                table: "Review",
                column: "BusinessId",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Business_BusinessID",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Business_BusinessId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_BusinessID",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "BusinessID",
                table: "Review");

            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "Review",
                newName: "BusinessID");

            migrationBuilder.RenameIndex(
                name: "IX_Review_BusinessId",
                table: "Review",
                newName: "IX_Review_BusinessID");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessID",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Business_BusinessID",
                table: "Review",
                column: "BusinessID",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
