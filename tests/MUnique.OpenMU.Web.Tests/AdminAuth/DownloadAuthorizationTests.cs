// <copyright file="DownloadAuthorizationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests that the downloads of the admin panel require a logged in user, even when the hosting
/// application doesn't require an authorization for all of its controllers.
/// </summary>
/// <remarks>
/// The admin panel of the distributed deployment maps the controllers without requiring an
/// authorization, because dapr calls some of them, see <c>ConfigureDaprService</c> in
/// <c>MUnique.OpenMU.Dapr.Common</c>. The host of these tests maps them the same way.
/// </remarks>
[TestFixture]
public sealed class DownloadAuthorizationTests : IDisposable
{
    private const string TestLoginPath = "/test-login";

    private const string PasswordHash = "secret-password-hash";

    private WebApplication? _app;
    private HttpClient? _http;
    private string[] _downloadUris = [];

    /// <summary>Stops the host.</summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        this.Dispose();
        if (this._app is { } app)
        {
            await app.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Disposes the HTTP client owned by this fixture.</summary>
    public void Dispose()
    {
        this._http?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Once an admin user exists, a download without a login is redirected to the login page.
    /// </summary>
    [Test]
    public async Task DownloadsRequireLoginAsync()
    {
        var http = await this.StartHostAsync(withAdminUser: true).ConfigureAwait(false);

        var responses = await GetAllAsync(http, this._downloadUris).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            foreach (var (uri, status, location, content) in responses)
            {
                Assert.That(status, Is.EqualTo(HttpStatusCode.Redirect), $"{uri} answered without a login: {content}");
                Assert.That(location, Is.EqualTo(AdminAuthenticationDefaults.LoginPath), uri);
                Assert.That(content, Does.Not.Contain(PasswordHash), uri);
            }
        });
    }

    /// <summary>A logged in user gets the downloads.</summary>
    [Test]
    public async Task DownloadsWorkWithLoginAsync()
    {
        var http = await this.StartHostAsync(withAdminUser: true).ConfigureAwait(false);
        using (var login = await http.GetAsync(TestLoginPath).ConfigureAwait(false))
        {
            login.EnsureSuccessStatusCode();
        }

        var responses = await GetAllAsync(http, this._downloadUris).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            foreach (var (uri, status, _, _) in responses)
            {
                Assert.That(status, Is.EqualTo(HttpStatusCode.OK), uri);
            }
        });
    }

    /// <summary>
    /// Before the first admin user is created, the admin panel is usable without a login by design,
    /// so that the server can be set up.
    /// </summary>
    [Test]
    public async Task DownloadsWorkWithoutLoginInSetupModeAsync()
    {
        var http = await this.StartHostAsync(withAdminUser: false).ConfigureAwait(false);

        var responses = await GetAllAsync(http, this._downloadUris).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            foreach (var (uri, status, _, _) in responses)
            {
                Assert.That(status, Is.EqualTo(HttpStatusCode.OK), uri);
            }
        });
    }

    /// <summary>
    /// Every controller action of the admin panel declares whether it requires an authorization,
    /// because a host may map the controllers without requiring it.
    /// </summary>
    [Test]
    public async Task EveryControllerActionDeclaresItsAuthorizationAsync()
    {
        await this.StartHostAsync(withAdminUser: true).ConfigureAwait(false);

        var actions = ((IEndpointRouteBuilder)this._app!).DataSources
            .SelectMany(source => source.Endpoints)
            .Where(endpoint => endpoint.Metadata.GetMetadata<ControllerActionDescriptor>() is not null)
            .ToList();
        var undeclared = actions
            .Where(endpoint => !endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>().Any()
                               && endpoint.Metadata.GetMetadata<IAllowAnonymous>() is null)
            .Select(endpoint => endpoint.DisplayName)
            .ToList();

        Assert.That(actions, Is.Not.Empty);
        Assert.That(undeclared, Is.Empty, "These actions need an [Authorize] or [AllowAnonymous].");
    }

    private static async Task<List<(string Uri, HttpStatusCode Status, string? Location, string Content)>> GetAllAsync(HttpClient http, IEnumerable<string> uris)
    {
        var result = new List<(string, HttpStatusCode, string?, string)>();
        foreach (var uri in uris)
        {
            using var response = await http.GetAsync(uri).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            result.Add((uri, response.StatusCode, response.Headers.Location?.AbsolutePath, content));
        }

        return result;
    }

    private static IDataSource<T> CreateDataSource<T>(T owner)
        where T : class
    {
        var dataSource = new Mock<IDataSource<T>>();
        dataSource.Setup(source => source.GetOwnerAsync(owner.GetId(), It.IsAny<CancellationToken>())).ReturnsAsync(owner);
        return dataSource.Object;
    }

    private async Task<HttpClient> StartHostAsync(bool withAdminUser)
    {
        var adminUser = new AdminUser { Id = Guid.NewGuid(), LoginName = "admin", NormalizedLoginName = "ADMIN", Roles = AdminRoles.Administrator, SecurityStamp = Guid.NewGuid().ToString() };
        var users = new InMemoryAdminUserRepository();
        if (withAdminUser)
        {
            await users.AddAsync(adminUser).ConfigureAwait(false);
        }

        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AdminPanel:Auth:DataProtectionKeyPath"] = Path.Combine(Path.GetTempPath(), "openmu-download-test-keys"),
        });
        builder.Services.AddSingleton<IAdminUserRepository>(users);
        builder.Services.AddSingleton<IApiKeyRepository>(new InMemoryApiKeyRepository());
        builder.Services.AddAdminPanelAuth(builder.Configuration);

        // The same controllers as AddAdminPanel adds.
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(AccountController).Assembly)
            .AddApplicationPart(typeof(ByteArrayDownloadController).Assembly)
            .ConfigureApplicationPartManager(setup => setup.FeatureProviders.Add(new GenericControllerFeatureProvider()));

        var persistence = new InMemoryPersistenceContextProvider();
        using var context = persistence.CreateNewContext();
        var configuration = context.CreateNew<GameConfiguration>();
        var client = context.CreateNew<GameClientDefinition>();
        client.Serial = [1, 2, 3];
        var account = context.CreateNew<Account>();
        account.LoginName = "tester";
        account.PasswordHash = PasswordHash;
        await context.SaveChangesAsync().ConfigureAwait(false);
        builder.Services.AddSingleton<IPersistenceContextProvider>(persistence);
        builder.Services.AddSingleton(CreateDataSource(configuration));
        builder.Services.AddSingleton(CreateDataSource(account));

        this._downloadUris =
        [
            $"/download/{nameof(GameConfiguration)}/{nameof(GameConfiguration)}_{configuration.GetId()}.json",
            $"/download/{nameof(Account)}/{nameof(Account)}_{account.GetId()}.json",
            $"/download/{client.GetType()}/{client.GetId()}/{nameof(GameClientDefinition.Serial)}",
        ];

        var app = builder.Build();
        this._app = app;

        // Like ConfigureDaprService and the Program.cs of the admin panel of the distributed deployment.
        app.MapControllers();
        app.UseRouting();
        app.UseAdminPanelAuth();
        app.MapGet(TestLoginPath, async (HttpContext http) =>
        {
            // The claims of a real login, which include the roles implied by the assigned one.
            var claims = AdminLoginService.CreateClaims(adminUser, usedSecondFactor: true);
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)).ConfigureAwait(false);
        });
        await app.StartAsync().ConfigureAwait(false);

        this._http = new HttpClient(new HttpClientHandler { UseProxy = false, AllowAutoRedirect = false }) { BaseAddress = new Uri(app.Urls.Single()) };
        return this._http;
    }
}
