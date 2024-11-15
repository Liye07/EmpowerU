using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentID",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_AppointmentID",
                table: "Payment",
                column: "AppointmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Appointment_AppointmentID",
                table: "Payment",
                column: "AppointmentID",
                principalTable: "Appointment",
                principalColumn: "AppointmentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Appointment_AppointmentID",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_AppointmentID",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "AppointmentID",
                table: "Payment");
        }
    }
}
