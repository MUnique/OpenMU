using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <summary>
    /// Adds the active flags to the maps, character classes, items, monsters and mini games.
    /// Existing entries stay active.
    /// </summary>
    public partial class AddIsActiveFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "config",
                table: "MonsterDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "config",
                table: "MiniGameDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "config",
                table: "ItemDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "config",
                table: "GameMapDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "config",
                table: "CharacterClass",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "GameMapDefinition");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "CharacterClass");
        }
    }
}
