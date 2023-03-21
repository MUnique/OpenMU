using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddItemDropDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ItemDropDuration",
                schema: "config",
                table: "GameConfiguration",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 1, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemDropDuration",
                schema: "config",
                table: "GameConfiguration");
        }
    }
}
