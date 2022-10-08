using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    public partial class PetLevels : Migration
    {
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
