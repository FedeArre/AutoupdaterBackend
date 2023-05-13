using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class userDiscordStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisableAutoupdating",
                table: "Mods");

            migrationBuilder.AddColumn<string>(
                name: "DiscordId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscordVerificationToken",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModVersion",
                columns: table => new
                {
                    ModId = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    RequiredGameBuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModVersion", x => x.ModId);
                    table.ForeignKey(
                        name: "FK_ModVersion_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "ModId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModVersion");

            migrationBuilder.DropColumn(
                name: "DiscordId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DiscordVerificationToken",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "DisableAutoupdating",
                table: "Mods",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
