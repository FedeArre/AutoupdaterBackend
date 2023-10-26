using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace DataAccess.Migrations
{
    public partial class database : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Mods",
                columns: table => new
                {
                    ModId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    ModAuthor = table.Column<string>(nullable: true),
                    CPC = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mods", x => x.ModId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false),
                    TokenAPI = table.Column<string>(nullable: true),
                    DiscordVerificationToken = table.Column<string>(nullable: true),
                    DiscordId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "ModVersion",
                columns: table => new
                {
                    ModId = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    RequiredGameBuildId = table.Column<int>(nullable: false),
                    DownloadLink = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "EarlyAccess",
                columns: table => new
                {
                    GroupName = table.Column<string>(nullable: false),
                    OwnerUsername = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarlyAccess", x => x.GroupName);
                    table.ForeignKey(
                        name: "FK_EarlyAccess_Users_OwnerUsername",
                        column: x => x.OwnerUsername,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EarlyAccessTesters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    eagGroupName = table.Column<string>(nullable: true),
                    Steam64 = table.Column<string>(maxLength: 50, nullable: true),
                    Username = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarlyAccessTesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EarlyAccessTesters_EarlyAccess_eagGroupName",
                        column: x => x.eagGroupName,
                        principalTable: "EarlyAccess",
                        principalColumn: "GroupName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModAllowed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ModIdentifierString = table.Column<string>(nullable: true),
                    ModId = table.Column<string>(nullable: true),
                    GroupName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModAllowed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModAllowed_EarlyAccess_GroupName",
                        column: x => x.GroupName,
                        principalTable: "EarlyAccess",
                        principalColumn: "GroupName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModAllowed_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "ModId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EarlyAccess_OwnerUsername",
                table: "EarlyAccess",
                column: "OwnerUsername");

            migrationBuilder.CreateIndex(
                name: "IX_EarlyAccessTesters_eagGroupName",
                table: "EarlyAccessTesters",
                column: "eagGroupName");

            migrationBuilder.CreateIndex(
                name: "IX_ModAllowed_GroupName",
                table: "ModAllowed",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_ModAllowed_ModId",
                table: "ModAllowed",
                column: "ModId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EarlyAccessTesters");

            migrationBuilder.DropTable(
                name: "ModAllowed");

            migrationBuilder.DropTable(
                name: "ModDailyData");

            migrationBuilder.DropTable(
                name: "ModVersion");

            migrationBuilder.DropTable(
                name: "EarlyAccess");

            migrationBuilder.DropTable(
                name: "Mods");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
