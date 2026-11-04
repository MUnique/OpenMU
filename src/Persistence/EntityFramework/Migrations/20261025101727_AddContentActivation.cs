using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <summary>
    /// Adds the active flags and the game version which introduced them to the maps, character classes,
    /// items, monsters and mini games. Existing entries stay active, and their version is unknown until
    /// a configuration update sets it.
    /// </summary>
    public partial class AddContentActivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntroducedIn",
                schema: "config",
                table: "MonsterDefinition",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "config",
                table: "MonsterDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "IntroducedIn",
                schema: "config",
                table: "MiniGameDefinition",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "config",
                table: "MiniGameDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "IntroducedIn",
                schema: "config",
                table: "ItemDefinition",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "config",
                table: "ItemDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "IntroducedIn",
                schema: "config",
                table: "GameMapDefinition",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "config",
                table: "GameMapDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "IntroducedIn",
                schema: "config",
                table: "CharacterClass",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
                name: "IntroducedIn",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropColumn(
                name: "IntroducedIn",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropColumn(
                name: "IntroducedIn",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "IntroducedIn",
                schema: "config",
                table: "GameMapDefinition");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "GameMapDefinition");

            migrationBuilder.DropColumn(
                name: "IntroducedIn",
                schema: "config",
                table: "CharacterClass");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "config",
                table: "CharacterClass");
        }
    }
}
