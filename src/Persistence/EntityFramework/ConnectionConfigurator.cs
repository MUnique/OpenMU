// <copyright file="ConnectionConfigurator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// The database roles.
/// </summary>
public enum DatabaseRole
{
    /// <summary>
    /// The admin role which can create databases and other roles.
    /// </summary>
    Admin,

    /// <summary>
    /// The account role which can load and save account data.
    /// </summary>
    Account,

    /// <summary>
    /// The role which can load configuration data.
    /// </summary>
    Configuration,

    /// <summary>
    /// The role which can load guild data.
    /// </summary>
    Guild,

    /// <summary>
    /// The role which can load friend data.
    /// </summary>
    Friend,
}

/// <summary>
/// The database connection configurator which loads the configuration from a file.
/// TODO: Make class non-static.
/// </summary>
public static class ConnectionConfigurator
{
    private static IDatabaseConnectionSettingProvider? _provider;

    /// <summary>
    /// Gets a value indicating whether this instance is initialized.
    /// </summary>
    public static bool IsInitialized => _provider is not null;

    private static IDatabaseConnectionSettingProvider Provider => _provider ?? throw new InvalidOperationException("Call Initialize before.");

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="provider">The <see cref="IDatabaseConnectionSettingProvider"/> which provides the required connection settings.</param>
    public static void Initialize(IDatabaseConnectionSettingProvider provider)
    {
        if (_provider is not null)
        {
            throw new InvalidOperationException("provider is initialized already");
        }

        _provider = provider;
    }

    /// <summary>
    /// Gets the name of the role from the configured connection string.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>The name of the role from the configured connection string.</returns>
    public static string GetRoleName(DatabaseRole role)
    {
        Provider.Initialization?.WaitWithoutException();
        var settings = Provider.GetConnectionSetting(GetContextTypeOfRole(role));
        return Regex.Match(settings.ConnectionString!, "User Id=([^;]+?);").Groups[1].Value;
    }

    /// <summary>
    /// Gets the password password of the role from the configured connection string.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>The password password of the role from the configured connection string.</returns>
    public static string GetRolePassword(DatabaseRole role)
    {
        Provider.Initialization?.WaitWithoutException();
        var settings = Provider.GetConnectionSetting(GetContextTypeOfRole(role));
        return Regex.Match(settings.ConnectionString!, "Password=([^;]+?);").Groups[1].Value;
    }

    /// <summary>
    /// Configures the specified options builder.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="optionsBuilder">The options builder.</param>
    /// <exception cref="NotImplementedException">At the moment only Npgsql engine (PostgreSQL) is implemented.</exception>
    internal static void Configure(this DbContext context, DbContextOptionsBuilder optionsBuilder)
    {
        // see https://github.com/dotnet/efcore/issues/34431
        optionsBuilder.ConfigureWarnings(a => a.Ignore(RelationalEventId.PendingModelChangesWarning));

        var type = context.GetType();
        if (type.IsGenericType)
        {
            type = type.GetGenericTypeDefinition();
        }

        Provider.Initialization?.WaitWithoutException();
        if (Provider.GetConnectionSetting(type) is { } setting)
        {
            switch (setting.DatabaseEngine)
            {
                case DatabaseEngine.Npgsql:
                    optionsBuilder.UseNpgsql(setting.ConnectionString!);
                    optionsBuilder.ReplaceService<IMigrationsSqlGenerator, MyNpgsqlMigrationsSqlGenerator>();
                    optionsBuilder.ReplaceService<IModelCacheKeyFactory, MyModelCacheKeyFactory>();
                    return;
                default:
                    throw new NotImplementedException("At the moment only Npgsql engine (PostgreSQL) is implemented.");
            }
        }

        throw new ArgumentException($"No configuration found for context type {context.GetType()}", nameof(context));
    }

    private static Type GetContextTypeOfRole(DatabaseRole role)
    {
        return role switch
        {
            DatabaseRole.Account => typeof(AccountContext),
            DatabaseRole.Admin => typeof(EntityDataContext),
            DatabaseRole.Configuration => typeof(ConfigurationContext),
            DatabaseRole.Guild => typeof(GuildContext),
            DatabaseRole.Friend => typeof(FriendContext),
            _ => throw new ArgumentException($"Role {role} unknown."),
        };
    }
}