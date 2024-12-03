using System;
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
            migrationBuilder.DropColumn(
                name: "CharacterId",
                schema: "data",
                table: "LetterHeader");

            migrationBuilder.AddColumn<byte>(
                name: "MinimumSuccessPercent",
                schema: "config",
                table: "SimpleCraftingSettings",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "ResultItemRateDependentOptions",
                schema: "config",
                table: "SimpleCraftingSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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
                name: "MinimumSuccessPercent",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.DropColumn(
                name: "ResultItemRateDependentOptions",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.DropColumn(
                name: "SuccessPercentageAdditionForGuardianItem",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.AddColumn<Guid>(
                name: "CharacterId",
                schema: "data",
                table: "LetterHeader",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
