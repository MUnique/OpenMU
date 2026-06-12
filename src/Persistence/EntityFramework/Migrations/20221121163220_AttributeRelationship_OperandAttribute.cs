// <copyright file="20221121163220_AttributeRelationship_OperandAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AttributeRelationshipOperandAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OperandAttributeId",
                schema: "config",
                table: "AttributeRelationship",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRelationship_OperandAttributeId",
                schema: "config",
                table: "AttributeRelationship",
                column: "OperandAttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRelationship_AttributeDefinition_OperandAttributeId",
                schema: "config",
                table: "AttributeRelationship",
                column: "OperandAttributeId",
                principalSchema: "config",
                principalTable: "AttributeDefinition",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRelationship_AttributeDefinition_OperandAttributeId",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropIndex(
                name: "IX_AttributeRelationship_OperandAttributeId",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropColumn(
                name: "OperandAttributeId",
                schema: "config",
                table: "AttributeRelationship");
        }
    }
}
