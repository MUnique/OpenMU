// <copyright file="20240805183034_ExpFormulas.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class ExpFormulas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExperienceFormula",
                schema: "config",
                table: "GameConfiguration",
                type: "text",
                nullable: true,
                defaultValue: "if(level == 0, 0, if(level < 256, 10 * (level + 8) * (level - 1) * (level - 1), (10 * (level + 8) * (level - 1) * (level - 1)) + (1000 * (level - 247) * (level - 256) * (level - 256))))");

            migrationBuilder.AddColumn<string>(
                name: "MasterExperienceFormula",
                schema: "config",
                table: "GameConfiguration",
                type: "text",
                nullable: true,
                defaultValue: "(505 * level * level * level) + (35278500 * level) + (228045 * level * level)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceFormula",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "MasterExperienceFormula",
                schema: "config",
                table: "GameConfiguration");
        }
    }
}
