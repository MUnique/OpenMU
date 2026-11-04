using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterSkillPassivePowerUps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MasterSkillDefinitionId",
                schema: "config",
                table: "PowerUpDefinition",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinition_MasterSkillDefinitionId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "MasterSkillDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUpDefinition_MasterSkillDefinition_MasterSkillDefiniti~",
                schema: "config",
                table: "PowerUpDefinition",
                column: "MasterSkillDefinitionId",
                principalSchema: "config",
                principalTable: "MasterSkillDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PowerUpDefinition_MasterSkillDefinition_MasterSkillDefiniti~",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropIndex(
                name: "IX_PowerUpDefinition_MasterSkillDefinitionId",
                schema: "config",
                table: "PowerUpDefinition");

            migrationBuilder.DropColumn(
                name: "MasterSkillDefinitionId",
                schema: "config",
                table: "PowerUpDefinition");
        }
    }
}
