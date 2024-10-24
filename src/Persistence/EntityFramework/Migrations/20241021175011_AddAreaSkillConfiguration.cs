using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaSkillConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AreaSkillSettingsId",
                schema: "config",
                table: "Skill",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AreaSkillSettings",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UseFrustumFilter = table.Column<bool>(type: "boolean", nullable: false),
                    FrustumStartWidth = table.Column<float>(type: "real", nullable: false),
                    FrustumEndWidth = table.Column<float>(type: "real", nullable: false),
                    FrustumDistance = table.Column<float>(type: "real", nullable: false),
                    UseTargetAreaFilter = table.Column<bool>(type: "boolean", nullable: false),
                    TargetAreaDiameter = table.Column<float>(type: "real", nullable: false),
                    UseDeferredHits = table.Column<bool>(type: "boolean", nullable: false),
                    DelayPerOneDistance = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DelayBetweenHits = table.Column<TimeSpan>(type: "interval", nullable: false),
                    MinimumNumberOfHitsPerTarget = table.Column<int>(type: "integer", nullable: false),
                    MaximumNumberOfHitsPerTarget = table.Column<int>(type: "integer", nullable: false),
                    MaximumNumberOfHitsPerAttack = table.Column<int>(type: "integer", nullable: false),
                    HitChancePerDistanceMultiplier = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaSkillSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skill_AreaSkillSettingsId",
                schema: "config",
                table: "Skill",
                column: "AreaSkillSettingsId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_AreaSkillSettings_AreaSkillSettingsId",
                schema: "config",
                table: "Skill",
                column: "AreaSkillSettingsId",
                principalSchema: "config",
                principalTable: "AreaSkillSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skill_AreaSkillSettings_AreaSkillSettingsId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropTable(
                name: "AreaSkillSettings",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_Skill_AreaSkillSettingsId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "AreaSkillSettingsId",
                schema: "config",
                table: "Skill");
        }
    }
}
