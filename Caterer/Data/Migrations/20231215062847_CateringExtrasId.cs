using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class CateringExtrasId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExtrasId",
                table: "CateringItems",
                newName: "CateringExtrasId1");

            migrationBuilder.AddColumn<int>(
                name: "CateringExtrasId",
                table: "CateringItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CateringItems_CateringExtrasId1",
                table: "CateringItems",
                column: "CateringExtrasId1");

            migrationBuilder.CreateIndex(
                name: "IX_CateringExtras_CateringItemId",
                table: "CateringExtras",
                column: "CateringItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CateringExtras_CateringItems_CateringItemId",
                table: "CateringExtras",
                column: "CateringItemId",
                principalTable: "CateringItems",
                principalColumn: "CateringItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CateringItems_CateringExtras_CateringExtrasId1",
                table: "CateringItems",
                column: "CateringExtrasId1",
                principalTable: "CateringExtras",
                principalColumn: "CateringExtrasId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CateringExtras_CateringItems_CateringItemId",
                table: "CateringExtras");

            migrationBuilder.DropForeignKey(
                name: "FK_CateringItems_CateringExtras_CateringExtrasId1",
                table: "CateringItems");

            migrationBuilder.DropIndex(
                name: "IX_CateringItems_CateringExtrasId1",
                table: "CateringItems");

            migrationBuilder.DropIndex(
                name: "IX_CateringExtras_CateringItemId",
                table: "CateringExtras");

            migrationBuilder.DropColumn(
                name: "CateringExtrasId",
                table: "CateringItems");

            migrationBuilder.RenameColumn(
                name: "CateringExtrasId1",
                table: "CateringItems",
                newName: "ExtrasId");
        }
    }
}
