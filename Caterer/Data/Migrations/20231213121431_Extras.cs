using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class Extras : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             

            migrationBuilder.CreateTable(
                name: "Extras",
                columns: table => new
                {
                    ExtrasId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuItemId = table.Column<int>(type: "int", nullable: false),
                    ExtraName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AddonCategory = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extras", x => x.ExtrasId);
                });

          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            
        }
    }
}
