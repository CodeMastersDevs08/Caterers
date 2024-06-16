using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class Extraid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExtrasId1",
                table: "MenuItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_ExtrasId1",
                table: "MenuItems",
                column: "ExtrasId1");

            migrationBuilder.CreateIndex(
                name: "IX_Extras_MenuItemId",
                table: "Extras",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Extras_MenuItems_MenuItemId",
                table: "Extras",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "MenuItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_Extras_ExtrasId1",
                table: "MenuItems",
                column: "ExtrasId1",
                principalTable: "Extras",
                principalColumn: "ExtrasId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Extras_MenuItems_MenuItemId",
                table: "Extras");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_Extras_ExtrasId1",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_ExtrasId1",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_Extras_MenuItemId",
                table: "Extras");

            migrationBuilder.DropColumn(
                name: "ExtrasId1",
                table: "MenuItems");
        }
    }
}
