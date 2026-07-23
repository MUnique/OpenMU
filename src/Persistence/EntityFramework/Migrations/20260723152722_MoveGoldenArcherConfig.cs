using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class MoveGoldenArcherConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropColumn(
                name: "GoldenArcherRenaRewardZen",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "GoldenArcherRequiredRenas",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.RenameColumn(
                name: "GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup",
                newName: "GoldenArcherConfigurationId");

            migrationBuilder.RenameIndex(
                name: "IX_DropItemGroup_GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup",
                newName: "IX_DropItemGroup_GoldenArcherConfigurationId");

            migrationBuilder.AddColumn<Guid>(
                name: "GoldenArcherConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalRegisteredRenas",
                schema: "data",
                table: "Character",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GoldenArcherConfiguration",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequiredRenas = table.Column<int>(type: "integer", nullable: false),
                    RewardZen = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoldenArcherConfiguration", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameConfiguration_GoldenArcherConfigurationId",
                schema: "config",
                table: "GameConfiguration",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DropItemGroup_GoldenArcherConfiguration_GoldenArcherConfigu~",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_GameConfiguration_GoldenArcherConfiguration_GoldenArcherCon~",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropTable(
                name: "GoldenArcherConfiguration",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_GameConfiguration_GoldenArcherConfigurationId",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "GoldenArcherConfigurationId",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "TotalRegisteredRenas",
                schema: "data",
                table: "Character");

            migrationBuilder.RenameColumn(
                name: "GoldenArcherConfigurationId",
                schema: "config",
                table: "DropItemGroup",
                newName: "GameConfigurationId1");

            migrationBuilder.RenameIndex(
                name: "IX_DropItemGroup_GoldenArcherConfigurationId",
                schema: "config",
                table: "DropItemGroup",
                newName: "IX_DropItemGroup_GameConfigurationId1");

            migrationBuilder.AddColumn<int>(
                name: "GoldenArcherRenaRewardZen",
                schema: "config",
                table: "GameConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoldenArcherRequiredRenas",
                schema: "config",
                table: "GameConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId1",
                schema: "config",
                table: "DropItemGroup",
                column: "GameConfigurationId1",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id");
        }
    }
}
