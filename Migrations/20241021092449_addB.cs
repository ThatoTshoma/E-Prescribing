using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Prescribing.Migrations
{
    /// <inheritdoc />
    public partial class addB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionDtae",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DischergeDate",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NurseId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_NurseId",
                table: "Bookings",
                column: "NurseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Nurses_NurseId",
                table: "Bookings",
                column: "NurseId",
                principalTable: "Nurses",
                principalColumn: "NurseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Nurses_NurseId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_NurseId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AdmissionDtae",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DischergeDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "NurseId",
                table: "Bookings");
        }
    }
}
