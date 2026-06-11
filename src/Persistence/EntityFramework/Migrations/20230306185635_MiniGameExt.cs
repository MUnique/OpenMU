// <copyright file="20230306185635_MiniGameExt.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class MiniGameExt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClientUpdateRequired",
                schema: "config",
                table: "MiniGameTerrainChange",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                schema: "data",
                table: "MiniGameRankingEntry",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "AllowParty",
                schema: "config",
                table: "MiniGameDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ArePlayerKillersAllowedToEnter",
                schema: "config",
                table: "MiniGameDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EntranceFee",
                schema: "config",
                table: "MiniGameDefinition",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClientUpdateRequired",
                schema: "config",
                table: "MiniGameTerrainChange");

            migrationBuilder.DropColumn(
                name: "AllowParty",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropColumn(
                name: "ArePlayerKillersAllowedToEnter",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.DropColumn(
                name: "EntranceFee",
                schema: "config",
                table: "MiniGameDefinition");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                schema: "data",
                table: "MiniGameRankingEntry",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
