using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class unsupportedMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ModUnsupported",
                table: "Mods",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EAModObjects",
                columns: table => new
                {
                    ModId = table.Column<string>(nullable: false),
                    DownloadLink = table.Column<string>(nullable: true),
                    CurrentKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EAModObjects", x => x.ModId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EAModObjects");

            migrationBuilder.DropColumn(
                name: "ModUnsupported",
                table: "Mods");
        }
    }
}
