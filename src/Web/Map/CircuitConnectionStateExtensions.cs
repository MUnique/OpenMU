// <copyright file="CircuitConnectionStateExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions to register the <see cref="CircuitConnectionState"/>.
/// </summary>
public static class CircuitConnectionStateExtensions
{
    /// <summary>
    /// Registers the <see cref="CircuitConnectionState"/> and hooks it up as a <see cref="CircuitHandler"/>.
    /// Both resolve to the same scoped instance, so the live map sees the state of the circuit it's rendered on.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    /// <remarks>
    /// Calling it more than once has no effect, because a second handler would run twice per circuit event.
    /// </remarks>
    public static IServiceCollection AddCircuitConnectionState(this IServiceCollection services)
    {
        if (services.Any(descriptor => descriptor.ServiceType == typeof(CircuitConnectionState)))
        {
            return services;
        }

        services.AddScoped<CircuitConnectionState>();
        services.AddScoped<CircuitHandler>(provider => provider.GetRequiredService<CircuitConnectionState>());
        return services;
    }
}
