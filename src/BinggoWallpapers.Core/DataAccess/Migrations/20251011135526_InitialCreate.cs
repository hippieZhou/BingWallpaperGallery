using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinggoWallpapers.Core.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gallery");

            migrationBuilder.CreateTable(
                name: "wallpapers",
                schema: "gallery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    ActualDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    MarketCode = table.Column<string>(type: "TEXT", nullable: false),
                    ResolutionCode = table.Column<string>(type: "TEXT", nullable: false),
                    InfoJson = table.Column<string>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallpapers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wallpapers_ActualDate",
                schema: "gallery",
                table: "wallpapers",
                column: "ActualDate");

            migrationBuilder.CreateIndex(
                name: "IX_Wallpapers_MarketCode",
                schema: "gallery",
                table: "wallpapers",
                column: "MarketCode");

            migrationBuilder.CreateIndex(
                name: "IX_Wallpapers_MarketCode_ResolutionCode_Hash",
                schema: "gallery",
                table: "wallpapers",
                columns: new[] { "MarketCode", "ResolutionCode", "Hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallpapers_ResolutionCode",
                schema: "gallery",
                table: "wallpapers",
                column: "ResolutionCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wallpapers",
                schema: "gallery");
        }
    }
}
