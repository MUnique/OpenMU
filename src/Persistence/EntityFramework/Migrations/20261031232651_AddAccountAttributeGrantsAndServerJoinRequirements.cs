using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountAttributeGrantsAndServerJoinRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameServerDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountAttributeGrant",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<float>(type: "real", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAttributeGrant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountAttributeGrant_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "data",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountAttributeGrant_AttributeDefinition_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRequirement_GameServerDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "GameServerDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAttributeGrant_AccountId",
                schema: "data",
                table: "AccountAttributeGrant",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAttributeGrant_AttributeId",
                schema: "data",
                table: "AccountAttributeGrant",
                column: "AttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_GameServerDefinition_GameServerDefinit~",
                schema: "config",
                table: "AttributeRequirement",
                column: "GameServerDefinitionId",
                principalSchema: "config",
                principalTable: "GameServerDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_GameServerDefinition_GameServerDefinit~",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropTable(
                name: "AccountAttributeGrant",
                schema: "data");

            migrationBuilder.DropIndex(
                name: "IX_AttributeRequirement_GameServerDefinitionId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropColumn(
                name: "GameServerDefinitionId",
                schema: "config",
                table: "AttributeRequirement");
        }
    }
}
