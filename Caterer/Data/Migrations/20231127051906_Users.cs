using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class Users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "Users",
               columns: table => new
               {
                   RestaurantId = table.Column<int>(type: "int", nullable: false),
                   UserID = table.Column<int>(nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   UserName = table.Column<string>(nullable: false),
                   RestaurantName = table.Column<string>(nullable: true),
                   Email = table.Column<string>(nullable: false),
                   PhoneNumber = table.Column<string>(nullable: false),
                   Password = table.Column<string>(nullable: false),
                   Role = table.Column<string>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Users", x => x.UserID);
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "Users");
        }
    }
}