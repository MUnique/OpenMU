using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddExcellentItemDropLevelDelta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ExcellentItemDropLevelDelta",
                schema: "config",
                table: "GameConfiguration",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)25);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcellentItemDropLevelDelta",
                schema: "config",
                table: "GameConfiguration");
        }
    }
}
