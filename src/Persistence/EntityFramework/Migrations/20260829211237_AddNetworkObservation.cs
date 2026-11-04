using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddNetworkObservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NetworkAnalyzerLiveBufferSize",
                schema: "config",
                table: "SystemConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 5000);

            migrationBuilder.AddColumn<string>(
                name: "NetworkObservationArchivePath",
                schema: "config",
                table: "SystemConfiguration",
                type: "text",
                nullable: true,
                defaultValue: "captures");

            migrationBuilder.AddColumn<int>(
                name: "NetworkObservationMaxSessionSizeMb",
                schema: "config",
                table: "SystemConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 50);

            migrationBuilder.AddColumn<int>(
                name: "NetworkObservationMaxTotalSizeMb",
                schema: "config",
                table: "SystemConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 1000);

            migrationBuilder.AddColumn<int>(
                name: "NetworkObservationRetentionDays",
                schema: "config",
                table: "SystemConfiguration",
                type: "integer",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<bool>(
                name: "IsNetworkObservationActive",
                schema: "data",
                table: "Account",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetworkAnalyzerLiveBufferSize",
                schema: "config",
                table: "SystemConfiguration");

            migrationBuilder.DropColumn(
                name: "NetworkObservationArchivePath",
                schema: "config",
                table: "SystemConfiguration");

            migrationBuilder.DropColumn(
                name: "NetworkObservationMaxSessionSizeMb",
                schema: "config",
                table: "SystemConfiguration");

            migrationBuilder.DropColumn(
                name: "NetworkObservationMaxTotalSizeMb",
                schema: "config",
                table: "SystemConfiguration");

            migrationBuilder.DropColumn(
                name: "NetworkObservationRetentionDays",
                schema: "config",
                table: "SystemConfiguration");

            migrationBuilder.DropColumn(
                name: "IsNetworkObservationActive",
                schema: "data",
                table: "Account");
        }
    }
}
