using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    public partial class Combo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ComboDefinitionId",
                schema: "config",
                table: "CharacterClass",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SkillComboDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MaximumCompletionTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillComboDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillComboStep",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillComboDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsFinalStep = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillComboStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillComboStep_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "config",
                        principalTable: "Skill",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SkillComboStep_SkillComboDefinition_SkillComboDefinitionId",
                        column: x => x.SkillComboDefinitionId,
                        principalSchema: "config",
                        principalTable: "SkillComboDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterClass_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass",
                column: "ComboDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillComboStep_SkillComboDefinitionId",
                schema: "config",
                table: "SkillComboStep",
                column: "SkillComboDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillComboStep_SkillId",
                schema: "config",
                table: "SkillComboStep",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterClass_SkillComboDefinition_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass",
                column: "ComboDefinitionId",
                principalSchema: "config",
                principalTable: "SkillComboDefinition",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterClass_SkillComboDefinition_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass");

            migrationBuilder.DropTable(
                name: "SkillComboStep",
                schema: "config");

            migrationBuilder.DropTable(
                name: "SkillComboDefinition",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_CharacterClass_ComboDefinitionId",
                schema: "config",
                table: "CharacterClass");

            migrationBuilder.DropColumn(
                name: "ComboDefinitionId",
                schema: "config",
                table: "CharacterClass");
        }
    }
}
