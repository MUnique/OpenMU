// <copyright file="AdminOperationsExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using Microsoft.Extensions.DependencyInjection;

/// <summary>Registers UI independent admin read operations.</summary>
public static class AdminOperationsExtensions
{
    /// <summary>Adds the read operations to the application.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same collection for further registration.</returns>
    public static IServiceCollection AddAdminReadOperations(this IServiceCollection services)
    {
        services.AddScoped<AccountReadOperations>();
        services.AddScoped<ServerReadOperations>();
        services.AddScoped<GuildReadOperations>();
        services.AddScoped<ConfigurationReadOperations>();
        services.AddScoped<PluginReadOperations>();
        return services;
    }
}
