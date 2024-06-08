using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FixItemOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.AddForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemSetGroupId",
                principalSchema: "config",
                principalTable: "ItemSetGroup",
                principalColumn: "Id");

            // First, we need to create the new item option definitions for the ancient sets
            migrationBuilder.Sql(
                """
                INSERT INTO config."ItemOptionDefinition" ("Id", "GameConfigurationId", "Name", "AddsRandomly", "AddChance", "MaximumOptionsPerItem")
                SELECT UUID(REPLACE(sets.setId, '00000092-','00000083-')), '00000001-0001-0000-0000-000000000000', sets.setName || ' (Ancient Set)', false, 0, 0
                FROM
                (
                    SELECT COUNT(o."Id") c, text(o."ItemSetGroupId") setId, g."Name" setName
                    FROM config."IncreasableItemOption" o, config."ItemSetGroup" g
                    WHERE "ItemSetGroupId" is not null
                      AND "ItemOptionDefinitionId" is null
                      AND g."Id" = o."ItemSetGroupId"
                    GROUP BY o."ItemSetGroupId", g."Name"
                    ORDER BY c
                ) sets
                WHERE sets.c > 1
                """);

            // Then we need to update the item option definitions of the ancient sets,
            // so that they belong to the previously created item option defintions
            migrationBuilder.Sql(
                """
                UPDATE config."IncreasableItemOption" o
                SET "ItemOptionDefinitionId" = UUID(REPLACE(TEXT(o."ItemSetGroupId"), '00000092-','00000083-'))
                    WHERE o."ItemSetGroupId" is not null
                      AND o."ItemOptionDefinitionId" is null
                      AND o."OptionTypeId" is not null
                """);

            // Then what's left are the options for set completion of normal sets.
            migrationBuilder.Sql(
                """
                INSERT INTO config."ItemOptionDefinition" ("Id", "GameConfigurationId", "Name", "AddsRandomly", "AddChance", "MaximumOptionsPerItem")
                SELECT UUID(REPLACE(sets.optionId, '00000088-','00000083-')),
                        '00000001-0001-0000-0000-000000000000',
                        CASE WHEN sets.setLevel=0 THEN 'Complete Set Bonus (any level)'
                            ELSE 'Complete Set Bonus (Level ' || sets.setLevel || ')'
                        END,
                        false, 0, 0
                FROM
                (
                    SELECT text(o."Id") optionId, text(o."ItemSetGroupId") setId, g."Name" setName, g."SetLevel" setLevel
                    FROM config."IncreasableItemOption" o, config."ItemSetGroup" g
                    WHERE "ItemSetGroupId" is not null
                      AND "ItemOptionDefinitionId" is null
                      AND g."Id" = o."ItemSetGroupId"
                    ORDER BY setLevel
                ) sets
                """);

            // Then we need to update the item option definitions of the normal sets
            // so that they belong to the previously created item option defintions
            migrationBuilder.Sql(
                """
                UPDATE config."IncreasableItemOption" o
                SET "ItemOptionDefinitionId" = UUID(REPLACE(TEXT(o."Id"), '00000088-','00000083-'))
                    WHERE o."ItemOptionDefinitionId" is null
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.AddForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemSetGroupId",
                principalSchema: "config",
                principalTable: "ItemSetGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
