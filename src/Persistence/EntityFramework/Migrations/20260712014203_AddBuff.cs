// <copyright file="20260712014203_AddBuff.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AddBuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buff",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MagicEffectDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MonsterDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MinimumLevel = table.Column<int>(type: "integer", nullable: true),
                    MaximumLevel = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buff_MagicEffectDefinition_MagicEffectDefinitionId",
                        column: x => x.MagicEffectDefinitionId,
                        principalSchema: "config",
                        principalTable: "MagicEffectDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Buff_MonsterDefinition_MonsterDefinitionId",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buff_MagicEffectDefinitionId",
                schema: "config",
                table: "Buff",
                column: "MagicEffectDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buff_MonsterDefinitionId",
                schema: "config",
                table: "Buff",
                column: "MonsterDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Buff",
                schema: "config");
        }
    }
}
