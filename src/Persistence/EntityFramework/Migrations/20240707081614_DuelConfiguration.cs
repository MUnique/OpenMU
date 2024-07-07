using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class DuelConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DuelConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DuelConfiguration",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExitId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaximumScore = table.Column<int>(type: "integer", nullable: false),
                    EntranceFee = table.Column<int>(type: "integer", nullable: false),
                    MinimumCharacterLevel = table.Column<int>(type: "integer", nullable: false),
                    MaximumSpectatorsPerDuelRoom = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuelConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuelConfiguration_ExitGate_ExitId",
                        column: x => x.ExitId,
                        principalSchema: "config",
                        principalTable: "ExitGate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DuelArea",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstPlayerGateId = table.Column<Guid>(type: "uuid", nullable: true),
                    SecondPlayerGateId = table.Column<Guid>(type: "uuid", nullable: true),
                    SpectatorsGateId = table.Column<Guid>(type: "uuid", nullable: true),
                    DuelConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Index = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuelArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuelArea_DuelConfiguration_DuelConfigurationId",
                        column: x => x.DuelConfigurationId,
                        principalSchema: "config",
                        principalTable: "DuelConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DuelArea_ExitGate_FirstPlayerGateId",
                        column: x => x.FirstPlayerGateId,
                        principalSchema: "config",
                        principalTable: "ExitGate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DuelArea_ExitGate_SecondPlayerGateId",
                        column: x => x.SecondPlayerGateId,
                        principalSchema: "config",
                        principalTable: "ExitGate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DuelArea_ExitGate_SpectatorsGateId",
                        column: x => x.SpectatorsGateId,
                        principalSchema: "config",
                        principalTable: "ExitGate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameConfiguration_DuelConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                column: "DuelConfigurationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DuelArea_DuelConfigurationId",
                schema: "config",
                table: "DuelArea",
                column: "DuelConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_DuelArea_FirstPlayerGateId",
                schema: "config",
                table: "DuelArea",
                column: "FirstPlayerGateId");

            migrationBuilder.CreateIndex(
                name: "IX_DuelArea_SecondPlayerGateId",
                schema: "config",
                table: "DuelArea",
                column: "SecondPlayerGateId");

            migrationBuilder.CreateIndex(
                name: "IX_DuelArea_SpectatorsGateId",
                schema: "config",
                table: "DuelArea",
                column: "SpectatorsGateId");

            migrationBuilder.CreateIndex(
                name: "IX_DuelConfiguration_ExitId",
                schema: "config",
                table: "DuelConfiguration",
                column: "ExitId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameConfiguration_DuelConfiguration_DuelConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                column: "DuelConfigurationId",
                principalSchema: "config",
                principalTable: "DuelConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameConfiguration_DuelConfiguration_DuelConfigurationId",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropTable(
                name: "DuelArea",
                schema: "config");

            migrationBuilder.DropTable(
                name: "DuelConfiguration",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_GameConfiguration_DuelConfigurationId",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "DuelConfigurationId",
                schema: "config",
                table: "GameConfiguration");
        }
    }
}
