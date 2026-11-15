using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <summary>
    /// Adds the flag whether players can use a warp. Existing warps stay usable; the update
    /// "Restrict warps outside of the original warp window" restricts the ones of the original client.
    /// </summary>
    public partial class AddWarpInfoIsWarpable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWarpable",
                schema: "config",
                table: "WarpInfo",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWarpable",
                schema: "config",
                table: "WarpInfo");
        }
    }
}
