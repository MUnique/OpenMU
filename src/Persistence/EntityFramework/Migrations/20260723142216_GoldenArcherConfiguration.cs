using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class GoldenArcherConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoldenArcherRenaRewardZen",
                schema: "config",
                table: "GameConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldenArcherRequiredRenas",
                schema: "config",
                table: "GameConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegisteredRenas",
                schema: "data",
                table: "Character",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoldenArcherRenaRewardZen",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "GoldenArcherRequiredRenas",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "RegisteredRenas",
                schema: "data",
                table: "Character");
        }
    }
}
