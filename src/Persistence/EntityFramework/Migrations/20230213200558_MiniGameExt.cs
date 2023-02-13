using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class MiniGameExt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClientUpdateRequired",
                schema: "config",
                table: "MiniGameTerrainChange",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowParty",
                schema: "config",
                table: "MiniGameDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ArePlayerKillersAllowedToEnter",
                schema: "config",
                table: "MiniGameDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EntranceFee",
                schema: "config",
                table: "MiniGameDefinition",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "MiniGameDefinition",
                column: "AllowParty",
                schema: "config",
                keyColumn: "AllowParty",
                value: true,
                keyValue: false);

            migrationBuilder.UpdateData(
                table: "MiniGameTerrainChange",
                column: "IsClientUpdateRequired",
                schema: "config",
                keyColumn: "IsClientUpdateRequired",
                value: true,
                keyValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClientUpdateRequired",
                schema: "config",
                table: "MiniGameTerrainChange");

            migrationBuilder.DropColumn(
                name: "AllowParty",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropColumn(
                name: "ArePlayerKillersAllowedToEnter",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropColumn(
                name: "EntranceFee",
                schema: "config",
                table: "MiniGameDefinition");
        }
    }
}
