using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
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
