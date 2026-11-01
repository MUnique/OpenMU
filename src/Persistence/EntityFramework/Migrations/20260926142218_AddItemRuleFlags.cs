using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <summary>
    /// Adds the item rule flags. Existing items allow everything; the
    /// AddItemRuleFlagsPlugIn update sets the Season 6 values.
    /// </summary>
    public partial class AddItemRuleFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDroppable",
                schema: "config",
                table: "ItemDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPersonalStoreSellable",
                schema: "config",
                table: "ItemDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRepairable",
                schema: "config",
                table: "ItemDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSellableToNpc",
                schema: "config",
                table: "ItemDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsStorable",
                schema: "config",
                table: "ItemDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTradable",
                schema: "config",
                table: "ItemDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDroppable",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "IsPersonalStoreSellable",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "IsRepairable",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "IsSellableToNpc",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "IsStorable",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "IsTradable",
                schema: "config",
                table: "ItemDefinition");
        }
    }
}
