using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class earlyAccess3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EarlyAccess",
                columns: table => new
                {
                    ModId = table.Column<string>(maxLength: 127, nullable: false),
                    Steam64 = table.Column<string>(maxLength: 127, nullable: false),
                    Username = table.Column<string>(maxLength: 127, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarlyAccess", x => new { x.ModId, x.Steam64 });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EarlyAccess");
        }
    }
}
