using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class MaximumDropLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "MaximumDropLevel",
                schema: "config",
                table: "ItemDefinition",
                type: "smallint",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE config.""ItemDefinition""
                SET ""MaximumDropLevel"" = 66
                WHERE ""Group"" = 12 AND ""Number"" = 15");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumDropLevel",
                schema: "config",
                table: "ItemDefinition");
        }
    }
}
