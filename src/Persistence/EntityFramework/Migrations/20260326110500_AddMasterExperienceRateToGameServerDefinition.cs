using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(EntityDataContext))]
    [Migration("20260326110500_AddMasterExperienceRateToGameServerDefinition")]
    public partial class AddMasterExperienceRateToGameServerDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MasterExperienceRate",
                schema: "config",
                table: "GameServerDefinition",
                type: "real",
                nullable: false,
                defaultValue: 1f);

            migrationBuilder.Sql("""UPDATE config."GameServerDefinition" SET "MasterExperienceRate" = "ExperienceRate";""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasterExperienceRate",
                schema: "config",
                table: "GameServerDefinition");
        }
    }
}
