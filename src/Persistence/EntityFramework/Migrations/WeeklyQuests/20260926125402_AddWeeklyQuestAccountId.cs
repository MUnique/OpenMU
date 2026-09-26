using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations.WeeklyQuests
{
    /// <inheritdoc />
    public partial class AddWeeklyQuestAccountId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                schema: "weekly",
                table: "WeeklyQuestProgress",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyQuestProgress_AccountId_PeriodStart_QuestId",
                schema: "weekly",
                table: "WeeklyQuestProgress",
                columns: new[] { "AccountId", "PeriodStart", "QuestId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WeeklyQuestProgress_AccountId_PeriodStart_QuestId",
                schema: "weekly",
                table: "WeeklyQuestProgress");

            migrationBuilder.DropColumn(
                name: "AccountId",
                schema: "weekly",
                table: "WeeklyQuestProgress");
        }
    }
}
