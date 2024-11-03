using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "Appointment");

            migrationBuilder.AddColumn<int>(
                name: "ServiceID",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ServiceID",
                table: "Appointment",
                column: "ServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Service_ServiceID",
                table: "Appointment",
                column: "ServiceID",
                principalTable: "Service",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Service_ServiceID",
                table: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_ServiceID",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "ServiceID",
                table: "Appointment");

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "Appointment",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "");
        }
    }
}
