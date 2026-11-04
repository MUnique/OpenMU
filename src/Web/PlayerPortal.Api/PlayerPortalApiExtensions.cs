// <copyright file="PlayerPortalApiExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Registers the isolated player portal API and its Worker-only authentication policy.</summary>
public static class PlayerPortalApiExtensions
{
    /// <summary>Registers API services. Startup fails closed when the Worker credential is absent or weak.</summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="configuration">Configuration containing the private Worker credential.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPlayerPortalApi(this IServiceCollection services, IConfiguration configuration)
    {
        var workerToken = configuration[PlayerPortalApiDefaults.WorkerTokenConfigurationKey];
        if (workerToken is null || Encoding.UTF8.GetByteCount(workerToken) < 32)
        {
            throw new InvalidOperationException($"{PlayerPortalApiDefaults.WorkerTokenConfigurationKey} must contain at least 32 UTF-8 bytes.");
        }

        services.AddSingleton(new PlayerPortalWorkerCredential(workerToken));
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<PlayerSessionStore>();
        services.AddSingleton<PlayerLoginAttemptLimiter>();
        services.AddScoped<AccountRegistrationService>();
        services.AddScoped<PlayerPortalService>();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = PlayerPortalApiDefaults.WorkerAuthenticationScheme;
                options.DefaultChallengeScheme = PlayerPortalApiDefaults.WorkerAuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, PlayerPortalWorkerAuthenticationHandler>(PlayerPortalApiDefaults.WorkerAuthenticationScheme, _ => { });
        services.AddAuthorization(options => options.AddPolicy(
            PlayerPortalApiDefaults.WorkerAuthorizationPolicy,
            policy =>
            {
                policy.AddAuthenticationSchemes(PlayerPortalApiDefaults.WorkerAuthenticationScheme);
                policy.RequireAuthenticatedUser();
            }));

        return services;
    }
}
