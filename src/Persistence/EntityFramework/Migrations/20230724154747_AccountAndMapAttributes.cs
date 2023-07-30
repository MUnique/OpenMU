using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AccountAndMapAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                schema: "data",
                table: "StatAttribute",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GameMapDefinitionId",
                schema: "config",
                table: "PowerUpDefinition",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatAttribute_AccountId",
                schema: "data",
                table: "StatAttribute",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinition_GameMapDefinitionId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "GameMapDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUpDefinition_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "GameMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatAttribute_Account_AccountId",
                schema: "data",
                table: "StatAttribute",
                column: "AccountId",
                principalSchema: "data",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PowerUpDefinition_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_StatAttribute_Account_AccountId",
                schema: "data",
                table: "StatAttribute");

            migrationBuilder.DropIndex(
                name: "IX_StatAttribute_AccountId",
                schema: "data",
                table: "StatAttribute");

            migrationBuilder.DropIndex(
                name: "IX_PowerUpDefinition_GameMapDefinitionId",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropColumn(
                name: "AccountId",
                schema: "data",
                table: "StatAttribute");

            migrationBuilder.DropColumn(
                name: "GameMapDefinitionId",
                schema: "config",
                table: "PowerUpDefinition");
        }
    }
}
