using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class Taxes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                 name: "Taxes",
                 columns: table => new
                 {
                     TaxId = table.Column<int>(type: "int", nullable: false)
                         .Annotation("SqlServer:Identity", "1, 1"),
                     RestaurantId = table.Column<int>(type: "int", nullable: false),
                     TaxNo = table.Column<int>(type: "int", nullable: false),
                     TaxName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                     TaxType = table.Column<string>(type: "nvarchar(100)", nullable: true),
                     TaxPercentage = table.Column<decimal>(type: "decimal", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Tax", x => x.TaxId);
                 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
               
        }
    }
}
