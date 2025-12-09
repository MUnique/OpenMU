using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddMagicEffectChanceAndOthers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MaximumValue",
                schema: "config",
                table: "PowerUpDefinitionValue",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChanceId",
                schema: "config",
                table: "MagicEffectDefinition",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChancePvpId",
                schema: "config",
                table: "MagicEffectDefinition",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DurationDependsOnTargetLevel",
                schema: "config",
                table: "MagicEffectDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DurationPvpId",
                schema: "config",
                table: "MagicEffectDefinition",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "MonsterTargetLevelDivisor",
                schema: "config",
                table: "MagicEffectDefinition",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PlayerTargetLevelDivisor",
                schema: "config",
                table: "MagicEffectDefinition",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "IX_MagicEffectDefinition_ChanceId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "ChanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MagicEffectDefinition_ChancePvpId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "ChancePvpId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MagicEffectDefinition_DurationPvpId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "DurationPvpId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_ChanceId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "ChanceId",
                principalSchema: "config",
                principalTable: "PowerUpDefinitionValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_ChancePvpId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "ChancePvpId",
                principalSchema: "config",
                principalTable: "PowerUpDefinitionValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_DurationPvpId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "DurationPvpId",
                principalSchema: "config",
                principalTable: "PowerUpDefinitionValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_ChanceId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_ChancePvpId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_DurationPvpId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropIndex(
                name: "IX_MagicEffectDefinition_ChanceId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropIndex(
                name: "IX_MagicEffectDefinition_ChancePvpId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropIndex(
                name: "IX_MagicEffectDefinition_DurationPvpId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropColumn(
                name: "MaximumValue",
                schema: "config",
                table: "PowerUpDefinitionValue");

            migrationBuilder.DropColumn(
                name: "ChanceId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropColumn(
                name: "ChancePvpId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropColumn(
                name: "DurationDependsOnTargetLevel",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropColumn(
                name: "DurationPvpId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropColumn(
                name: "MonsterTargetLevelDivisor",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropColumn(
                name: "PlayerTargetLevelDivisor",
                schema: "config",
                table: "MagicEffectDefinition");
        }
    }
}
