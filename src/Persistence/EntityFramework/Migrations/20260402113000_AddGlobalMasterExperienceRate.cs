// <copyright file="20260402113000_AddGlobalMasterExperienceRate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    [DbContext(typeof(EntityDataContext))]
    [Migration("20260402113000_AddGlobalMasterExperienceRate")]
    public partial class AddGlobalMasterExperienceRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MasterExperienceRate",
                schema: "config",
                table: "GameConfiguration",
                type: "real",
                nullable: false,
                defaultValue: 1f);

            migrationBuilder.Sql("""UPDATE config."GameConfiguration" SET "MasterExperienceRate" = "ExperienceRate";""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasterExperienceRate",
                schema: "config",
                table: "GameConfiguration");
        }
    }
}
