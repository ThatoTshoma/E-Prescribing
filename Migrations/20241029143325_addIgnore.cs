using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Prescribing.Migrations
{
    /// <inheritdoc />
    public partial class addIgnore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IgnorePrescriptionsReasons",
                columns: table => new
                {
                    IgnorePrescriptionReasonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrescriptionId = table.Column<int>(type: "int", nullable: false),
                    PharmacistId = table.Column<int>(type: "int", nullable: true),
                    SurgeonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IgnorePrescriptionsReasons", x => x.IgnorePrescriptionReasonId);
                    table.ForeignKey(
                        name: "FK_IgnorePrescriptionsReasons_Pharmacists_PharmacistId",
                        column: x => x.PharmacistId,
                        principalTable: "Pharmacists",
                        principalColumn: "PharmacistId");
                    table.ForeignKey(
                        name: "FK_IgnorePrescriptionsReasons_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "PrescriptionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IgnorePrescriptionsReasons_Surgeons_SurgeonId",
                        column: x => x.SurgeonId,
                        principalTable: "Surgeons",
                        principalColumn: "SurgeonId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IgnorePrescriptionsReasons_PharmacistId",
                table: "IgnorePrescriptionsReasons",
                column: "PharmacistId");

            migrationBuilder.CreateIndex(
                name: "IX_IgnorePrescriptionsReasons_PrescriptionId",
                table: "IgnorePrescriptionsReasons",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_IgnorePrescriptionsReasons_SurgeonId",
                table: "IgnorePrescriptionsReasons",
                column: "SurgeonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IgnorePrescriptionsReasons");
        }
    }
}
