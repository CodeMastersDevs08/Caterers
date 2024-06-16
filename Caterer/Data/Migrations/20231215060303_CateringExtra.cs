using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class CateringExtra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CateringExtras",
                columns: table => new
                {
                    CateringExtrasId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CateringItemId = table.Column<int>(type: "int", nullable: false),
                    ExtraName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AddonCategory = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CateringExtras", x => x.CateringExtrasId);
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropTable(
                name: "CateringExtras");

           
        }
    }
}
