// <copyright file="20260425205521_MaximumDropLevel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

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
