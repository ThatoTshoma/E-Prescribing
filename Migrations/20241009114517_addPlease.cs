using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Prescribing.Migrations
{
    /// <inheritdoc />
    public partial class addPlease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IgnoreReason",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IgnoreReason",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnoreReason",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "IgnoreReason",
                table: "Orders");

         
        }
    }
}
