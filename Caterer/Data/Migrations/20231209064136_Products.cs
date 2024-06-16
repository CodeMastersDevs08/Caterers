using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class Products : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
              name: "Products",
              columns: table => new
              {
                  RestaurantId = table.Column<int>(type: "int", nullable: false),
                  ProductId = table.Column<int>(type: "int", nullable: false)
                      .Annotation("SqlServer:Identity", "1, 1"),
                  ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                  WarehouseId = table.Column<int>(type: "int", nullable: false),
                  WarehouseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                  ProductType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                  TaxId = table.Column<int>(type: "int", nullable: true),
                  MeasurementId = table.Column<int>(type: "int", nullable: true),
                  CategoryId = table.Column<int>(type: "int", nullable: true),
                  productcode = table.Column<int>(type: "int", nullable: true),
                  Barcode = table.Column<string>(type: "nvarchar(100)", nullable: true),
                  ProductDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                  Quantity = table.Column<decimal>(type: "Decimal", nullable: true),
                  UnitPrice = table.Column<decimal>(type: "Decimal", nullable: true),
                  UnitCost = table.Column<decimal>(type: "Decimal", nullable: true),
                  StockControl = table.Column<int>(type: "int", nullable: true),
                  ExpireDate = table.Column<int>(type: "int", nullable: true),
                  Instock = table.Column<decimal>(type: "Decimal", nullable: true),
                  SafetyStock = table.Column<decimal>(type: "Decimal", nullable: true),
                  ProductImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  OpeningStock = table.Column<decimal>(type: "Decimal", nullable: true),
                  OpeningStockDate = table.Column<DateOnly>(type: "datetime2", nullable: true),
                  Mrp = table.Column<decimal>(type: "Decimal", nullable: true),
                  CreatedBy = table.Column<bool>(type: "bit", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Products", x => x.ProductId);
              });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
          name: "Products");
        }
    }
}