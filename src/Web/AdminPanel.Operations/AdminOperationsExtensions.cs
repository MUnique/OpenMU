// <copyright file="AdminOperationsExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>Registers UI independent admin read operations.</summary>
public static class AdminOperationsExtensions
{
    /// <summary>Adds the read operations to the application.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same collection for further registration.</returns>
    public static IServiceCollection AddAdminReadOperations(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<AdminChangeConfirmation>();
        services.AddScoped<ConfigurationWriteOperations>();
        services.AddScoped<PluginWriteOperations>();
        services.AddScoped<ContentActivationOperations>();
        services.AddScoped<ConfigurationTaskOperations>();
        services.AddScoped<CharacterWriteOperations>();
        services.AddScoped<AccountReadOperations>();
        services.AddScoped<ServerReadOperations>();
        services.AddScoped<GuildReadOperations>();
        services.AddScoped<ConfigurationReadOperations>();
        services.AddScoped<PluginReadOperations>();
        services.AddScoped<MiniGameReadOperations>();
        services.TryAddScoped<IAdminOperationContext, UnconfiguredAdminOperationContext>();
        services.TryAddSingleton<IAdminAuditRepository, UnconfiguredAdminAuditRepository>();
        services.TryAddSingleton<AdminWriteSettings>();
        services.TryAddSingleton<AdminAuditSettings>();
        services.TryAddSingleton<AdminLogAccess>();
        services.TryAddSingleton(new LoggingConfigurationService(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));
        services.AddScoped<AdminOperationExecutor>();
        services.AddScoped<ServerWriteOperations>();
        services.AddScoped<PlayerWriteOperations>();
        services.AddScoped<LiveGameOperations>();
        services.AddScoped<PluginAdminOperations>();
        services.AddScoped<AccountWriteOperations>();
        services.AddScoped<CashShopOperations>();
        services.AddScoped<CastleSiegeOperations>();
        services.AddScoped<LogOperations>();
        services.AddScoped<AuditReadOperations>();
        services.AddScoped<CashShopCoinService>();
        services.AddScoped<CastleSiegeManagementService>();
        return services;
    }
}
