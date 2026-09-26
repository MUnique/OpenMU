using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations.WeeklyQuests
{
    /// <inheritdoc />
    public partial class InitialWeeklyQuests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "weekly");

            migrationBuilder.CreateTable(
                name: "WeeklyQuestProgress",
                schema: "weekly",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    QuestId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RewardedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyQuestProgress", x => new { x.CharacterId, x.PeriodStart, x.QuestId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyQuestProgress_PeriodStart",
                schema: "weekly",
                table: "WeeklyQuestProgress",
                column: "PeriodStart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeeklyQuestProgress",
                schema: "weekly");
        }
    }
}
