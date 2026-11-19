using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddWeightedValueTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SocketCountTableId",
                schema: "config",
                table: "ItemDropItemGroup",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SocketCountTableId",
                schema: "config",
                table: "ItemDefinition",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SocketCountTableId",
                schema: "config",
                table: "DropItemGroup",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WeightedValueTable",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightedValueTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeightedValueTable_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeightedValue",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeightedValueTableId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightedValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeightedValue_WeightedValueTable_WeightedValueTableId",
                        column: x => x.WeightedValueTableId,
                        principalSchema: "config",
                        principalTable: "WeightedValueTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemDropItemGroup_SocketCountTableId",
                schema: "config",
                table: "ItemDropItemGroup",
                column: "SocketCountTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_SocketCountTableId",
                schema: "config",
                table: "ItemDefinition",
                column: "SocketCountTableId");

            migrationBuilder.CreateIndex(
                name: "IX_DropItemGroup_SocketCountTableId",
                schema: "config",
                table: "DropItemGroup",
                column: "SocketCountTableId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightedValue_WeightedValueTableId",
                schema: "config",
                table: "WeightedValue",
                column: "WeightedValueTableId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightedValueTable_GameConfigurationId",
                schema: "config",
                table: "WeightedValueTable",
                column: "GameConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DropItemGroup_WeightedValueTable_SocketCountTableId",
                schema: "config",
                table: "DropItemGroup",
                column: "SocketCountTableId",
                principalSchema: "config",
                principalTable: "WeightedValueTable",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDefinition_WeightedValueTable_SocketCountTableId",
                schema: "config",
                table: "ItemDefinition",
                column: "SocketCountTableId",
                principalSchema: "config",
                principalTable: "WeightedValueTable",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDropItemGroup_WeightedValueTable_SocketCountTableId",
                schema: "config",
                table: "ItemDropItemGroup",
                column: "SocketCountTableId",
                principalSchema: "config",
                principalTable: "WeightedValueTable",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DropItemGroup_WeightedValueTable_SocketCountTableId",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemDefinition_WeightedValueTable_SocketCountTableId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemDropItemGroup_WeightedValueTable_SocketCountTableId",
                schema: "config",
                table: "ItemDropItemGroup");

            migrationBuilder.DropTable(
                name: "WeightedValue",
                schema: "config");

            migrationBuilder.DropTable(
                name: "WeightedValueTable",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_ItemDropItemGroup_SocketCountTableId",
                schema: "config",
                table: "ItemDropItemGroup");

            migrationBuilder.DropIndex(
                name: "IX_ItemDefinition_SocketCountTableId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropIndex(
                name: "IX_DropItemGroup_SocketCountTableId",
                schema: "config",
                table: "DropItemGroup");

            migrationBuilder.DropColumn(
                name: "SocketCountTableId",
                schema: "config",
                table: "ItemDropItemGroup");

            migrationBuilder.DropColumn(
                name: "SocketCountTableId",
                schema: "config",
                table: "ItemDefinition");

            migrationBuilder.DropColumn(
                name: "SocketCountTableId",
                schema: "config",
                table: "DropItemGroup");
        }
    }
}
