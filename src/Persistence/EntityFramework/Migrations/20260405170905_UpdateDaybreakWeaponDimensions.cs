// <copyright file="20260405170905_UpdateDaybreakWeaponDimensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    [DbContext(typeof(EntityDataContext))]
    [Migration("20260405170905_UpdateDaybreakWeaponDimensions")]
    public partial class UpdateDaybreakWeaponDimensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE config.""ItemDefinition"" 
                SET ""Height"" = 4 
                WHERE ""Group"" = 0 AND ""Number"" = 24");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE config.""ItemDefinition"" 
                SET ""Height"" = 2 
                WHERE ""Group"" = 0 AND ""Number"" = 24");
        }
    }
}
