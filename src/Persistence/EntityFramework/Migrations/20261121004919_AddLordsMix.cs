using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddLordsMix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ResultItemExcellentOptionCountTableId",
                schema: "config",
                table: "SimpleCraftingSettings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResultItemSocketCountTableId",
                schema: "config",
                table: "SimpleCraftingSettings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ExcellentOptionChance",
                schema: "config",
                table: "ItemCraftingResultItem",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LordsMixCount",
                schema: "data",
                table: "CastleSiegeData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LordsMixCountDate",
                schema: "data",
                table: "CastleSiegeData",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LordsMixLimit",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LordsMixLimitPeriod",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SimpleCraftingSettings_ResultItemExcellentOptionCountTableId",
                schema: "config",
                table: "SimpleCraftingSettings",
                column: "ResultItemExcellentOptionCountTableId");

            migrationBuilder.CreateIndex(
                name: "IX_SimpleCraftingSettings_ResultItemSocketCountTableId",
                schema: "config",
                table: "SimpleCraftingSettings",
                column: "ResultItemSocketCountTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimpleCraftingSettings_WeightedValueTable_ResultItemExcelle~",
                schema: "config",
                table: "SimpleCraftingSettings",
                column: "ResultItemExcellentOptionCountTableId",
                principalSchema: "config",
                principalTable: "WeightedValueTable",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SimpleCraftingSettings_WeightedValueTable_ResultItemSocketC~",
                schema: "config",
                table: "SimpleCraftingSettings",
                column: "ResultItemSocketCountTableId",
                principalSchema: "config",
                principalTable: "WeightedValueTable",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimpleCraftingSettings_WeightedValueTable_ResultItemExcelle~",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_SimpleCraftingSettings_WeightedValueTable_ResultItemSocketC~",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.DropIndex(
                name: "IX_SimpleCraftingSettings_ResultItemExcellentOptionCountTableId",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.DropIndex(
                name: "IX_SimpleCraftingSettings_ResultItemSocketCountTableId",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.DropColumn(
                name: "ResultItemExcellentOptionCountTableId",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.DropColumn(
                name: "ResultItemSocketCountTableId",
                schema: "config",
                table: "SimpleCraftingSettings");

            migrationBuilder.DropColumn(
                name: "ExcellentOptionChance",
                schema: "config",
                table: "ItemCraftingResultItem");

            migrationBuilder.DropColumn(
                name: "LordsMixCount",
                schema: "data",
                table: "CastleSiegeData");

            migrationBuilder.DropColumn(
                name: "LordsMixCountDate",
                schema: "data",
                table: "CastleSiegeData");

            migrationBuilder.DropColumn(
                name: "LordsMixLimit",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropColumn(
                name: "LordsMixLimitPeriod",
                schema: "config",
                table: "CastleSiegeConfiguration");
        }
    }
}
