using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class GoldenArcherItemReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DropItemGroup_GoldenArcherConfiguration_GoldenArcherConfigu~",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_GameConfiguration_GoldenArcherConfiguration_GoldenArcherCon~",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_GameConfiguration_GoldenArcherConfigurationId",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_DropItemGroup_GoldenArcherConfigurationId",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropColumn(
                name: "GoldenArcherConfigurationId",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.AddColumn<double>(
                name: "ItemDropChance",
                schema: "config",
                table: "GoldenArcherConfiguration",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "GoldenArcherConfigurationItemDefinition",
                schema: "config",
                columns: table => new
                {
                    GoldenArcherConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoldenArcherConfigurationItemDefinition", x => new { x.GoldenArcherConfigurationId, x.ItemDefinitionId });
                    table.ForeignKey(
                        name: "FK_GoldenArcherConfigurationItemDefinition_GoldenArcherConfigu~",
                        column: x => x.GoldenArcherConfigurationId,
                        principalSchema: "config",
                        principalTable: "GoldenArcherConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoldenArcherConfigurationItemDefinition_ItemDefinition_Item~",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameConfiguration_GoldenArcherConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                column: "GoldenArcherConfigurationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoldenArcherConfigurationItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "GoldenArcherConfigurationItemDefinition",
                column: "ItemDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameConfiguration_GoldenArcherConfiguration_GoldenArcherCon~",
                schema: "config",
                table: "GameConfiguration",
                column: "GoldenArcherConfigurationId",
                principalSchema: "config",
                principalTable: "GoldenArcherConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameConfiguration_GoldenArcherConfiguration_GoldenArcherCon~",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropTable(
                name: "GoldenArcherConfigurationItemDefinition",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_GameConfiguration_GoldenArcherConfigurationId",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "ItemDropChance",
                schema: "config",
                table: "GoldenArcherConfiguration");

            migrationBuilder.AddColumn<Guid>(
                name: "GoldenArcherConfigurationId",
                schema: "config",
                table: "DropItemGroup",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameConfiguration_GoldenArcherConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                column: "GoldenArcherConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_DropItemGroup_GoldenArcherConfigurationId",
                schema: "config",
                table: "DropItemGroup",
                column: "GoldenArcherConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DropItemGroup_GoldenArcherConfiguration_GoldenArcherConfigu~",
                schema: "config",
                table: "DropItemGroup",
                column: "GoldenArcherConfigurationId",
                principalSchema: "config",
                principalTable: "GoldenArcherConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameConfiguration_GoldenArcherConfiguration_GoldenArcherCon~",
                schema: "config",
                table: "GameConfiguration",
                column: "GoldenArcherConfigurationId",
                principalSchema: "config",
                principalTable: "GoldenArcherConfiguration",
                principalColumn: "Id");
        }
    }
}
