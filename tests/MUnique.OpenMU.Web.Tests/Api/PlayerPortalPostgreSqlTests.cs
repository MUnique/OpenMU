// <copyright file="PlayerPortalPostgreSqlTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.PlayerPortal.Api;
using Npgsql;

/// <summary>Checks the portal's narrow owner query with a PostgreSQL role that cannot read unlocked classes.</summary>
/// <remarks>Run alone with OPENMU_TEST_PLAYER_PORTAL_POSTGRES set to an empty disposable PostgreSQL database.</remarks>
[TestFixture]
[Explicit("Requires an empty disposable PostgreSQL database and a separate test process.")]
[NonParallelizable]
public sealed class PlayerPortalPostgreSqlTests
{
    /// <summary>Registration, native login, and owner-scoped account reads work with minimum portal table grants.</summary>
    [Test]
    public async Task RegistrationLoginAndMeUseOnlyGrantedOwnerDataAsync()
    {
        var adminConnection = Environment.GetEnvironmentVariable("OPENMU_TEST_PLAYER_PORTAL_POSTGRES");
        Assert.That(adminConnection, Is.Not.Null.And.Not.Empty, "Set OPENMU_TEST_PLAYER_PORTAL_POSTGRES to an empty disposable database.");

        var accountRole = GetRoleName("OPENMU_TEST_PLAYER_PORTAL_ACCOUNT_ROLE", $"portal_account_{Guid.NewGuid():N}");
        var configurationRole = GetRoleName("OPENMU_TEST_PLAYER_PORTAL_CONFIGURATION_ROLE", $"portal_config_{Guid.NewGuid():N}");
        var accountPassword = Environment.GetEnvironmentVariable("OPENMU_TEST_PLAYER_PORTAL_ACCOUNT_PASSWORD") ?? string.Empty;
        var configurationPassword = Environment.GetEnvironmentVariable("OPENMU_TEST_PLAYER_PORTAL_CONFIGURATION_PASSWORD") ?? string.Empty;
        var keepRoles = string.Equals(Environment.GetEnvironmentVariable("OPENMU_TEST_PLAYER_PORTAL_KEEP_ROLES"), "true", StringComparison.OrdinalIgnoreCase);
        var requiredAdminConnection = adminConnection!;
        var connectionParts = new NpgsqlConnectionStringBuilder(requiredAdminConnection);
        var adminSettings = $"Host={connectionParts.Host};Port={connectionParts.Port};Database={connectionParts.Database};User Id={connectionParts.Username};Password={connectionParts.Password};";
        var accountConnection = BuildConnectionString(connectionParts, accountRole, accountPassword);
        var configurationConnection = BuildConnectionString(connectionParts, configurationRole, configurationPassword);
        try
        {
            await using (var admin = new NpgsqlConnection(requiredAdminConnection))
            {
                await admin.OpenAsync().ConfigureAwait(false);
                await CreateRoleAsync(admin, accountRole, accountPassword).ConfigureAwait(false);
                await CreateRoleAsync(admin, configurationRole, configurationPassword).ConfigureAwait(false);
            }

            ConnectionConfigurator.Initialize(new PortalDatabaseSettings(adminSettings, accountConnection, configurationConnection));
            await using (var schema = new EntityDataContext())
            {
                await schema.GetService<IRelationalDatabaseCreator>().CreateTablesAsync().ConfigureAwait(false);
            }

            await using (var admin = new NpgsqlConnection(requiredAdminConnection))
            {
                await admin.OpenAsync().ConfigureAwait(false);
                var grants = $"REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA data FROM {QuoteIdentifier(accountRole)}; ALTER DEFAULT PRIVILEGES IN SCHEMA data REVOKE ALL ON TABLES FROM {QuoteIdentifier(accountRole)}; REVOKE ALL ON SCHEMA data FROM {QuoteIdentifier(accountRole)}; GRANT USAGE ON SCHEMA data TO {QuoteIdentifier(accountRole)}; GRANT SELECT, INSERT ON TABLE data.\"Account\" TO {QuoteIdentifier(accountRole)}; GRANT SELECT ON TABLE data.\"Character\", data.\"StatAttribute\" TO {QuoteIdentifier(accountRole)}; REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA config FROM {QuoteIdentifier(configurationRole)}; REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA data FROM {QuoteIdentifier(configurationRole)}; ALTER DEFAULT PRIVILEGES IN SCHEMA config REVOKE ALL ON TABLES FROM {QuoteIdentifier(configurationRole)}; ALTER DEFAULT PRIVILEGES IN SCHEMA data REVOKE ALL ON TABLES FROM {QuoteIdentifier(configurationRole)}; REVOKE ALL ON SCHEMA config, data FROM {QuoteIdentifier(configurationRole)}; GRANT USAGE ON SCHEMA config, data TO {QuoteIdentifier(configurationRole)}; GRANT SELECT ON ALL TABLES IN SCHEMA config TO {QuoteIdentifier(configurationRole)}; GRANT SELECT ON TABLE data.\"Item\", data.\"ItemOptionLink\", data.\"ItemItemOfItemSet\", data.\"ItemStorage\" TO {QuoteIdentifier(configurationRole)};";
                await using var command = new NpgsqlCommand(grants, admin);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                await using var check = new NpgsqlCommand("SELECT has_table_privilege(@accountRole, 'data.\"Account\"', 'SELECT'), has_table_privilege(@accountRole, 'data.\"Account\"', 'INSERT'), has_table_privilege(@accountRole, 'data.\"Account\"', 'UPDATE'), has_table_privilege(@accountRole, 'data.\"Account\"', 'DELETE'), has_table_privilege(@accountRole, 'data.\"AccountCharacterClass\"', 'SELECT'), has_table_privilege(@configurationRole, 'config.\"GameConfiguration\"', 'SELECT'), has_table_privilege(@configurationRole, 'config.\"GameConfiguration\"', 'UPDATE'), has_table_privilege(@configurationRole, 'data.\"Account\"', 'SELECT')", admin);
                check.Parameters.AddWithValue("accountRole", accountRole);
                check.Parameters.AddWithValue("configurationRole", configurationRole);
                await using var reader = await check.ExecuteReaderAsync().ConfigureAwait(false);
                Assert.That(await reader.ReadAsync().ConfigureAwait(false), Is.True);
                Assert.That(reader.GetBoolean(0), Is.True);
                Assert.That(reader.GetBoolean(1), Is.True);
                Assert.That(reader.GetBoolean(2), Is.False);
                Assert.That(reader.GetBoolean(3), Is.False);
                Assert.That(reader.GetBoolean(4), Is.False);
                Assert.That(reader.GetBoolean(5), Is.True);
                Assert.That(reader.GetBoolean(6), Is.False);
                Assert.That(reader.GetBoolean(7), Is.False);
            }

            JsonConverterRegistry.RegisterConverter(new LocalizedStringJsonConverter());
            JsonConverterRegistry.RegisterConverter(new BinaryAsHexJsonConverter());
            var persistence = new PersistenceContextProvider(NullLoggerFactory.Instance, null);
            using (var setup = persistence.CreateNewContext())
            {
                _ = setup.CreateNew<GameConfiguration>();
                Assert.That(await setup.SaveChangesAsync().ConfigureAwait(false), Is.True);
            }

            var configurations = new Mock<IDataSource<GameConfiguration>>();
            configurations.Setup(source => source.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MUnique.OpenMU.Persistence.BasicModel.GameConfiguration());
            var configurationSource = configurations.Object;
            var registrations = new AccountRegistrationService(persistence, configurationSource);
            var sessions = new PlayerSessionStore(TimeProvider.System);
            var service = new PlayerPortalService(persistence, configurationSource, registrations, sessions, new PlayerLoginAttemptLimiter(TimeProvider.System));

            Assert.That(await service.RegisterAsync(new AccountRegistration { LoginName = "portalone", Password = "GamePass1" }).ConfigureAwait(false), Is.EqualTo(AccountRegistrationStatus.Created));
            Assert.That(await service.RegisterAsync(new AccountRegistration { LoginName = "portaltwo", Password = "GamePass2" }).ConfigureAwait(false), Is.EqualTo(AccountRegistrationStatus.Created));

            var first = await service.LoginAsync(new PlayerLoginRequest { LoginName = "portalone", Password = "GamePass1" }).ConfigureAwait(false);
            var second = await service.LoginAsync(new PlayerLoginRequest { LoginName = "portaltwo", Password = "GamePass2" }).ConfigureAwait(false);
            Assert.That(first.Status, Is.EqualTo(PlayerLoginStatus.Authenticated));
            Assert.That(second.Status, Is.EqualTo(PlayerLoginStatus.Authenticated));

            var firstMe = await RequestMeAsync(service, sessions, first.Session!.Token, "portaltwo").ConfigureAwait(false);
            var secondMe = await RequestMeAsync(service, sessions, second.Session!.Token, "portalone").ConfigureAwait(false);
            Assert.That(firstMe.LoginName, Is.EqualTo("portalone"));
            Assert.That(secondMe.LoginName, Is.EqualTo("portaltwo"));

            await using var verify = new NpgsqlConnection(requiredAdminConnection);
            await verify.OpenAsync().ConfigureAwait(false);
            await using var rows = new NpgsqlCommand("SELECT count(*) FROM data.\"Account\" WHERE \"LoginName\" IN ('portalone', 'portaltwo')", verify);
            Assert.That(await rows.ExecuteScalarAsync().ConfigureAwait(false), Is.EqualTo(2L));
        }
        finally
        {
            if (!keepRoles)
            {
                await using var admin = new NpgsqlConnection(requiredAdminConnection);
                await admin.OpenAsync().ConfigureAwait(false);
                await using var cleanup = new NpgsqlCommand($"DROP OWNED BY {QuoteIdentifier(accountRole)}; DROP OWNED BY {QuoteIdentifier(configurationRole)}; DROP ROLE IF EXISTS {QuoteIdentifier(accountRole)}; DROP ROLE IF EXISTS {QuoteIdentifier(configurationRole)}", admin);
                await cleanup.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }

    private static string GetRoleName(string variableName, string fallback)
    {
        var roleName = Environment.GetEnvironmentVariable(variableName) ?? fallback;
        Assert.That(roleName, Does.Match("^[a-z_][a-z0-9_]*$"), $"{variableName} must be a simple PostgreSQL role name.");
        return roleName;
    }

    private static string BuildConnectionString(NpgsqlConnectionStringBuilder parts, string role, string password)
        => $"Host={parts.Host};Port={parts.Port};Database={parts.Database};User Id={role};Password={password};";

    private static async Task CreateRoleAsync(NpgsqlConnection admin, string role, string password)
    {
        var passwordClause = password.Length == 0 ? string.Empty : $" PASSWORD '{password.Replace("'", "''", StringComparison.Ordinal)}'";
        await using var command = new NpgsqlCommand($"CREATE ROLE {QuoteIdentifier(role)} LOGIN{passwordClause}", admin);
        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    private static string QuoteIdentifier(string identifier) => $"\"{identifier.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";

    private static async Task<PlayerAccountSnapshot> RequestMeAsync(PlayerPortalService service, PlayerSessionStore sessions, string token, string tamperedLoginName)
    {
        var controller = new PlayerPortalController(service, sessions)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
        controller.Request.Headers[PlayerPortalApiDefaults.SessionHeaderName] = token;
        controller.Request.QueryString = new QueryString($"?loginName={tamperedLoginName}");
        var result = await controller.GetOwnAccountAsync(CancellationToken.None).ConfigureAwait(false);
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        return (PlayerAccountSnapshot)((OkObjectResult)result).Value!;
    }

    private sealed class PortalDatabaseSettings(string adminConnection, string accountConnection, string configurationConnection) : IDatabaseConnectionSettingProvider
    {
        public Task? Initialization => null;

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public ConnectionSetting GetConnectionSetting<TContextType>()
            where TContextType : DbContext => this.GetConnectionSetting(typeof(TContextType));

        public ConnectionSetting GetConnectionSetting(Type contextType)
        {
            var connection = contextType.FullName switch
            {
                "MUnique.OpenMU.Persistence.EntityFramework.AccountContext" => accountConnection,
                "MUnique.OpenMU.Persistence.EntityFramework.ConfigurationContext" => configurationConnection,
                _ => adminConnection,
            };
            return new ConnectionSetting { DatabaseEngine = DatabaseEngine.Npgsql, ConnectionString = connection };
        }
    }
}
