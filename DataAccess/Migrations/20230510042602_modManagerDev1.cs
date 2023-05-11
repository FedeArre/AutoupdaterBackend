using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class modManagerDev1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EarlyAccess");

            migrationBuilder.CreateTable(
                name: "ModDailyData",
                columns: table => new
                {
                    ModId = table.Column<string>(nullable: false),
                    PlayerCount = table.Column<int>(nullable: false),
                    DataDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModDailyData", x => x.ModId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModDailyData");

            migrationBuilder.CreateTable(
                name: "EarlyAccess",
                columns: table => new
                {
                    ModId = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: false),
                    Steam64 = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: false),
                    Username = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarlyAccess", x => new { x.ModId, x.Steam64 });
                });
        }
    }
}
