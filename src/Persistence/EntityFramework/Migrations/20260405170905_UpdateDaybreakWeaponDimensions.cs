using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
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
