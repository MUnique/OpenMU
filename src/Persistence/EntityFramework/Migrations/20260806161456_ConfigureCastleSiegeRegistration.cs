// <copyright file="20260806161456_ConfigureCastleSiegeRegistration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class ConfigureCastleSiegeRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SignOfLordItemDefinitionId",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SignOfLordItemLevel",
                schema: "config",
                table: "CastleSiegeConfiguration",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)3);

            migrationBuilder.CreateIndex(
                name: "IX_CastleSiegeConfiguration_SignOfLordItemDefinitionId",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "SignOfLordItemDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CastleSiegeConfiguration_ItemDefinition_SignOfLordItemDefin~",
                schema: "config",
                table: "CastleSiegeConfiguration",
                column: "SignOfLordItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CastleSiegeConfiguration_ItemDefinition_SignOfLordItemDefin~",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_CastleSiegeConfiguration_SignOfLordItemDefinitionId",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropColumn(
                name: "SignOfLordItemDefinitionId",
                schema: "config",
                table: "CastleSiegeConfiguration");

            migrationBuilder.DropColumn(
                name: "SignOfLordItemLevel",
                schema: "config",
                table: "CastleSiegeConfiguration");
        }
    }
}
