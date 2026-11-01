// <copyright file="AdminMcpExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Mcp;

using System.Globalization;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Registers and maps the opt-in MCP endpoint in either admin host.</summary>
public static class AdminMcpExtensions
{
    private const string RateLimitPolicy = "OpenMU.Mcp";

    /// <summary>Registers MCP when explicitly enabled by configuration.</summary>
    /// <param name="services">The service collection, including admin authentication and read operations.</param>
    /// <param name="configuration">The host configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAdminMcp(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new AdminMcpOptions();
        configuration.GetSection("AdminPanel:Mcp").Bind(options);
        if (bool.TryParse(Environment.GetEnvironmentVariable("OPENMU_MCP_ENABLED"), out var enabled))
        {
            options.Enabled = enabled;
        }

        services.AddSingleton(options);
        services.AddSingleton<AdminWriteSettings>(options);
        if (!options.Enabled)
        {
            return services;
        }

        if (options.RequestsPerMinute < 1 || options.MaxRequestBodyBytes < 1 || options.MaxBulkSize is < 1 or > 1000)
        {
            throw new ArgumentException("MCP request and size limits must be positive.", nameof(configuration));
        }

        services.AddAuthorizationBuilder().AddPolicy(AdminPolicies.McpAccess, policy => policy
            .AddAuthenticationSchemes(ApiKeyAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireAssertion(context => context.User.IsInRole(AdminRoles.Viewer) || context.User.IsInRole(AdminRoles.CashShop)));
        services.AddRateLimiter(rateLimiter =>
        {
            rateLimiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimiter.OnRejected = (context, _) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                }

                return ValueTask.CompletedTask;
            };
            rateLimiter.AddPolicy(RateLimitPolicy, context => RateLimitPartition.GetFixedWindowLimiter(
                context.User.FindFirst(ApiKeyAuthenticationDefaults.ClientIdClaimType)?.Value ?? "anonymous",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = options.RequestsPerMinute, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
        });
        var server = services.AddMcpServer()
            .WithHttpTransport(transport => transport.Stateless = true)
            .WithTools<AdminReadTools>()
            .WithTools<AdminOperationReadTools>()
            .WithTools<ConfigurationReadTools>()
            .WithTools<LiveGameReadTools>()
            .WithTools<PluginOperationReadTools>();
        if (!options.ReadOnly)
        {
            server.WithTools<AdminWriteTools>().WithTools<ConfigurationWriteTools>().WithTools<LiveGameWriteTools>().WithTools<PluginOperationWriteTools>();
        }

        server.WithRequestFilters(filters =>
        {
            filters.AddListToolsFilter(next => async (context, cancellationToken) =>
            {
                var result = await next(context, cancellationToken).ConfigureAwait(false);
                var groups = context.User?.FindFirst(AdminToolGroups.ClaimType)?.Value;
                return new ListToolsResult { Tools = result.Tools.Where(t => AdminToolGroups.Allows(groups, t.Name)).ToArray(), NextCursor = result.NextCursor };
            });
            filters.AddCallToolFilter(next => async (context, cancellationToken) =>
            {
                var groups = context.User?.FindFirst(AdminToolGroups.ClaimType)?.Value;
                if (context.Params?.Name is not { } name || !AdminToolGroups.Allows(groups, name))
                {
                    throw new McpException("This tool is excluded by the API key's allowlist.");
                }

                try
                {
                    return await next(context, cancellationToken).ConfigureAwait(false);
                }
                catch (McpException ex) when (ex.InnerException is AdminOperationException or ArgumentException or ConfigurationReadConflictException)
                {
                    var kind = ex.InnerException is AdminOperationException operationError ? operationError.Kind
                        : ex.InnerException is ConfigurationReadConflictException ? AdminOperationErrorKind.Conflict : AdminOperationErrorKind.InvalidInput;
                    return new CallToolResult
                    {
                        IsError = true,
                        Content = [new TextContentBlock { Text = $"An error occurred invoking '{name}': {ex.Message}" }],
                        Meta = new JsonObject { ["openmu/errorKind"] = kind.ToString() },
                    };
                }
            });
        });
        server.AddAuthorizationFilters();
        return services;
    }

    /// <summary>Checks MCP transport security and request size before admin authentication.</summary>
    /// <param name="app">The application.</param>
    /// <returns>The same application for further endpoint mapping.</returns>
    public static WebApplication UseAdminMcpProtection(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<AdminMcpOptions>();
        if (!options.Enabled)
        {
            return app;
        }

        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/mcp"))
            {
                // A direct, non-TLS deployment must only accept local clients. Hosts behind a
                // TLS proxy must configure ASP.NET Core's trusted forwarded headers as usual.
                if (!context.Request.IsHttps && (context.Connection.RemoteIpAddress is not { } address || !IPAddress.IsLoopback(address)))
                {
                    app.Logger.LogWarning("Rejected a non-local MCP request over HTTP. Configure HTTPS or trusted forwarded headers.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }

                if (context.Request.ContentLength > options.MaxRequestBodyBytes)
                {
                    context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                    return;
                }

                if (context.Features.Get<IHttpMaxRequestBodySizeFeature>() is { IsReadOnly: false } bodyLimit)
                {
                    bodyLimit.MaxRequestBodySize = options.MaxRequestBodyBytes;
                }
            }

            await next(context).ConfigureAwait(false);
        });
        return app;
    }

    /// <summary>Maps the MCP endpoint after routing and admin authentication.</summary>
    /// <param name="app">The application.</param>
    /// <returns>The same application for further endpoint mapping.</returns>
    public static WebApplication MapAdminMcp(this WebApplication app)
    {
        if (!app.Services.GetRequiredService<AdminMcpOptions>().Enabled)
        {
            return app;
        }

        app.UseRateLimiter();
        app.MapMcp("/mcp").RequireAuthorization(AdminPolicies.McpAccess).RequireRateLimiting(RateLimitPolicy);
        return app;
    }
}
