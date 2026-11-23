using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddZenDropsAndRewardTiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DropsNoZen",
                schema: "config",
                table: "MonsterDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaximumRank",
                schema: "config",
                table: "MiniGameReward",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumRankPercentage",
                schema: "config",
                table: "MiniGameReward",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumRankedPlayers",
                schema: "config",
                table: "MiniGameReward",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinimumRankPercentage",
                schema: "config",
                table: "MiniGameReward",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinimumRankedPlayers",
                schema: "config",
                table: "MiniGameReward",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DropsNoZen",
                schema: "config",
                table: "MonsterDefinition");

            migrationBuilder.DropColumn(
                name: "MaximumRank",
                schema: "config",
                table: "MiniGameReward");

            migrationBuilder.DropColumn(
                name: "MaximumRankPercentage",
                schema: "config",
                table: "MiniGameReward");

            migrationBuilder.DropColumn(
                name: "MaximumRankedPlayers",
                schema: "config",
                table: "MiniGameReward");

            migrationBuilder.DropColumn(
                name: "MinimumRankPercentage",
                schema: "config",
                table: "MiniGameReward");

            migrationBuilder.DropColumn(
                name: "MinimumRankedPlayers",
                schema: "config",
                table: "MiniGameReward");
        }
    }
}
