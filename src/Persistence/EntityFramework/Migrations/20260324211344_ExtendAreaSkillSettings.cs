// <copyright file="20260324211344_ExtendAreaSkillSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class ExtendAreaSkillSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SkipElementalModifier",
                schema: "config",
                table: "Skill",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EffectRange",
                schema: "config",
                table: "AreaSkillSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinimumNumberOfHitsPerAttack",
                schema: "config",
                table: "AreaSkillSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkipElementalModifier",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "EffectRange",
                schema: "config",
                table: "AreaSkillSettings");

            migrationBuilder.DropColumn(
                name: "MinimumNumberOfHitsPerAttack",
                schema: "config",
                table: "AreaSkillSettings");
        }
    }
}
