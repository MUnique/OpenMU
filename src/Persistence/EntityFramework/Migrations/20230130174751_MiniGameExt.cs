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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
