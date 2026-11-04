using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddItemStorePriceCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StorePriceCoinType",
                schema: "data",
                table: "Item",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StorePriceItemDefinitionId",
                schema: "data",
                table: "Item",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_StorePriceItemDefinitionId",
                schema: "data",
                table: "Item",
                column: "StorePriceItemDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_ItemDefinition_StorePriceItemDefinitionId",
                schema: "data",
                table: "Item",
                column: "StorePriceItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_ItemDefinition_StorePriceItemDefinitionId",
                schema: "data",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_StorePriceItemDefinitionId",
                schema: "data",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "StorePriceCoinType",
                schema: "data",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "StorePriceItemDefinitionId",
                schema: "data",
                table: "Item");
        }
    }
}
