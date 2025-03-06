using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddStageToAttributeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Stage",
                schema: "config",
                table: "PowerUpDefinitionValue",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "ExtendsDuration",
                schema: "config",
                table: "MasterSkillDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "Stage",
                schema: "config",
                table: "ItemBasePowerUpDefinition",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "AggregateType",
                schema: "config",
                table: "ConstValueAttribute",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "Stage",
                schema: "config",
                table: "ConstValueAttribute",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "AggregateType",
                schema: "config",
                table: "AttributeRelationship",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "Stage",
                schema: "config",
                table: "AttributeRelationship",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stage",
                schema: "config",
                table: "PowerUpDefinitionValue");

            migrationBuilder.DropColumn(
                name: "ExtendsDuration",
                schema: "config",
                table: "MasterSkillDefinition");

            migrationBuilder.DropColumn(
                name: "Stage",
                schema: "config",
                table: "ItemBasePowerUpDefinition");

            migrationBuilder.DropColumn(
                name: "AggregateType",
                schema: "config",
                table: "ConstValueAttribute");

            migrationBuilder.DropColumn(
                name: "Stage",
                schema: "config",
                table: "ConstValueAttribute");

            migrationBuilder.DropColumn(
                name: "AggregateType",
                schema: "config",
                table: "AttributeRelationship");

            migrationBuilder.DropColumn(
                name: "Stage",
                schema: "config",
                table: "AttributeRelationship");
        }
    }
}
