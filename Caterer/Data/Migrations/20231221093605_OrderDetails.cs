using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class OrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "OrderDetailWebsites",
               columns: table => new
               {
                   OrderDetailsWebsiteId = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   UserId = table.Column<int>(type: "int", nullable: false),
                   RestaurantId = table.Column<int>(type: "int", nullable: false),
                   CateringCategoryId = table.Column<int>(type: "int", nullable: false),
                   CateringItemId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   ItemPrice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   NoOfPerson = table.Column<int>(type: "int", nullable: false),
                   Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   UserName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                   ExtraName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                   Subcategory = table.Column<string>(type: "nvarchar(255)", nullable: false),
                   ExtraPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                   ExtraId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   RestaurantName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                   Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                   SelfCollect = table.Column<int>(type: "int", nullable: false),
                   Delivery = table.Column<int>(type: "int", nullable: false),
                   Buffet = table.Column<int>(type: "int", nullable: false),
                   Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   UserAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   FoodCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                   Transport = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                   BuffetSetup = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                   Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                   GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                   Card = table.Column<bool>(type: "bit", nullable: false),
                   Cash = table.Column<bool>(type: "bit", nullable: false),
                   Role = table.Column<string>(type: "nvarchar(15)", nullable: true),
                   Password = table.Column<string>(type: "nvarchar(10)", nullable: true),
                   CardNumber = table.Column<string>(type: "nvarchar(12)", nullable: true),
                   CCV = table.Column<string>(type: "nvarchar(3)", nullable: true),
                   CreatedAt = table.Column<string>(type: "nvarchar(50)", nullable: false),
                   UpdatedAt = table.Column<string>(type: "nvarchar(50)", nullable: false),
                   DeletedAt = table.Column<DateOnly>(type: "date", nullable: false),
                   OrderTime = table.Column<TimeOnly>(type: "date", nullable: false),
                   SoftDelete = table.Column<string>(type: "nvarchar(50)", nullable: false),
                   DeliveryStatus = table.Column<string>(type: "nvarchar(50)", nullable: false),
                   PartialPay = table.Column<int>(type: "int", nullable: false),
                   FullPay = table.Column<int>(type: "int", nullable: false),
                   PartialAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                   Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                   ExtraCateringItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   ExtraItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   ExtraItemPrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   ExtraSubcategory = table.Column<string>(type: "nvarchar(255)", nullable: true),
                   AddExtra = table.Column<bool>(type: "bit", nullable: true),
                   ExtraCateringCategoryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   ExtraCateringCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   ExtraCateringSubCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   ExtraCateringItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   ExtraCateringNoOfPax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   ExtraCateringItemPrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   ExtrasCateringItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_OrderDetailsWebsites", x => x.OrderDetailsWebsiteId);
               });
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetailsWebsites");
        }
    }
}