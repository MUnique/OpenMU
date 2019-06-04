using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    public partial class AddMapRequirements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttributeRequirementGameMapDefinition",
                schema: "config",
                columns: table => new
                {
                    AttributeRequirementId = table.Column<Guid>(nullable: false),
                    GameMapDefinitionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeRequirementGameMapDefinition", x => new { x.AttributeRequirementId, x.GameMapDefinitionId });
                    table.ForeignKey(
                        name: "FK_AttributeRequirementGameMapDefinition_AttributeRequirement_~",
                        column: x => x.AttributeRequirementId,
                        principalSchema: "config",
                        principalTable: "AttributeRequirement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeRequirementGameMapDefinition_GameMapDefinition_Gam~",
                        column: x => x.GameMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRequirement_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "GameMapDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRequirementGameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirementGameMapDefinition",
                column: "GameMapDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "GameMapDefinitionId",
                principalSchema: "config",
                principalTable: "GameMapDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            var configRoleName = ConnectionConfigurator.GetRoleName(DatabaseRole.Configuration);
            migrationBuilder.Sql($"GRANT SELECT ON ALL TABLES IN SCHEMA config TO GROUP {configRoleName};");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_GameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropTable(
                name: "AttributeRequirementGameMapDefinition",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_AttributeRequirement_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropColumn(
                name: "GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement");
        }
    }
}
