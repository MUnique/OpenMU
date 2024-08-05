using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FixGameMapMonsterSpawnRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""UPDATE config."MonsterSpawnArea" SET "GameMapId" = "GameMapDefinitionId" WHERE "GameMapId" is null""");
            migrationBuilder.DropForeignKey(
                name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea");

            migrationBuilder.DropForeignKey(
                name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapId",
                schema: "config",
                table: "MonsterSpawnArea");

            migrationBuilder.DropIndex(
                name: "IX_MonsterSpawnArea_GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea");

            migrationBuilder.DropColumn(
                name: "GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea");

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapId",
                schema: "config",
                table: "MonsterSpawnArea",
                column: "GameMapId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapId",
                schema: "config",
                table: "MonsterSpawnArea");

            migrationBuilder.AddColumn<Guid>(
                name: "GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpawnArea_GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea",
                column: "GameMapDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea",
                column: "GameMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapId",
                schema: "config",
                table: "MonsterSpawnArea",
                column: "GameMapId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id");

            migrationBuilder.Sql("""UPDATE config."MonsterSpawnArea" SET "GameMapDefinitionId" = "GameMapId" """);
        }
    }
}
