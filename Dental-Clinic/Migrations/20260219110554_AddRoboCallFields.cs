using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dental_Clinic.Migrations
{
    /// <inheritdoc />
    public partial class AddRoboCallFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CallStatus",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CallTriggeredAt",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoboCallId",
                table: "Appointments",
                type: "int",
                nullable: true);

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "CallStatus",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CallTriggeredAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "RoboCallId",
                table: "Appointments");
        }
    }
}
