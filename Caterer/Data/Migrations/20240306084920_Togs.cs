using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class Togs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.CreateTable(
                name: "Togs",
                columns: table => new
                {
                    TogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    TNo = table.Column<int>(type: "int", nullable: false),
                    TogNO = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    TogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GRNType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToWarehouseId = table.Column<int>(type: "int", nullable: false),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal", nullable: false),
                    Total = table.Column<decimal>(type: "decimal", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Togs", x => x.TogId);
                    table.ForeignKey(
                        name: "FK_Togs_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Cascade);
                });

          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropTable(
                name: "Togs");

          
        }
    }
}
