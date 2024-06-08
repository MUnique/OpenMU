using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FixItemOptions2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropIndex(
                name: "IX_IncreasableItemOption_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.DropColumn(
                name: "ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption");

            migrationBuilder.AddColumn<Guid>(
                name: "OptionsId",
                schema: "config",
                table: "ItemSetGroup",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemSetGroup_OptionsId",
                schema: "config",
                table: "ItemSetGroup",
                column: "OptionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSetGroup_ItemOptionDefinition_OptionsId",
                schema: "config",
                table: "ItemSetGroup",
                column: "OptionsId",
                principalSchema: "config",
                principalTable: "ItemOptionDefinition",
                principalColumn: "Id");

            // Now we are left with ItemSetGroups which have no options assigned.
            // Luckily, we can identify them by their UUIDs, they share the same pattern.
            // First we care about ancient sets, which have the same id as the option definition,
            // except in their prefix.
            migrationBuilder.Sql(
                """
                UPDATE config."ItemSetGroup" s
                  SET "OptionsId" = UUID(REPLACE(TEXT(s."Id"), '00000092-','00000083-'))
                WHERE EXISTS(SELECT "Id" FROM config."ItemOptionDefinition" WHERE "Id" = UUID(REPLACE(TEXT(s."Id"), '00000092-','00000083-')) )
                """);

            // Then we look at the non-ancient sets.
            migrationBuilder.Sql(
                """
                UPDATE config."ItemSetGroup" s SET
                  "AlwaysApplies" = true,
                  "OptionsId" = 
                    CASE
                      WHEN "SetLevel"=0 THEN uuid('00000083-0021-0000-0000-000000000000')
                      ELSE uuid('00000083-0020-000' || to_hex("SetLevel") || '-0000-000000000000')
                    END
                WHERE "OptionsId" is null
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemSetGroup_ItemOptionDefinition_OptionsId",
                schema: "config",
                table: "ItemSetGroup");

            migrationBuilder.DropIndex(
                name: "IX_ItemSetGroup_OptionsId",
                schema: "config",
                table: "ItemSetGroup");

            migrationBuilder.DropColumn(
                name: "OptionsId",
                schema: "config",
                table: "ItemSetGroup");

            migrationBuilder.AddColumn<Guid>(
                name: "ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncreasableItemOption_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemSetGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemSetGroupId",
                principalSchema: "config",
                principalTable: "ItemSetGroup",
                principalColumn: "Id");
        }
    }
}
