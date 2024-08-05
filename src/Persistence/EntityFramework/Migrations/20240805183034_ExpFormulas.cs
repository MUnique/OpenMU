using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
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
