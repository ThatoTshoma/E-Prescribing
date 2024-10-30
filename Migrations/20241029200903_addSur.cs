using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Prescribing.Migrations
{
    /// <inheritdoc />
    public partial class addSur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnaesthesiologistId",
                table: "IgnoreOderReasons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PharmacistId",
                table: "IgnoreOderReasons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IgnoreOderReasons_AnaesthesiologistId",
                table: "IgnoreOderReasons",
                column: "AnaesthesiologistId");

            migrationBuilder.CreateIndex(
                name: "IX_IgnoreOderReasons_PharmacistId",
                table: "IgnoreOderReasons",
                column: "PharmacistId");

            migrationBuilder.AddForeignKey(
                name: "FK_IgnoreOderReasons_Anaesthesiologists_AnaesthesiologistId",
                table: "IgnoreOderReasons",
                column: "AnaesthesiologistId",
                principalTable: "Anaesthesiologists",
                principalColumn: "AnaesthesiologistId");

            migrationBuilder.AddForeignKey(
                name: "FK_IgnoreOderReasons_Pharmacists_PharmacistId",
                table: "IgnoreOderReasons",
                column: "PharmacistId",
                principalTable: "Pharmacists",
                principalColumn: "PharmacistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IgnoreOderReasons_Anaesthesiologists_AnaesthesiologistId",
                table: "IgnoreOderReasons");

            migrationBuilder.DropForeignKey(
                name: "FK_IgnoreOderReasons_Pharmacists_PharmacistId",
                table: "IgnoreOderReasons");

            migrationBuilder.DropIndex(
                name: "IX_IgnoreOderReasons_AnaesthesiologistId",
                table: "IgnoreOderReasons");

            migrationBuilder.DropIndex(
                name: "IX_IgnoreOderReasons_PharmacistId",
                table: "IgnoreOderReasons");

            migrationBuilder.DropColumn(
                name: "AnaesthesiologistId",
                table: "IgnoreOderReasons");

            migrationBuilder.DropColumn(
                name: "PharmacistId",
                table: "IgnoreOderReasons");
        }
    }
}
