using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
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
