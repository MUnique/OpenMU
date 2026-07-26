using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class GoldenArcherDropGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DropItemGroup_GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup",
                column: "GameConfigurationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup",
                column: "GameConfigurationId1",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropIndex(
                name: "IX_DropItemGroup_GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropColumn(
                name: "GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup");
        }
    }
}
