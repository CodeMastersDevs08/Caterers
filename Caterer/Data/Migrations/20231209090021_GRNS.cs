using System;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Caterer.Data.Migrations
{
    public partial class GRNS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GRNS",
                columns: table => new
                {
                    GrnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GNo = table.Column<int>(type: "int", nullable: false),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    GRNNO = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    GRNDate = table.Column<DateOnly>(type: "datetime2", nullable: false),
                    GRNType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseOrderDate = table.Column<DateOnly>(type: "datetime2", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Paymentmethod = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    SupplierInvoiceNo = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    addonetime = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRN", x => x.GrnId);

                });
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GRNS");
        }
    }
}
