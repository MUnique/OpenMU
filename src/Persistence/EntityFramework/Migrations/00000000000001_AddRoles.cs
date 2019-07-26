// <copyright file="00000000000001_AddRoles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Migration which adds the required database roles for <see cref="DatabaseRole.Account"/> and <see cref="DatabaseRole.Configuration"/>.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Migrations.Migration" />
    [DbContext(typeof(EntityDataContext))]
    [Migration("00000000000001_AddRoles")]
    public class AddRoles : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP ROLE IF EXISTS {ConnectionConfigurator.GetRoleName(DatabaseRole.Account)}, {ConnectionConfigurator.GetRoleName(DatabaseRole.Configuration)}, {ConnectionConfigurator.GetRoleName(DatabaseRole.Guild)}, {ConnectionConfigurator.GetRoleName(DatabaseRole.Friend)};");

            var accountRoleName = ConnectionConfigurator.GetRoleName(DatabaseRole.Account);

            migrationBuilder.Sql($"CREATE ROLE {accountRoleName} WITH LOGIN PASSWORD '{ConnectionConfigurator.GetRolePassword(DatabaseRole.Account)}';");

            migrationBuilder.Sql($"GRANT SELECT, UPDATE, INSERT, DELETE ON ALL TABLES IN SCHEMA data TO GROUP {accountRoleName};");
            migrationBuilder.Sql($"ALTER DEFAULT PRIVILEGES IN SCHEMA data GRANT ALL ON TABLES TO {accountRoleName};");

            migrationBuilder.Sql($"GRANT USAGE ON SCHEMA data TO GROUP {accountRoleName};");

            var configRoleName = ConnectionConfigurator.GetRoleName(DatabaseRole.Configuration);

            migrationBuilder.Sql($"CREATE ROLE {configRoleName} WITH LOGIN PASSWORD '{ConnectionConfigurator.GetRolePassword(DatabaseRole.Configuration)}';");

            migrationBuilder.Sql($"GRANT SELECT ON ALL TABLES IN SCHEMA config TO GROUP {configRoleName};");
            migrationBuilder.Sql($"ALTER DEFAULT PRIVILEGES IN SCHEMA config GRANT ALL ON TABLES TO {configRoleName};");
            migrationBuilder.Sql($"GRANT SELECT ON TABLE data.\"Item\", data.\"ItemOptionLink\", data.\"ItemItemSetGroup\", data.\"ItemStorage\" TO GROUP {configRoleName};");

            migrationBuilder.Sql($"GRANT USAGE ON SCHEMA data, config TO GROUP {configRoleName};");

            var guildRoleName = ConnectionConfigurator.GetRoleName(DatabaseRole.Guild);

            migrationBuilder.Sql($"CREATE ROLE {guildRoleName} WITH LOGIN PASSWORD '{ConnectionConfigurator.GetRolePassword(DatabaseRole.Guild)}';");

            migrationBuilder.Sql($"GRANT SELECT, UPDATE, INSERT, DELETE ON ALL TABLES IN SCHEMA guild TO GROUP {guildRoleName};");
            migrationBuilder.Sql($"ALTER DEFAULT PRIVILEGES IN SCHEMA guild GRANT ALL ON TABLES TO {guildRoleName};");
            migrationBuilder.Sql($"GRANT SELECT ON TABLE data.\"Character\" TO GROUP {guildRoleName};");

            migrationBuilder.Sql($"GRANT USAGE ON SCHEMA data, guild TO GROUP {guildRoleName};");

            var friendRoleName = ConnectionConfigurator.GetRoleName(DatabaseRole.Friend);

            migrationBuilder.Sql($"CREATE ROLE {friendRoleName} WITH LOGIN PASSWORD '{ConnectionConfigurator.GetRolePassword(DatabaseRole.Friend)}';");

            migrationBuilder.Sql($"GRANT SELECT, UPDATE, INSERT, DELETE ON ALL TABLES IN SCHEMA friend TO GROUP {friendRoleName};");
            migrationBuilder.Sql($"ALTER DEFAULT PRIVILEGES IN SCHEMA friend GRANT ALL ON TABLES TO {friendRoleName};");
            migrationBuilder.Sql($"GRANT SELECT ON TABLE data.\"Character\" TO GROUP {friendRoleName};");

            migrationBuilder.Sql($"GRANT USAGE ON SCHEMA data, friend TO GROUP {friendRoleName};");
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP ROLE IF EXISTS {ConnectionConfigurator.GetRoleName(DatabaseRole.Account)};");
            migrationBuilder.Sql($"DROP ROLE IF EXISTS {ConnectionConfigurator.GetRoleName(DatabaseRole.Configuration)};");
            migrationBuilder.Sql($"DROP ROLE IF EXISTS {ConnectionConfigurator.GetRoleName(DatabaseRole.Guild)};");
            migrationBuilder.Sql($"DROP ROLE IF EXISTS {ConnectionConfigurator.GetRoleName(DatabaseRole.Friend)};");
        }
    }
}
