using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Caterer.Data.Migrations
{
    public partial class PurchaseOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PNo = table.Column<int>(type: "int", nullable: false),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderDate = table.Column<DateOnly>(type: "datetime2", nullable: true),
                    ExpectedDate = table.Column<DateOnly>(type: "datetime2", nullable: true),
                    Paymentmethod = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    DeliveryInstruction = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Measurement = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    InStock = table.Column<decimal>(type: "decimal", nullable: false),
                    Incoming = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrder", x => x.PurchaseOrderId);

                });
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrders");
        }
    }
}