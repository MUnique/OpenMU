using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations.WeeklyQuests
{
    /// <inheritdoc />
    public partial class AddWeeklyQuestObjectiveCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "AdditionalCounts",
                schema: "weekly",
                table: "WeeklyQuestProgress",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalCounts",
                schema: "weekly",
                table: "WeeklyQuestProgress");
        }
    }
}
