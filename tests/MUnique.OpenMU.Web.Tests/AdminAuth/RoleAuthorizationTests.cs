// <copyright file="RoleAuthorizationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.API;

/// <summary>
/// Tests that the roles of the admin panel users are enforced for the pages and paths which
/// change data directly, without an admin operation.
/// </summary>
[TestFixture]
public sealed class RoleAuthorizationTests : System.IDisposable
{
    private const string TestLoginPath = "/test-login";

    private WebApplication? _app;
    private HttpClient? _http;
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Gets the pages which change data directly, with the policy they require.
    /// </summary>
    private static IEnumerable<TestCaseData> RestrictedPages =>
    [
        new TestCaseData(typeof(EditConfig), AdminPolicies.Administrator),
        new TestCaseData(typeof(EditConfigGrid), AdminPolicies.Administrator),
        new TestCaseData(typeof(EditConnectionServer), AdminPolicies.Administrator),
        new TestCaseData(typeof(EditItemDrops), AdminPolicies.Administrator),
        new TestCaseData(typeof(Merchants), AdminPolicies.Administrator),
        new TestCaseData(typeof(EditMap), AdminPolicies.Administrator),
        new TestCaseData(typeof(EditTerrain), AdminPolicies.Administrator),
        new TestCaseData(typeof(CreateGameServerConfig), AdminPolicies.Administrator),
        new TestCaseData(typeof(CreateConnectServerConfig), AdminPolicies.Administrator),
        new TestCaseData(typeof(NetworkAnalyzer), AdminPolicies.Administrator),
        new TestCaseData(typeof(NetworkArchiveController), AdminPolicies.Administrator),
        new TestCaseData(typeof(EditAccount), AdminPolicies.Operator),
        new TestCaseData(typeof(MapPage), AdminPolicies.Operator),
    ];

    /// <inheritdoc />
    public void Dispose()
    {
        this._http?.Dispose();
        this._serviceProvider?.Dispose();
    }

    /// <summary>
    /// Disposes the host which was started by a test.
    /// </summary>
    /// <returns>The task.</returns>
    [TearDown]
    public async Task TearDownAsync()
    {
        this._http?.Dispose();
        this._http = null;
        this._serviceProvider?.Dispose();
        this._serviceProvider = null;
        if (this._app is { } app)
        {
            this._app = null;
            await app.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tests that the pages which change data directly require the expected policy, and not only a logged in user.
    /// </summary>
    /// <param name="pageType">The type of the page.</param>
    /// <param name="expectedPolicy">The expected policy.</param>
    [TestCaseSource(nameof(RestrictedPages))]
    public void PageRequiresPolicy(Type pageType, string expectedPolicy)
    {
        var policies = pageType.GetCustomAttributes<AuthorizeAttribute>(true).Select(a => a.Policy);
        Assert.That(policies, Does.Contain(expectedPolicy));
    }

    /// <summary>
    /// Tests that the guard of the direct save paths only lets users with the required role pass.
    /// </summary>
    /// <param name="assignedRole">The role which is assigned to the user.</param>
    /// <param name="policy">The checked policy.</param>
    /// <returns><c>true</c>, if the user is authorized.</returns>
    [TestCase("Viewer", AdminPolicies.Administrator, ExpectedResult = false)]
    [TestCase("Operator", AdminPolicies.Administrator, ExpectedResult = false)]
    [TestCase("Administrator", AdminPolicies.Administrator, ExpectedResult = true)]
    [TestCase("Viewer", AdminPolicies.Operator, ExpectedResult = false)]
    [TestCase("Operator", AdminPolicies.Operator, ExpectedResult = true)]
    [TestCase("Administrator", AdminPolicies.Operator, ExpectedResult = true)]
    public async Task<bool> PolicyGuardChecksRoleAsync(string assignedRole, string policy)
    {
        var user = CreateUser(assignedRole);
        var guard = await this.CreateGuardAsync(user, CreatePrincipal(user)).ConfigureAwait(false);

        return await guard.IsAuthorizedAsync(policy).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that the guard throws when the user doesn't fulfill the policy.
    /// </summary>
    [Test]
    public async Task PolicyGuardThrowsForMissingRoleAsync()
    {
        var user = CreateUser(AdminRoles.Viewer);
        var guard = await this.CreateGuardAsync(user, CreatePrincipal(user)).ConfigureAwait(false);

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => guard.EnsureAuthorizedAsync(AdminPolicies.Administrator));
    }

    /// <summary>
    /// Tests that the guard lets an anonymous user pass in the initial setup mode, like the pages do.
    /// </summary>
    [Test]
    public async Task PolicyGuardAllowsInitialSetupAsync()
    {
        var guard = await this.CreateGuardAsync(null, new ClaimsPrincipal(new ClaimsIdentity())).ConfigureAwait(false);

        Assert.That(await guard.IsAuthorizedAsync(AdminPolicies.Administrator).ConfigureAwait(false), Is.True);
    }

    /// <summary>
    /// Tests that the static log files require an administrator, like the log files page.
    /// </summary>
    /// <param name="assignedRole">The role which is assigned to the user, or <c>null</c> for an anonymous request.</param>
    /// <returns>The status code of the response.</returns>
    [TestCase(null, ExpectedResult = HttpStatusCode.Found)]
    [TestCase("Viewer", ExpectedResult = HttpStatusCode.Found)]
    [TestCase("Operator", ExpectedResult = HttpStatusCode.Found)]
    [TestCase("Administrator", ExpectedResult = HttpStatusCode.OK)]
    public async Task<HttpStatusCode> LogFilesRequireAdministratorAsync(string? assignedRole)
    {
        var http = await this.StartHostAsync(CreateUser(assignedRole ?? AdminRoles.Viewer)).ConfigureAwait(false);
        if (assignedRole is not null)
        {
            using var login = await http.GetAsync(TestLoginPath).ConfigureAwait(false);
            login.EnsureSuccessStatusCode();
        }

        using var response = await http.GetAsync("/logs/log.txt").ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.Found)
        {
            // An authenticated user is sent to the access denied page, an anonymous one to the login.
            var expectedPath = assignedRole is null ? AdminAuthenticationDefaults.LoginPath : AdminAuthenticationDefaults.AccessDeniedPath;
            Assert.That(response.Headers.Location?.AbsolutePath, Is.EqualTo(expectedPath));
        }

        return response.StatusCode;
    }

    private static AdminUser CreateUser(string role)
    {
        return new AdminUser
        {
            Id = Guid.NewGuid(),
            LoginName = "tester",
            NormalizedLoginName = "TESTER",
            Roles = role,
            SecurityStamp = Guid.NewGuid().ToString(),
        };
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

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminPanel:Auth:DataProtectionKeyPath"] = Path.Combine(Path.GetTempPath(), "openmu-role-test-keys"),
            })
            .Build();
    }

    private async Task<AdminPolicyGuard> CreateGuardAsync(AdminUser? existingUser, ClaimsPrincipal principal)
    {
        var users = new InMemoryAdminUserRepository();
        if (existingUser is not null)
        {
            await users.AddAsync(existingUser).ConfigureAwait(false);
        }

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddSingleton<IAdminUserRepository>(users);
        services.AddAdminPanelAuth(CreateConfiguration());
        services.AddScoped<AuthenticationStateProvider>(_ => new FixedAuthenticationStateProvider(principal));
        this._serviceProvider = services.BuildServiceProvider();

        return this._serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AdminPolicyGuard>();
    }

    private async Task<HttpClient> StartHostAsync(AdminUser user)
    {
        var users = new InMemoryAdminUserRepository();
        await users.AddAsync(user).ConfigureAwait(false);

        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Configuration.AddConfiguration(CreateConfiguration());
        builder.Services.AddSingleton<IAdminUserRepository>(users);
        builder.Services.AddAdminPanelAuth(builder.Configuration);

        var app = builder.Build();
        this._app = app;
        app.UseRouting();
        app.UseAdminPanelAuth();
        app.UseAuthorizedPath("/logs", AdminPolicies.Administrator);
        app.MapTestSignIn(TestLoginPath, user);
        app.MapGet("/logs/{name}", () => "log content");
        await app.StartAsync().ConfigureAwait(false);

        this._http = new HttpClient(new HttpClientHandler { UseProxy = false, AllowAutoRedirect = false }) { BaseAddress = new Uri(app.Urls.Single()) };
        return this._http;
    }

    private sealed class FixedAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthenticationState _state;

        public FixedAuthenticationStateProvider(ClaimsPrincipal principal)
        {
            this._state = new AuthenticationState(principal);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(this._state);
    }
}
