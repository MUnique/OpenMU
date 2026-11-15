using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddExcellentOptionsToDropGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ExcellentOptionsAddChance",
                schema: "config",
                table: "ItemDropItemGroup",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExcellentOptionsMaximumOptionsPerItem",
                schema: "config",
                table: "ItemDropItemGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ExcellentOptionsAddChance",
                schema: "config",
                table: "DropItemGroup",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExcellentOptionsMaximumOptionsPerItem",
                schema: "config",
                table: "DropItemGroup",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcellentOptionsAddChance",
                schema: "config",
                table: "ItemDropItemGroup");

            migrationBuilder.DropColumn(
                name: "ExcellentOptionsMaximumOptionsPerItem",
                schema: "config",
                table: "ItemDropItemGroup");

            migrationBuilder.DropColumn(
                name: "ExcellentOptionsAddChance",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropColumn(
                name: "ExcellentOptionsMaximumOptionsPerItem",
                schema: "config",
                table: "DropItemGroup");
        }
    }
}
