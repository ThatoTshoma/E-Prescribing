using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Prescribing.Migrations
{
    /// <inheritdoc />
    public partial class addchan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnoreReason",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "AdmissionDate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DischargeDate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IgnoreReason",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IgnoreReason",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionDate",
                table: "Patients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DischargeDate",
                table: "Patients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IgnoreReason",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
