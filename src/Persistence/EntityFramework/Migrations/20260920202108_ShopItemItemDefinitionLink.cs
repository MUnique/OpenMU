using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ShopItemItemDefinitionLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The ItemDefinitionId column was already added by an earlier
            // attempt at this migration (timestamp 20260920201640) that
            // was rolled back at the code level but kept the column at
            // the DB level (a drop+re-add would lose the FK + index).
            //
            // AddColumn is intentionally skipped — column already exists.
            // The Up() here only creates the unique partial index + FK.
            //
            // AddColumn (commented out, kept for documentation):
            //
            // migrationBuilder.AddColumn<Guid>(
            //     name: "ItemDefinitionId",
            //     schema: "data",
            //     table: "ShopItem",
            //     type: "uuid",
            //     nullable: true);

            // FK to config."ItemDefinition". SET NULL on delete: if the
            // wrapped item definition is ever removed from OpenMU's
            // config, the ShopItem row survives with ItemDefinitionId=NULL
            // so the catalog entry becomes a "pure" product (no sprite,
            // no in-game counterpart). Same pattern as the manual FK we
            // added for CrimsonCoinLedger in PR 2.
            //
            // Partial unique index — only enforced when ItemDefinitionId
            // is set, so multiple NULL-valued rows (pure shop products
            // without an in-game counterpart) coexist. Used as the upsert
            // key by `scripts/seed-shop.ts`.
            migrationBuilder.CreateIndex(
                name: "IX_ShopItem_ItemDefinitionId",
                schema: "data",
                table: "ShopItem",
                column: "ItemDefinitionId",
                unique: true,
                filter: "\"ItemDefinitionId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopItem_ItemDefinition_ItemDefinitionId",
                schema: "data",
                table: "ShopItem",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopItem_ItemDefinition_ItemDefinitionId",
                schema: "data",
                table: "ShopItem");

            // DropIndex doesn't take a filter arg — drop by SQL.
            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS data.\"IX_ShopItem_ItemDefinitionId\";");

            // DropColumn removed — the column was added by the prior
            // attempt at this migration and is intentionally left in
            // place here. To drop the column entirely, run a hand-written
            // DROP COLUMN outside this migration.
        }
    }
}
