using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <summary>
    /// Adds the item stat profiles and the item formula fields. Existing configurations get the drop level bonuses
    /// 25 and 30 and existing items <c>ExcellentRaisesLevel</c>, which the item formulas use today; the profiles
    /// are added by the AddItemStatProfiles configuration update.
    /// </summary>
    public partial class AddItemStatProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ExcellentBonus",
                schema: "config",
                table: "ItemDefinition",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "ExcellentRaisesLevel",
                schema: "config",
                table: "ItemDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StatProfileId",
                schema: "config",
                table: "ItemDefinition",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "AncientDropLevelBonus",
                schema: "config",
                table: "GameConfiguration",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)30);

            migrationBuilder.AddColumn<byte>(
                name: "ExcellentDropLevelBonus",
                schema: "config",
                table: "GameConfiguration",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)25);

            migrationBuilder.CreateTable(
                name: "ItemStatProfile",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LevelPerItemLevel = table.Column<int>(type: "integer", nullable: true),
                    ExcellentLevelAdd = table.Column<int>(type: "integer", nullable: true),
                    StrengthPerOptionLevel = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStatProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemStatProfile_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemRequirementFormula",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemStatProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    KeepBase = table.Column<bool>(type: "boolean", nullable: false),
                    Add = table.Column<int>(type: "integer", nullable: false),
                    LevelWeight = table.Column<int>(type: "integer", nullable: false),
                    Percent = table.Column<int>(type: "integer", nullable: false),
                    LevelRequirementBasis = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRequirementFormula", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemRequirementFormula_AttributeDefinition_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemRequirementFormula_ItemStatProfile_ItemStatProfileId",
                        column: x => x.ItemStatProfileId,
                        principalSchema: "config",
                        principalTable: "ItemStatProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemStatTerms",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    TableId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemStatProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    TableState = table.Column<int>(type: "integer", nullable: false),
                    ExcellentState = table.Column<int>(type: "integer", nullable: false),
                    ExcellentTimes = table.Column<int>(type: "integer", nullable: false),
                    ExcellentAdd = table.Column<int>(type: "integer", nullable: false),
                    ExcellentDropLevelDivisor = table.Column<int>(type: "integer", nullable: false),
                    AncientState = table.Column<int>(type: "integer", nullable: false),
                    AncientTimes = table.Column<int>(type: "integer", nullable: false),
                    AncientAdd = table.Column<int>(type: "integer", nullable: false),
                    AncientDropLevelDivisor = table.Column<int>(type: "integer", nullable: false),
                    AncientAfterLevelBonus = table.Column<bool>(type: "boolean", nullable: false),
                    PerLevelAfterHalving = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStatTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemStatTerms_AttributeDefinition_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemStatTerms_ItemLevelBonusTable_TableId",
                        column: x => x.TableId,
                        principalSchema: "config",
                        principalTable: "ItemLevelBonusTable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemStatTerms_ItemStatProfile_ItemStatProfileId",
                        column: x => x.ItemStatProfileId,
                        principalSchema: "config",
                        principalTable: "ItemStatProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_StatProfileId",
                schema: "config",
                table: "ItemDefinition",
                column: "StatProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequirementFormula_AttributeId",
                schema: "config",
                table: "ItemRequirementFormula",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRequirementFormula_ItemStatProfileId_AttributeId",
                schema: "config",
                table: "ItemRequirementFormula",
                columns: new[] { "ItemStatProfileId", "AttributeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemStatProfile_GameConfigurationId_Name",
                schema: "config",
                table: "ItemStatProfile",
                columns: new[] { "GameConfigurationId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemStatTerms_AttributeId",
                schema: "config",
                table: "ItemStatTerms",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemStatTerms_ItemStatProfileId_AttributeId",
                schema: "config",
                table: "ItemStatTerms",
                columns: new[] { "ItemStatProfileId", "AttributeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemStatTerms_TableId",
                schema: "config",
                table: "ItemStatTerms",
                column: "TableId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDefinition_ItemStatProfile_StatProfileId",
                schema: "config",
                table: "ItemDefinition",
                column: "StatProfileId",
                principalSchema: "config",
                principalTable: "ItemStatProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemDefinition_ItemStatProfile_StatProfileId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropTable(
                name: "ItemRequirementFormula",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemStatTerms",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemStatProfile",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_ItemDefinition_StatProfileId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "ExcellentBonus",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "ExcellentRaisesLevel",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "StatProfileId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "AncientDropLevelBonus",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "ExcellentDropLevelBonus",
                schema: "config",
                table: "GameConfiguration");
        }
    }
}
