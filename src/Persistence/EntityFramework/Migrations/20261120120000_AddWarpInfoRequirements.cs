using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <summary>
    /// Adds the attribute requirements of warps, e.g. a minimum master level or number of resets.
    /// Existing warps get no requirements.
    /// </summary>
    public partial class AddWarpInfoRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WarpInfoId",
                schema: "config",
                table: "AttributeRequirement",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRequirement_WarpInfoId",
                schema: "config",
                table: "AttributeRequirement",
                column: "WarpInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_WarpInfo_WarpInfoId",
                schema: "config",
                table: "AttributeRequirement",
                column: "WarpInfoId",
                principalSchema: "config",
                principalTable: "WarpInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeRequirement_WarpInfo_WarpInfoId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropIndex(
                name: "IX_AttributeRequirement_WarpInfoId",
                schema: "config",
                table: "AttributeRequirement");

            migrationBuilder.DropColumn(
                name: "WarpInfoId",
                schema: "config",
                table: "AttributeRequirement");
        }
    }
}
