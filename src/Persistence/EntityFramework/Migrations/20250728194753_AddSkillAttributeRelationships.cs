using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillAttributeRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SkillId",
                schema: "config",
                table: "AttributeRelationship",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRelationship_SkillId",
                schema: "config",
                table: "AttributeRelationship",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRelationship_Skill_SkillId",
                schema: "config",
                table: "AttributeRelationship",
                column: "SkillId",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRelationship_Skill_SkillId",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropIndex(
                name: "IX_AttributeRelationship_SkillId",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropColumn(
                name: "SkillId",
                schema: "config",
                table: "AttributeRelationship");
        }
    }
}
