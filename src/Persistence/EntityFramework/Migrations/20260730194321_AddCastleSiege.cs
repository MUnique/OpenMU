// <copyright file="20260730194321_AddCastleSiege.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AddCastleSiege : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CastleSiegeConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CastleSiegeData",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerGuildId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsOccupied = table.Column<bool>(type: "boolean", nullable: false),
                    TaxChaos = table.Column<byte>(type: "smallint", nullable: false),
                    TaxStore = table.Column<byte>(type: "smallint", nullable: false),
                    TaxHunt = table.Column<int>(type: "integer", nullable: false),
                    IsHuntZoneEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    TributeMoney = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CastleSiegeNpcState",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CastleSiegeDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    MonsterNumber = table.Column<short>(type: "smallint", nullable: false),
                    InstanceId = table.Column<byte>(type: "smallint", nullable: false),
                    DefenseLevel = table.Column<byte>(type: "smallint", nullable: false),
                    RegenLevel = table.Column<byte>(type: "smallint", nullable: false),
                    LifeLevel = table.Column<byte>(type: "smallint", nullable: false),
                    CurrentHp = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeNpcState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastleSiegeNpcState_CastleSiegeData_CastleSiegeDataId",
                        column: x => x.CastleSiegeDataId,
                        principalSchema: "data",
                        principalTable: "CastleSiegeData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CastleSiegeConfiguration",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CastleSiegeMapDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    LandOfTrialsMapDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    RewardItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DefenseRespawnAreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttackRespawnAreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CrownHoldTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    RegisterMinLevel = table.Column<int>(type: "integer", nullable: false),
                    RegisterMinMembers = table.Column<int>(type: "integer", nullable: false),
                    ParticipantRewardMinSeconds = table.Column<int>(type: "integer", nullable: false),
                    MaxAttackingGuilds = table.Column<int>(type: "integer", nullable: false),
                    GuildScoreCastleSiege = table.Column<int>(type: "integer", nullable: false),
                    GuildScoreCastleSiegeMembers = table.Column<int>(type: "integer", nullable: false),
                    GateBuyPrice = table.Column<int>(type: "integer", nullable: false),
                    StatueBuyPrice = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastleSiegeConfiguration_GameMapDefinition_CastleSiegeMapDe~",
                        column: x => x.CastleSiegeMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CastleSiegeConfiguration_GameMapDefinition_LandOfTrialsMapD~",
                        column: x => x.LandOfTrialsMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CastleSiegeConfiguration_ItemDefinition_RewardItemDefinitio~",
                        column: x => x.RewardItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CastleSiegeNpcDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonsterDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CastleSiegeConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    InstanceId = table.Column<byte>(type: "smallint", nullable: false),
                    IsPersistedToDatabase = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultSide = table.Column<byte>(type: "smallint", nullable: false),
                    SpawnX = table.Column<byte>(type: "smallint", nullable: false),
                    SpawnY = table.Column<byte>(type: "smallint", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeNpcDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastleSiegeNpcDefinition_CastleSiegeConfiguration_CastleSie~",
                        column: x => x.CastleSiegeConfigurationId,
                        principalSchema: "config",
                        principalTable: "CastleSiegeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CastleSiegeNpcDefinition_MonsterDefinition_MonsterDefinitio~",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CastleSiegeStateScheduleEntry",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CastleSiegeConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    State = table.Column<byte>(type: "smallint", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    Hour = table.Column<byte>(type: "smallint", nullable: false),
                    Minute = table.Column<byte>(type: "smallint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeStateScheduleEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastleSiegeStateScheduleEntry_CastleSiegeConfiguration_Cast~",
                        column: x => x.CastleSiegeConfigurationId,
                        principalSchema: "config",
                        principalTable: "CastleSiegeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CastleSiegeUpgradeDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CastleSiegeConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CastleSiegeConfigurationId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    CastleSiegeConfigurationId2 = table.Column<Guid>(type: "uuid", nullable: true),
                    CastleSiegeConfigurationId3 = table.Column<Guid>(type: "uuid", nullable: true),
                    CastleSiegeConfigurationId4 = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    RequiredJewelOfGuardianCount = table.Column<int>(type: "integer", nullable: false),
                    RequiredZen = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeUpgradeDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastleSiegeUpgradeDefinition_CastleSiegeConfiguration_Castl~",
                        column: x => x.CastleSiegeConfigurationId,
                        principalSchema: "config",
                        principalTable: "CastleSiegeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CastleSiegeUpgradeDefinition_CastleSiegeConfiguration_Cast~1",
                        column: x => x.CastleSiegeConfigurationId1,
                        principalSchema: "config",
                        principalTable: "CastleSiegeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CastleSiegeUpgradeDefinition_CastleSiegeConfiguration_Cast~2",
                        column: x => x.CastleSiegeConfigurationId2,
                        principalSchema: "config",
                        principalTable: "CastleSiegeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CastleSiegeUpgradeDefinition_CastleSiegeConfiguration_Cast~3",
                        column: x => x.CastleSiegeConfigurationId3,
                        principalSchema: "config",
                        principalTable: "CastleSiegeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CastleSiegeUpgradeDefinition_CastleSiegeConfiguration_Cast~4",
                        column: x => x.CastleSiegeConfigurationId4,
                        principalSchema: "config",
                        principalTable: "CastleSiegeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CastleSiegeZoneDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CastleSiegeConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CastleSiegeConfigurationId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    X1 = table.Column<byte>(type: "smallint", nullable: false),
                    Y1 = table.Column<byte>(type: "smallint", nullable: false),
                    X2 = table.Column<byte>(type: "smallint", nullable: false),
                    Y2 = table.Column<byte>(type: "smallint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeZoneDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastleSiegeZoneDefinition_CastleSiegeConfiguration_CastleSi~",
                        column: x => x.CastleSiegeConfigurationId,
                        principalSchema: "config",
                        principalTable: "CastleSiegeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CastleSiegeZoneDefinition_CastleSiegeConfiguration_CastleS~1",
                        column: x => x.CastleSiegeConfigurationId1,
                        principalSchema: "config",
                        principalTable: "CastleSiegeConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameConfiguration_CastleSiegeConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                column: "CastleSiegeConfigurationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeConfiguration_AttackRespawnAreaId",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "AttackRespawnAreaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeConfiguration_CastleSiegeMapDefinitionId",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "CastleSiegeMapDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeConfiguration_DefenseRespawnAreaId",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "DefenseRespawnAreaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeConfiguration_LandOfTrialsMapDefinitionId",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "LandOfTrialsMapDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeConfiguration_RewardItemDefinitionId",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "RewardItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeNpcDefinition_CastleSiegeConfigurationId",
                schema: "config",
                table: "CastleSiegeNpcDefinition",
                column: "CastleSiegeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeNpcDefinition_MonsterDefinitionId",
                schema: "config",
                table: "CastleSiegeNpcDefinition",
                column: "MonsterDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeNpcState_CastleSiegeDataId",
                schema: "data",
                table: "CastleSiegeNpcState",
                column: "CastleSiegeDataId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeStateScheduleEntry_CastleSiegeConfigurationId",
                schema: "config",
                table: "CastleSiegeStateScheduleEntry",
                column: "CastleSiegeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeUpgradeDefinition_CastleSiegeConfigurationId",
                schema: "config",
                table: "CastleSiegeUpgradeDefinition",
                column: "CastleSiegeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeUpgradeDefinition_CastleSiegeConfigurationId1",
                schema: "config",
                table: "CastleSiegeUpgradeDefinition",
                column: "CastleSiegeConfigurationId1");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeUpgradeDefinition_CastleSiegeConfigurationId2",
                schema: "config",
                table: "CastleSiegeUpgradeDefinition",
                column: "CastleSiegeConfigurationId2");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeUpgradeDefinition_CastleSiegeConfigurationId3",
                schema: "config",
                table: "CastleSiegeUpgradeDefinition",
                column: "CastleSiegeConfigurationId3");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeUpgradeDefinition_CastleSiegeConfigurationId4",
                schema: "config",
                table: "CastleSiegeUpgradeDefinition",
                column: "CastleSiegeConfigurationId4");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeZoneDefinition_CastleSiegeConfigurationId",
                schema: "config",
                table: "CastleSiegeZoneDefinition",
                column: "CastleSiegeConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeZoneDefinition_CastleSiegeConfigurationId1",
                schema: "config",
                table: "CastleSiegeZoneDefinition",
                column: "CastleSiegeConfigurationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GameConfiguration_CastleSiegeConfiguration_CastleSiegeConfi~",
                schema: "config",
                table: "GameConfiguration",
                column: "CastleSiegeConfigurationId",
                principalSchema: "config",
                principalTable: "CastleSiegeConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeConfiguration_CastleSiegeZoneDefinition_AttackRe~",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "AttackRespawnAreaId",
                principalSchema: "config",
                principalTable: "CastleSiegeZoneDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeConfiguration_CastleSiegeZoneDefinition_DefenseR~",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "DefenseRespawnAreaId",
                principalSchema: "config",
                principalTable: "CastleSiegeZoneDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameConfiguration_CastleSiegeConfiguration_CastleSiegeConfi~",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeConfiguration_CastleSiegeZoneDefinition_AttackRe~",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeConfiguration_CastleSiegeZoneDefinition_DefenseR~",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropTable(
                name: "CastleSiegeNpcDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CastleSiegeNpcState",
                schema: "data");

            migrationBuilder.DropTable(
                name: "CastleSiegeStateScheduleEntry",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CastleSiegeUpgradeDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CastleSiegeData",
                schema: "data");

            migrationBuilder.DropTable(
                name: "CastleSiegeZoneDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CastleSiegeConfiguration",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_GameConfiguration_CastleSiegeConfigurationId",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "CastleSiegeConfigurationId",
                schema: "config",
                table: "GameConfiguration");
        }
    }
}
