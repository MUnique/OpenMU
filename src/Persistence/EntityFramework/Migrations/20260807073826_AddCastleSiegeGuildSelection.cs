// <copyright file="20260807073826_AddCastleSiegeGuildSelection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AddCastleSiegeGuildSelection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CastleSiegeGuild",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CastleSiegeDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    GuildId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildName = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Side = table.Column<byte>(type: "smallint", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    IsAllianceMaster = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegeGuild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastleSiegeGuild_CastleSiegeData_CastleSiegeDataId",
                        column: x => x.CastleSiegeDataId,
                        principalSchema: "data",
                        principalTable: "CastleSiegeData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CastleSiegeGuild_Guild_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "guild",
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CastleSiegePendingReward",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastleSiegePendingReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastleSiegePendingReward_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CastleSiegePendingReward_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeGuild_CastleSiegeDataId",
                schema: "data",
                table: "CastleSiegeGuild",
                column: "CastleSiegeDataId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeGuild_GuildId",
                schema: "data",
                table: "CastleSiegeGuild",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegePendingReward_CharacterId",
                schema: "data",
                table: "CastleSiegePendingReward",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegePendingReward_ItemDefinitionId",
                schema: "data",
                table: "CastleSiegePendingReward",
                column: "ItemDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CastleSiegeGuild",
                schema: "data");

            migrationBuilder.DropTable(
                name: "CastleSiegePendingReward",
                schema: "data");
        }
    }
}
