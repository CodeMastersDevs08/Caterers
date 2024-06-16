﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caterer.Data.Migrations
{
    public partial class Categories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "Categories",
               columns: table => new
               {
                   RestaurantId = table.Column<int>(type: "int", nullable: false),
                   CategoryId = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                      CategoryLogo = table.Column<string>(type: "nvarchar(max)", nullable: false)


               },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Category", x => x.CategoryId);
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                  name: "Categories");
        }
    }
}