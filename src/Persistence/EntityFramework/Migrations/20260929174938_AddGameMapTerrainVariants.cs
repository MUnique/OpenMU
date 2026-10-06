using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddGameMapTerrainVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameMapTerrainVariant",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameMapDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TerrainData = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameMapTerrainVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameMapTerrainVariant_GameMapDefinition_GameMapDefinitionId",
                        column: x => x.GameMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameMapTerrainVariant_GameMapDefinitionId",
                schema: "config",
                table: "GameMapTerrainVariant",
                column: "GameMapDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameMapTerrainVariant",
                schema: "config");
        }
    }
}
