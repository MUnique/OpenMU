// <copyright file="ToastServiceCollectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Extension methods for registering the toast service.
/// </summary>
public static class ToastServiceCollectionExtensions
{
    /// <summary>
    /// Adds the toast service to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection, for chaining.</returns>
    public static IServiceCollection AddToasts(this IServiceCollection services)
    {
        services.AddScoped<ToastService>();
        services.AddScoped<IToastService>(sp => sp.GetRequiredService<ToastService>());
        return services;
    }
}