using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddGameConfigurationMaxItemOptLevelDrop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "MaximumItemOptionLevelDrop",
                schema: "config",
                table: "GameConfiguration",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumItemOptionLevelDrop",
                schema: "config",
                table: "GameConfiguration");
        }
    }
}
