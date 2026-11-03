using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddMiniGameFreeTicketItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FreeTicketItemId",
                schema: "config",
                table: "MiniGameDefinition",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameDefinition_FreeTicketItemId",
                schema: "config",
                table: "MiniGameDefinition",
                column: "FreeTicketItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameDefinition_ItemDefinition_FreeTicketItemId",
                schema: "config",
                table: "MiniGameDefinition",
                column: "FreeTicketItemId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameDefinition_ItemDefinition_FreeTicketItemId",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropIndex(
                name: "IX_MiniGameDefinition_FreeTicketItemId",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropColumn(
                name: "FreeTicketItemId",
                schema: "config",
                table: "MiniGameDefinition");
        }
    }
}
