using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddBuffRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuffRequest",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MagicEffectDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MonsterDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MinimumLevel = table.Column<int>(type: "integer", nullable: true),
                    MaximumLevel = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuffRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuffRequest_MagicEffectDefinition_MagicEffectDefinitionId",
                        column: x => x.MagicEffectDefinitionId,
                        principalSchema: "config",
                        principalTable: "MagicEffectDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuffRequest_MonsterDefinition_MonsterDefinitionId",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuffRequest_MagicEffectDefinitionId",
                schema: "config",
                table: "BuffRequest",
                column: "MagicEffectDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuffRequest_MonsterDefinitionId",
                schema: "config",
                table: "BuffRequest",
                column: "MonsterDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuffRequest",
                schema: "config");
        }
    }
}
