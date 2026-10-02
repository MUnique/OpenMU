using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddGensAbuse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GensAbuse",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KillerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VictimId = table.Column<Guid>(type: "uuid", nullable: false),
                    KillCount = table.Column<int>(type: "integer", nullable: false),
                    LastKillAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GensAbuse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GensAbuse_Character_KillerId",
                        column: x => x.KillerId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GensAbuse_Character_VictimId",
                        column: x => x.VictimId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GensAbuse_KillerId_VictimId",
                schema: "data",
                table: "GensAbuse",
                columns: new[] { "KillerId", "VictimId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GensAbuse_VictimId",
                schema: "data",
                table: "GensAbuse",
                column: "VictimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GensAbuse",
                schema: "data");
        }
    }
}
