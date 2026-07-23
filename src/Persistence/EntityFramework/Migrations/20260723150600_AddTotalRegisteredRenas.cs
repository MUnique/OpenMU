// <copyright file="20260723150600_AddTotalRegisteredRenas.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

/// <summary>
/// Migration which adds the TotalRegisteredRenas column to the Character table.
/// </summary>
public partial class AddTotalRegisteredRenas : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "TotalRegisteredRenas",
            schema: "data",
            table: "Character",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TotalRegisteredRenas",
            schema: "data",
            table: "Character");
    }
}
