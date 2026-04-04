using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterClassId",
                schema: "config",
                table: "ConstValueAttribute",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "GameConfigurationId",
                schema: "config",
                table: "ConstValueAttribute",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GameConfigurationId",
                schema: "config",
                table: "AttributeRelationship",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConstValueAttribute_GameConfigurationId",
                schema: "config",
                table: "ConstValueAttribute",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRelationship_GameConfigurationId",
                schema: "config",
                table: "AttributeRelationship",
                column: "GameConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRelationship_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "AttributeRelationship",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConstValueAttribute_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ConstValueAttribute",
                column: "GameConfigurationId",
                principalSchema: "config",
                principalTable: "GameConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRelationship_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropForeignKey(
                name: "FK_ConstValueAttribute_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "ConstValueAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ConstValueAttribute_GameConfigurationId",
                schema: "config",
                table: "ConstValueAttribute");

            migrationBuilder.DropIndex(
                name: "IX_AttributeRelationship_GameConfigurationId",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropColumn(
                name: "GameConfigurationId",
                schema: "config",
                table: "ConstValueAttribute");

            migrationBuilder.DropColumn(
                name: "GameConfigurationId",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterClassId",
                schema: "config",
                table: "ConstValueAttribute",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
