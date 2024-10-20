using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class AddServicesToBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_Business_BusinessID",
                table: "Service");

            migrationBuilder.RenameColumn(
                name: "BusinessID",
                table: "Service",
                newName: "BusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_BusinessID",
                table: "Service",
                newName: "IX_Service_BusinessId");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessId",
                table: "Service",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Remove the add column logic
            // Uncomment the following line if you need to add the foreign key relationship.
            migrationBuilder.AddForeignKey(
                name: "FK_Service_Business_BusinessId",
                table: "Service",
                column: "BusinessId",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_Business_BusinessID",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_Business_BusinessId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_BusinessID",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "BusinessID",
                table: "Service");

            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "Service",
                newName: "BusinessID");

            migrationBuilder.RenameIndex(
                name: "IX_Service_BusinessId",
                table: "Service",
                newName: "IX_Service_BusinessID");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessID",
                table: "Service",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Business_BusinessID",
                table: "Service",
                column: "BusinessID",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
