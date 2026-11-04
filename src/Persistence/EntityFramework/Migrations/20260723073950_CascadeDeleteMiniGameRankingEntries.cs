// <copyright file="20260723073950_CascadeDeleteMiniGameRankingEntries.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class CascadeDeleteMiniGameRankingEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameRankingEntry_Character_CharacterId",
                schema: "data",
                table: "MiniGameRankingEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameRankingEntry_MiniGameDefinition_MiniGameId",
                schema: "data",
                table: "MiniGameRankingEntry");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameRankingEntry_Character_CharacterId",
                schema: "data",
                table: "MiniGameRankingEntry",
                column: "CharacterId",
                principalSchema: "data",
                principalTable: "Character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameRankingEntry_MiniGameDefinition_MiniGameId",
                schema: "data",
                table: "MiniGameRankingEntry",
                column: "MiniGameId",
                principalSchema: "config",
                principalTable: "MiniGameDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameRankingEntry_Character_CharacterId",
                schema: "data",
                table: "MiniGameRankingEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniGameRankingEntry_MiniGameDefinition_MiniGameId",
                schema: "data",
                table: "MiniGameRankingEntry");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameRankingEntry_Character_CharacterId",
                schema: "data",
                table: "MiniGameRankingEntry",
                column: "CharacterId",
                principalSchema: "data",
                principalTable: "Character",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniGameRankingEntry_MiniGameDefinition_MiniGameId",
                schema: "data",
                table: "MiniGameRankingEntry",
                column: "MiniGameId",
                principalSchema: "config",
                principalTable: "MiniGameDefinition",
                principalColumn: "Id");
        }
    }
}
