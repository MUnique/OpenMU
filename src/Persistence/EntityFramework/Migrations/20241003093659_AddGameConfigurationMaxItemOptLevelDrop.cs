// <copyright file="20241003093659_AddGameConfigurationMaxItemOptLevelDrop.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

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
