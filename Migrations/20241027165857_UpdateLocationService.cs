using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerU.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLocationService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         /*   migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Consumer_ConsumerID",
                table: "Appointment");

            migrationBuilder.RenameColumn(
                name: "ConsumerID",
                table: "Appointment",
                newName: "ConsumerId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointment_ConsumerID",
                table: "Appointment",
                newName: "IX_Appointment_ConsumerId");*/

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "LocationService",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "LocationService",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "LocationService",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Suburb",
                table: "LocationService",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "ConsumerId",
                table: "Appointment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
/*
            migrationBuilder.AddColumn<int>(
                name: "ConsumerID",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);*/
/*
            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ConsumerID",
                table: "Appointment",
                column: "ConsumerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Consumer_ConsumerID",
                table: "Appointment",
                column: "ConsumerID",
                principalTable: "Consumer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Consumer_ConsumerId",
                table: "Appointment",
                column: "ConsumerId",
                principalTable: "Consumer",
                principalColumn: "Id");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Consumer_ConsumerID",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Consumer_ConsumerId",
                table: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_ConsumerID",
                table: "Appointment");
*/
            migrationBuilder.DropColumn(
                name: "City",
                table: "LocationService");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "LocationService");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "LocationService");

            migrationBuilder.DropColumn(
                name: "Suburb",
                table: "LocationService");

            migrationBuilder.DropColumn(
                name: "ConsumerID",
                table: "Appointment");

            migrationBuilder.RenameColumn(
                name: "ConsumerId",
                table: "Appointment",
                newName: "ConsumerID");

            /*migrationBuilder.RenameIndex(
                name: "IX_Appointment_ConsumerId",
                table: "Appointment",
                newName: "IX_Appointment_ConsumerID");*/

            migrationBuilder.AlterColumn<int>(
                name: "ConsumerID",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

          /*  migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Consumer_ConsumerID",
                table: "Appointment",
                column: "ConsumerID",
                principalTable: "Consumer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);*/
        }
    }
}
