using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentEmpowerUModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Payment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Payment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Payment");
        }
    }
}
