using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSimpleCraftingSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NpcPriceDivisor",
                schema: "config",
                table: "SimpleCraftingSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SuccessPercentageAdditionForGuardianItem",
                schema: "config",
                table: "SimpleCraftingSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NpcPriceDivisor",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.DropColumn(
                name: "SuccessPercentageAdditionForGuardianItem",
                schema: "config",
                table: "SimpleCraftingSettings");
        }
    }
}
