using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <summary>
    /// Adds the flag whether a warp is shown in the warp list of the game client. Existing warps are shown.
    /// </summary>
    public partial class AddWarpInfoIsShownInWarpList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShownInWarpList",
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
                name: "IsShownInWarpList",
                schema: "config",
                table: "WarpInfo");
        }
    }
}
