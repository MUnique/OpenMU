using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
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
