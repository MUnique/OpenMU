using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestMonsterKillRequirementState_CharacterQuestState_Charac~",
                schema: "data",
                table: "QuestMonsterKillRequirementState");

            migrationBuilder.CreateTable(
                name: "SystemConfiguration",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IpResolver = table.Column<int>(type: "integer", nullable: false),
                    IpResolverParameter = table.Column<string>(type: "text", nullable: true),
                    AutoStart = table.Column<bool>(type: "boolean", nullable: false),
                    AutoUpdateSchema = table.Column<bool>(type: "boolean", nullable: false),
                    ReadConsoleInput = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfiguration", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_QuestMonsterKillRequirementState_CharacterQuestState_Charac~",
                schema: "data",
                table: "QuestMonsterKillRequirementState",
                column: "CharacterQuestStateId",
                principalSchema: "data",
                principalTable: "CharacterQuestState",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestMonsterKillRequirementState_CharacterQuestState_Charac~",
                schema: "data",
                table: "QuestMonsterKillRequirementState");

            migrationBuilder.DropTable(
                name: "SystemConfiguration",
                schema: "config");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestMonsterKillRequirementState_CharacterQuestState_Charac~",
                schema: "data",
                table: "QuestMonsterKillRequirementState",
                column: "CharacterQuestStateId",
                principalSchema: "data",
                principalTable: "CharacterQuestState",
                principalColumn: "Id");
        }
    }
}
