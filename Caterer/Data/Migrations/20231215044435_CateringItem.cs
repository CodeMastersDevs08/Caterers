using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class CateringItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CateringItems",
                columns: table => new
                {
                    CateringItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenuCategoryId = table.Column<int>(type: "int", nullable: true),
                    SelectListItem = table.Column<int>(type: "int", nullable: false),
                    CateringCategoryId = table.Column<int>(type: "int", nullable: false),
                    ExtrasId = table.Column<int>(type: "int", nullable: true),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DineInPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VATPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ItemImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemAvailable = table.Column<bool>(type: "bit", nullable: false),
                    EnableVariants = table.Column<bool>(type: "bit", nullable: false),
                    EnableAlwaysAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Monday = table.Column<bool>(type: "bit", nullable: false),
                    MondayStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MondayEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tuesday = table.Column<bool>(type: "bit", nullable: false),
                    TuesdayStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TuesdayEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Wednesday = table.Column<bool>(type: "bit", nullable: false),
                    WednesdayStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WednesdayEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Thursday = table.Column<bool>(type: "bit", nullable: false),
                    ThursdayStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThursdayEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Friday = table.Column<bool>(type: "bit", nullable: false),
                    FridayStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FridayEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Saturday = table.Column<bool>(type: "bit", nullable: false),
                    SaturdayStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SaturdayEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sunday = table.Column<bool>(type: "bit", nullable: false),
                    SundayStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SundayEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CateringItems", x => x.CateringItemId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CateringItems");
        }
    }
}
