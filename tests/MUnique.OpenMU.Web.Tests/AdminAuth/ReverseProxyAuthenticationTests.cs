// <copyright file="ReverseProxyAuthenticationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Tests for the <see cref="ReverseProxyAuthenticationService"/>, which lets other web applications
/// behind the same reverse proxy rely on the admin panel login.
/// </summary>
[TestFixture]
public class ReverseProxyAuthenticationTests
{
    private ServiceProvider _serviceProvider = null!;
    private InMemoryAdminUserRepository _repository = null!;
    private ReverseProxyAuthenticationService _service = null!;

    /// <summary>
    /// Sets a fresh service provider up for each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._repository = new InMemoryAdminUserRepository();
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddDataProtection();
        services.AddSingleton<IAdminUserRepository>(this._repository);
        services.AddSingleton(Options.Create(new AdminPanelAuthOptions()));
        services.AddSingleton<AdminUserSecretProtector>();
        services.AddSingleton<IPasswordHasher<AdminUser>, BCryptPasswordHasher>();
        services.AddSingleton<BootstrapAdminUserProvider>();
        services.AddSingleton<AdminUserAvailabilityService>();
        services.AddSingleton<AdminPrincipalValidator>();
        services.AddSingleton<ReverseProxyAuthenticationService>();

        this._serviceProvider = services.BuildServiceProvider();
        this._service = this._serviceProvider.GetRequiredService<ReverseProxyAuthenticationService>();
    }

    /// <summary>
    /// Disposes the service provider.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this._serviceProvider.Dispose();
    }

    /// <summary>
    /// Tests that everybody gets access in the initial setup mode, because the admin panel itself
    /// is reachable without a login until the first user exists.
    /// </summary>
    [Test]
    public async Task SetupModeGrantsAccessAsync()
    {
        var result = await this._service.AuthenticateAsync(new ClaimsPrincipal(new ClaimsIdentity())).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(new ReverseProxyAuthenticationResult(StatusCodes.Status204NoContent, ReverseProxyAuthenticationService.SetupUserName, AdminRoles.Administrator)));
    }

    /// <summary>
    /// Tests that a request without a login is rejected as soon as a user exists.
    /// </summary>
    [Test]
    public async Task AnonymousRequestIsRejectedAsync()
    {
        await this.AddUserAsync("tester", AdminRoles.Administrator).ConfigureAwait(false);

        var result = await this._service.AuthenticateAsync(new ClaimsPrincipal(new ClaimsIdentity())).ConfigureAwait(false);

        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        Assert.That(result.UserName, Is.Null);
    }

    /// <summary>
    /// Tests that a logged in user gets its name and its most privileged role.
    /// </summary>
    /// <param name="assignedRole">The role which is assigned to the user.</param>
    [TestCase("Viewer")]
    [TestCase("Operator")]
    [TestCase("Administrator")]
    public async Task UserGetsItsMostPrivilegedRoleAsync(string assignedRole)
    {
        var user = await this.AddUserAsync("tester", assignedRole).ConfigureAwait(false);

        var result = await this._service.AuthenticateAsync(CreatePrincipal(user)).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(new ReverseProxyAuthenticationResult(StatusCodes.Status204NoContent, "tester", assignedRole)));
    }

    /// <summary>
    /// Tests that the cookie of a disabled user isn't accepted anymore.
    /// </summary>
    [Test]
    public async Task DisabledUserIsRejectedAsync()
    {
        var user = await this.AddUserAsync("tester", AdminRoles.Administrator).ConfigureAwait(false);
        var principal = CreatePrincipal(user);
        user.IsDisabled = true;

        var result = await this._service.AuthenticateAsync(principal).ConfigureAwait(false);

        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
    }

    /// <summary>
    /// Tests that the cookie isn't accepted anymore after the security stamp of the user changed,
    /// e.g. because of a password change.
    /// </summary>
    [Test]
    public async Task ChangedSecurityStampIsRejectedAsync()
    {
        var user = await this.AddUserAsync("tester", AdminRoles.Administrator).ConfigureAwait(false);
        var principal = CreatePrincipal(user);
        user.SecurityStamp = Guid.NewGuid().ToString();

        var result = await this._service.AuthenticateAsync(principal).ConfigureAwait(false);

        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
    }

    /// <summary>
    /// Tests that a logged in user without any role is forbidden.
    /// </summary>
    [Test]
    public async Task UserWithoutRoleIsForbiddenAsync()
    {
        var user = await this.AddUserAsync("tester", string.Empty).ConfigureAwait(false);

        var result = await this._service.AuthenticateAsync(CreatePrincipal(user)).ConfigureAwait(false);

        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
    }

    /// <summary>
    /// Tests that the user id is used as name, when the login name can't be transmitted in a header.
    /// </summary>
    [Test]
    public async Task NonAsciiLoginNameIsReplacedByTheIdAsync()
    {
        var user = await this.AddUserAsync("tëster", AdminRoles.Viewer).ConfigureAwait(false);

        var result = await this._service.AuthenticateAsync(CreatePrincipal(user)).ConfigureAwait(false);

        Assert.That(result.UserName, Is.EqualTo(user.Id.ToString()));
    }

    private static ClaimsPrincipal CreatePrincipal(AdminUser user)
    {
        var identity = new ClaimsIdentity(
            AdminLoginService.CreateClaims(user, false),
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name,
            ClaimTypes.Role);
        return new ClaimsPrincipal(identity);
    }

    private async Task<AdminUser> AddUserAsync(string loginName, string roles)
    {
        var user = new AdminUser
        {
            Id = Guid.NewGuid(),
            LoginName = loginName,
            NormalizedLoginName = loginName.ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString(),
            Roles = roles,
        };

        await this._repository.AddAsync(user).ConfigureAwait(false);
        return user;
    }
}
