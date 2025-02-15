﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Prescribing.Migrations
{
    /// <inheritdoc />
    public partial class addOder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IgnoreOderReasons",
                columns: table => new
                {
                    IgnoreOderReasonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IgnoreOderReasons", x => x.IgnoreOderReasonId);
                    table.ForeignKey(
                        name: "FK_IgnoreOderReasons_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IgnoreOderReasons_OrderId",
                table: "IgnoreOderReasons",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IgnoreOderReasons");
        }
    }
}
