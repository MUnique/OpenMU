// <copyright file="20221008183306_PetLevels.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Adds pet experience columns to Item and ItemDefinition.
    /// </summary>
    public partial class PetLevels : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PetExperienceFormula",
                schema: "config",
                table: "ItemDefinition",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PetExperience",
                schema: "data",
                table: "Item",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PetExperienceFormula",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "PetExperience",
                schema: "data",
                table: "Item");
        }
    }
}
