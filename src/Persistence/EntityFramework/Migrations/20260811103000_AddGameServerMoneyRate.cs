// <copyright file="20260811103000_AddGameServerMoneyRate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Adds a per-game-server Zen multiplier for MU Nueva Era.
    /// </summary>
    [DbContext(typeof(EntityDataContext))]
    [Migration("20260811103000_AddGameServerMoneyRate")]
    public partial class AddGameServerMoneyRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MoneyRate",
                schema: "config",
                table: "GameServerDefinition",
                type: "real",
                nullable: false,
                defaultValue: 1f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoneyRate",
                schema: "config",
                table: "GameServerDefinition");
        }
    }
}
