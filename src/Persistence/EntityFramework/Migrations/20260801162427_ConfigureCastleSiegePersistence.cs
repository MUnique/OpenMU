// <copyright file="20260801162427_ConfigureCastleSiegePersistence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class ConfigureCastleSiegePersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeConfiguration_GameMapDefinition_CastleSiegeMapDe~",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeConfiguration_GameMapDefinition_LandOfTrialsMapD~",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeConfiguration_ItemDefinition_RewardItemDefinitio~",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeNpcDefinition_MonsterDefinition_MonsterDefinitio~",
                schema: "config",
                table: "CastleSiegeNpcDefinition");

            migrationBuilder.DropIndex(
                name: "IX_CastleSiegeNpcDefinition_MonsterDefinitionId",
                schema: "config",
                table: "CastleSiegeNpcDefinition");

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM config."CastleSiegeNpcDefinition" WHERE "MonsterDefinitionId" IS NULL) THEN
                        RAISE EXCEPTION 'CastleSiegeNpcDefinition contains rows without a MonsterDefinitionId. Repair or remove these rows before applying this migration.';
                    END IF;
                END
                $$;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "MonsterDefinitionId",
                schema: "config",
                table: "CastleSiegeNpcDefinition",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RegisterMinMembers",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 20,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "RegisterMinLevel",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 200,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "MaxAttackingGuilds",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CrownHoldTimeSeconds",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 30,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "CastleSiegeGuildRegistration",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildName = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Marks = table.Column<int>(type: "integer", nullable: false),
                    RegistrationOrder = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeGuildRegistration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastleSiegeGuildRegistration_Guild_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "guild",
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeNpcState_MonsterNumber_InstanceId",
                schema: "data",
                table: "CastleSiegeNpcState",
                columns: new[] { "MonsterNumber", "InstanceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeNpcDefinition_MonsterDefinitionId_InstanceId",
                schema: "config",
                table: "CastleSiegeNpcDefinition",
                columns: new[] { "MonsterDefinitionId", "InstanceId" });

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeData_OwnerGuildId",
                schema: "data",
                table: "CastleSiegeData",
                column: "OwnerGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeGuildRegistration_GuildId",
                schema: "data",
                table: "CastleSiegeGuildRegistration",
                column: "GuildId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeConfiguration_GameMapDefinition_CastleSiegeMapDe~",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "CastleSiegeMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeConfiguration_GameMapDefinition_LandOfTrialsMapD~",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "LandOfTrialsMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeConfiguration_ItemDefinition_RewardItemDefinitio~",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "RewardItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeData_Guild_OwnerGuildId",
                schema: "data",
                table: "CastleSiegeData",
                column: "OwnerGuildId",
                principalSchema: "guild",
                principalTable: "Guild",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeNpcDefinition_MonsterDefinition_MonsterDefinitio~",
                schema: "config",
                table: "CastleSiegeNpcDefinition",
                column: "MonsterDefinitionId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeConfiguration_GameMapDefinition_CastleSiegeMapDe~",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeConfiguration_GameMapDefinition_LandOfTrialsMapD~",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeConfiguration_ItemDefinition_RewardItemDefinitio~",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeData_Guild_OwnerGuildId",
                schema: "data",
                table: "CastleSiegeData");

            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeNpcDefinition_MonsterDefinition_MonsterDefinitio~",
                schema: "config",
                table: "CastleSiegeNpcDefinition");

            migrationBuilder.DropTable(
                name: "CastleSiegeGuildRegistration",
                schema: "data");

            migrationBuilder.DropIndex(
                name: "IX_CastleSiegeNpcState_MonsterNumber_InstanceId",
                schema: "data",
                table: "CastleSiegeNpcState");

            migrationBuilder.DropIndex(
                name: "IX_CastleSiegeNpcDefinition_MonsterDefinitionId_InstanceId",
                schema: "config",
                table: "CastleSiegeNpcDefinition");

            migrationBuilder.DropIndex(
                name: "IX_CastleSiegeData_OwnerGuildId",
                schema: "data",
                table: "CastleSiegeData");

            migrationBuilder.AlterColumn<Guid>(
                name: "MonsterDefinitionId",
                schema: "config",
                table: "CastleSiegeNpcDefinition",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "RegisterMinMembers",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 20);

            migrationBuilder.AlterColumn<int>(
                name: "RegisterMinLevel",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 200);

            migrationBuilder.AlterColumn<int>(
                name: "MaxAttackingGuilds",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 3);

            migrationBuilder.AlterColumn<int>(
                name: "CrownHoldTimeSeconds",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 30);

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeNpcDefinition_MonsterDefinitionId",
                schema: "config",
                table: "CastleSiegeNpcDefinition",
                column: "MonsterDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeConfiguration_GameMapDefinition_CastleSiegeMapDe~",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "CastleSiegeMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeConfiguration_GameMapDefinition_LandOfTrialsMapD~",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "LandOfTrialsMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeConfiguration_ItemDefinition_RewardItemDefinitio~",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "RewardItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeNpcDefinition_MonsterDefinition_MonsterDefinitio~",
                schema: "config",
                table: "CastleSiegeNpcDefinition",
                column: "MonsterDefinitionId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id");
        }
    }
}
