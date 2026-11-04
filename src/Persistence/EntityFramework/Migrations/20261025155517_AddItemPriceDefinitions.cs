using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddItemPriceDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PriceDefinitionId",
                schema: "config",
                table: "ItemDefinition",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemPriceDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    BasePriceFormula = table.Column<string>(type: "text", nullable: true),
                    QuantityScaling = table.Column<int>(type: "integer", nullable: false),
                    Modifiers = table.Column<int>(type: "integer", nullable: false),
                    SellingPriceRounding = table.Column<int>(type: "integer", nullable: false),
                    CraftingReferencePrice = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPriceDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemPriceDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemLevelPrice",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemPriceDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLevelPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemLevelPrice_ItemPriceDefinition_ItemPriceDefinitionId",
                        column: x => x.ItemPriceDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemPriceDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_PriceDefinitionId",
                schema: "config",
                table: "ItemDefinition",
                column: "PriceDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLevelPrice_ItemPriceDefinitionId",
                schema: "config",
                table: "ItemLevelPrice",
                column: "ItemPriceDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPriceDefinition_GameConfigurationId",
                schema: "config",
                table: "ItemPriceDefinition",
                column: "GameConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDefinition_ItemPriceDefinition_PriceDefinitionId",
                schema: "config",
                table: "ItemDefinition",
                column: "PriceDefinitionId",
                principalSchema: "config",
                principalTable: "ItemPriceDefinition",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemDefinition_ItemPriceDefinition_PriceDefinitionId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropTable(
                name: "ItemLevelPrice",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemPriceDefinition",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_ItemDefinition_PriceDefinitionId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "PriceDefinitionId",
                schema: "config",
                table: "ItemDefinition");
        }
    }
}
