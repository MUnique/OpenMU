// <copyright file="20250228170503_AddMasterSkillDefinitionExtendsDuration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AddMasterSkillDefinitionExtendsDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExtendsDuration",
                schema: "config",
                table: "MasterSkillDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AggregateType",
                schema: "config",
                table: "AttributeRelationship",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtendsDuration",
                schema: "config",
                table: "MasterSkillDefinition");

            migrationBuilder.DropColumn(
                name: "AggregateType",
                schema: "config",
                table: "AttributeRelationship");
        }
    }
}
