using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <summary>
    /// Adds the date when an account was last seen in the game. The existing accounts start with the time of the
    /// migration, so nobody counts as inactive before the configured time has passed after the update.
    /// The guild server gets the permission to read it.
    /// </summary>
    public partial class AddAccountLastSeenDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeenDate",
                schema: "data",
                table: "Account",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            // The guild server reads the date to find inactive guild masters. Just these two columns are granted,
            // so that it can't read the credentials of the accounts.
            migrationBuilder.Sql($"GRANT SELECT (\"Id\", \"LastSeenDate\") ON TABLE data.\"Account\" TO GROUP {ConnectionConfigurator.GetRoleName(DatabaseRole.Guild)};");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"REVOKE SELECT (\"Id\", \"LastSeenDate\") ON TABLE data.\"Account\" FROM GROUP {ConnectionConfigurator.GetRoleName(DatabaseRole.Guild)};");

            migrationBuilder.DropColumn(
                name: "LastSeenDate",
                schema: "data",
                table: "Account");
        }
    }
}
