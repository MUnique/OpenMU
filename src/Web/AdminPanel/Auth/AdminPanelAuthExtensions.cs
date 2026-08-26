// <copyright file="AdminPanelAuthExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Extensions which add the authentication of the admin panel.
/// </summary>
public static class AdminPanelAuthExtensions
{
    /// <summary>
    /// The environment variable which defines the login name of the bootstrap user.
    /// </summary>
    public const string BootstrapUserVariableName = "OPENMU_ADMIN_USER";

    /// <summary>
    /// The environment variable which defines the password of the bootstrap user.
    /// </summary>
    public const string BootstrapPasswordVariableName = "OPENMU_ADMIN_PASSWORD";

    /// <summary>
    /// The environment variable which defines the base32 authenticator key of the bootstrap user.
    /// </summary>
    public const string BootstrapAuthenticatorKeyVariableName = "OPENMU_ADMIN_TOTP_SECRET";

    /// <summary>
    /// The environment variable which defines an API key for the public API.
    /// </summary>
    public const string ApiKeyVariableName = "OPENMU_API_KEY";

    /// <summary>
    /// The environment variable which defines the roles of the <see cref="ApiKeyVariableName"/>.
    /// </summary>
    public const string ApiKeyRolesVariableName = "OPENMU_API_KEY_ROLES";

    /// <summary>
    /// Adds the authentication of the admin panel to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same instance, to allow chaining of further calls.</returns>
    public static IServiceCollection AddAdminPanelAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = new AdminPanelAuthOptions();
        configuration.GetSection(AdminPanelAuthOptions.SectionName).Bind(authOptions);
        ApplyEnvironmentVariables(authOptions);
        services.Configure<AdminPanelAuthOptions>(options =>
        {
            options.RequireTwoFactor = authOptions.RequireTwoFactor;
            options.SessionTimeout = authOptions.SessionTimeout;
            options.MaxFailedAccessAttempts = authOptions.MaxFailedAccessAttempts;
            options.LockoutDuration = authOptions.LockoutDuration;
            options.BootstrapUser = authOptions.BootstrapUser;
        });

        var apiKeyOptions = new ApiKeyOptions();
        configuration.GetSection(ApiKeyOptions.SectionName).Bind(apiKeyOptions);
        ApplyApiKeyEnvironmentVariable(apiKeyOptions);
        services.Configure<ApiKeyOptions>(options => options.Keys = apiKeyOptions.Keys);
        services.AddSingleton<ApiKeyRegistry>();
        services.AddScoped<ApiKeyManagementService>();

        // The key ring protects the authentication cookies and the authenticator keys. It has to be
        // persisted, otherwise a restart invalidates all sessions and makes all stored authenticator
        // keys unreadable. In docker, the directory should be a mounted volume.
        var keyPath = configuration["AdminPanel:Auth:DataProtectionKeyPath"] ?? "data-protection-keys";
        services.AddDataProtection()
            .SetApplicationName("MUnique.OpenMU.AdminPanel")
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), keyPath)));

        // The hosting application registers the real storage; this is just a fallback which lets
        // the panel start in its initial setup mode instead of failing to resolve its services.
        services.TryAddSingleton<IAdminUserRepository, UnavailableAdminUserRepository>();
        services.TryAddSingleton<IApiKeyRepository, UnavailableApiKeyRepository>();

        services.AddSingleton<AdminUserSecretProtector>();
        services.AddSingleton<IPasswordHasher<AdminUser>, BCryptPasswordHasher>();
        services.AddSingleton<BootstrapAdminUserProvider>();
        services.AddSingleton<SignInTicketService>();
        services.AddSingleton<AdminUserAvailabilityService>();
        services.AddScoped<IUserStore<AdminUser>, AdminUserStore>();
        services.AddScoped<AdminLoginService>();
        services.AddScoped<AuthenticatorSetupService>();
        services.AddScoped<CurrentAdminUserService>();

        services.AddIdentityCore<AdminUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = authOptions.MaxFailedAccessAttempts;
                options.Lockout.DefaultLockoutTimeSpan = authOptions.LockoutDuration;
            })
            .AddDefaultTokenProviders();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = AdminAuthenticationDefaults.CookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;

                // The panel is usually run behind a reverse proxy which terminates TLS.
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.ExpireTimeSpan = authOptions.SessionTimeout;
                options.SlidingExpiration = true;
                options.LoginPath = AdminAuthenticationDefaults.LoginPath;
                options.LogoutPath = AdminAuthenticationDefaults.SignOutEndpointPath;
                options.AccessDeniedPath = AdminAuthenticationDefaults.AccessDeniedPath;

                // An API client can't do anything with the login page, so it gets a status code
                // instead of a redirect to it.
                options.Events.OnRedirectToLogin = context => RespondWithStatusCodeOnApiPath(context, StatusCodes.Status401Unauthorized);
                options.Events.OnRedirectToAccessDenied = context => RespondWithStatusCodeOnApiPath(context, StatusCodes.Status403Forbidden);
            })
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationDefaults.AuthenticationScheme,
                configureOptions: null);

        services.AddSingleton<IAuthorizationHandler, AdminAccessRequirementHandler>();
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder().AddRequirements(new AdminAccessRequirement()).Build())
            .AddPolicy(AdminPolicies.Viewer, policy => policy.AddRequirements(new AdminAccessRequirement(AdminRoles.Viewer)))
            .AddPolicy(AdminPolicies.Operator, policy => policy.AddRequirements(new AdminAccessRequirement(AdminRoles.Operator)))
            .AddPolicy(AdminPolicies.Administrator, policy => policy.AddRequirements(new AdminAccessRequirement(AdminRoles.Administrator)));
        services.AddCascadingAuthenticationState();
        services.AddScoped<AdminAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AdminAuthenticationStateProvider>());
        services.AddScoped<IHostEnvironmentAuthenticationStateProvider>(sp => sp.GetRequiredService<AdminAuthenticationStateProvider>());

        return services;
    }

    /// <summary>
    /// Adds the authentication middlewares to the request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The same instance, to allow chaining of further calls.</returns>
    public static IApplicationBuilder UseAdminPanelAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    /// <summary>
    /// Requires the default authorization policy for all requests below the specified path.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="path">The path, e.g. <c>/logs</c>.</param>
    /// <returns>The same instance, to allow chaining of further calls.</returns>
    /// <remarks>
    /// Static files are served by a middleware and not by an endpoint, so they are not covered by
    /// the authorization of the endpoint routing. The log files must not be readable by anyone.
    /// </remarks>
    public static IApplicationBuilder UseAuthorizedPath(this IApplicationBuilder app, string path)
    {
        return app.Use(async (context, next) =>
        {
            if (!context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase))
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            var policyProvider = context.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>();
            var authorizationService = context.RequestServices.GetRequiredService<IAuthorizationService>();
            var policy = await policyProvider.GetDefaultPolicyAsync().ConfigureAwait(false);
            var result = await authorizationService.AuthorizeAsync(context.User, null, policy).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                await context.ChallengeAsync().ConfigureAwait(false);
                return;
            }

            await next(context).ConfigureAwait(false);
        });
    }

    private static Task RespondWithStatusCodeOnApiPath(RedirectContext<CookieAuthenticationOptions> context, int statusCode)
    {
        if (context.Request.Path.StartsWithSegments(ApiKeyAuthenticationDefaults.ApiPathPrefix, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = statusCode;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    }

    private static void ApplyApiKeyEnvironmentVariable(ApiKeyOptions options)
    {
        var key = Environment.GetEnvironmentVariable(ApiKeyVariableName);
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        options.Keys.Add(new ApiKeyEntry
        {
            Name = ApiKeyVariableName,
            Key = key,
            Roles = Environment.GetEnvironmentVariable(ApiKeyRolesVariableName),
        });
    }

    private static void ApplyEnvironmentVariables(AdminPanelAuthOptions options)
    {
        var loginName = Environment.GetEnvironmentVariable(BootstrapUserVariableName);
        var password = Environment.GetEnvironmentVariable(BootstrapPasswordVariableName);
        if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        options.BootstrapUser = new BootstrapAdminUserOptions
        {
            LoginName = loginName,
            Password = password,
            AuthenticatorKey = Environment.GetEnvironmentVariable(BootstrapAuthenticatorKeyVariableName),
        };
    }
}
