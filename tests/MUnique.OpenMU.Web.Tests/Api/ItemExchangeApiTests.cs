// <copyright file="ItemExchangeApiTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Operations.ItemExchange;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Tests.AdminAuth;

/// <summary>
/// Tests who may download the item exchange file at <c>GET /api/items/exchange</c>.
/// </summary>
[TestFixture]
public sealed class ItemExchangeApiTests : IDisposable
{
    private const string ExchangePath = "/api/items/exchange";
    private const string ViewerKey = "omu_item-exchange-viewer-test";
    private const string CashShopKey = "omu_item-exchange-cashshop-test";

    private WebApplication? _app;
    private HttpClient? _http;

    /// <summary>
    /// Stops the host of a test.
    /// </summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        this.Dispose();
        if (this._app is { } app)
        {
            await app.DisposeAsync().ConfigureAwait(false);
            this._app = null;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._http?.Dispose();
        this._http = null;
    }

    /// <summary>
    /// Tests that a request without an API key is refused.
    /// </summary>
    [Test]
    public async Task WithoutKeyIsUnauthorizedAsync()
    {
        var http = await this.StartAsync().ConfigureAwait(false);

        using var response = await http.GetAsync(ExchangePath).ConfigureAwait(false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    /// <summary>
    /// Tests that the login cookie of the admin panel isn't enough; the file is only for API keys.
    /// </summary>
    [Test]
    public async Task LoginCookieIsNotEnoughAsync()
    {
        var http = await this.StartAsync().ConfigureAwait(false);
        using (var signIn = await http.GetAsync("/test-cookie").ConfigureAwait(false))
        {
            Assert.That(signIn.IsSuccessStatusCode, Is.True);
        }

        using var response = await http.GetAsync(ExchangePath).ConfigureAwait(false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    /// <summary>
    /// Tests that a key without the viewer role is refused.
    /// </summary>
    [Test]
    public async Task CashShopKeyIsForbiddenAsync()
    {
        var http = await this.StartAsync().ConfigureAwait(false);

        using var response = await SendAsync(http, CashShopKey).ConfigureAwait(false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    /// <summary>
    /// Tests that a viewer key gets the file of the stored configuration.
    /// </summary>
    [Test]
    public async Task ViewerKeyGetsTheFileAsync()
    {
        var http = await this.StartAsync().ConfigureAwait(false);

        using var response = await SendAsync(http, ViewerKey).ConfigureAwait(false);
        var body = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
        using var file = JsonDocument.Parse(body);
        Assert.That(file.RootElement.GetProperty("source").GetString(), Is.EqualTo("OpenMU"));
        Assert.That(file.RootElement.GetProperty("gameVersion").GetString(), Is.EqualTo("season6"));
        var item = file.RootElement.GetProperty("items").EnumerateArray().Single();
        Assert.That(item.GetProperty("group").GetInt32(), Is.EqualTo(0));
        Assert.That(item.GetProperty("number").GetInt32(), Is.EqualTo(1));
        Assert.That(ItemExchangeController.ExportGate.CurrentCount, Is.EqualTo(1), "The export is done, so the next one may start.");
    }

    /// <summary>
    /// Tests that a request while another export is running gets <c>429</c> instead of a second export,
    /// and that the next request after it gets the file.
    /// </summary>
    [Test]
    public async Task OneExportAtATimeAsync()
    {
        var http = await this.StartAsync().ConfigureAwait(false);

        await ItemExchangeController.ExportGate.WaitAsync().ConfigureAwait(false);
        try
        {
            using var refused = await SendAsync(http, ViewerKey).ConfigureAwait(false);

            Assert.That(refused.StatusCode, Is.EqualTo(HttpStatusCode.TooManyRequests));
            Assert.That(refused.Headers.RetryAfter?.Delta, Is.EqualTo(TimeSpan.FromSeconds(1)));
        }
        finally
        {
            ItemExchangeController.ExportGate.Release();
        }

        using var response = await SendAsync(http, ViewerKey).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private static async Task<HttpResponseMessage> SendAsync(HttpClient http, string key)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ExchangePath);
        request.Headers.Add("X-Api-Key", key);
        return await http.SendAsync(request).ConfigureAwait(false);
    }

    private async Task<HttpClient> StartAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AdminPanel:Auth:DataProtectionKeyPath"] = Path.Combine(Path.GetTempPath(), "openmu-item-exchange-test-keys"),
        });

        var keys = new InMemoryApiKeyRepository();
        await keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "viewer", KeyHash = ApiKeyGenerator.Hash(ViewerKey), Roles = AdminRoles.Viewer }).ConfigureAwait(false);
        await keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "cash shop", KeyHash = ApiKeyGenerator.Hash(CashShopKey), Roles = AdminRoles.CashShop }).ConfigureAwait(false);
        builder.Services.AddSingleton<IApiKeyRepository>(keys);

        // A user ends the setup mode of the admin panel, as on a server in use.
        var users = new InMemoryAdminUserRepository();
        await users.AddAsync(new AdminUser { Id = Guid.NewGuid(), LoginName = "admin", NormalizedLoginName = "ADMIN", Roles = AdminRoles.Administrator }).ConfigureAwait(false);

        builder.Services.AddSingleton<IAdminUserRepository>(users);
        builder.Services.AddSingleton(Mock.Of<IModalService>());
        builder.Services.AddSingleton(Mock.Of<IToastService>());
        builder.Services.AddAdminPanelAuth(builder.Configuration);
        builder.Services.AddScoped<ItemExchangeOperations>();
        builder.Services.AddControllers(options => options.Filters.Add<AdminOperationExceptionFilter>())
            .AddApplicationPart(typeof(ItemExchangeController).Assembly);

        var persistence = new InMemoryPersistenceContextProvider();
        using (var context = persistence.CreateNewContext())
        {
            var configuration = context.CreateNew<GameConfiguration>();
            var item = context.CreateNew<ItemDefinition>();
            item.Group = 0;
            item.Number = 1;
            item.Name = "Short Sword";
            configuration.Items.Add(item);
            context.CreateNew<ConfigurationUpdateState>().InitializationKey = "season6";
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        builder.Services.AddSingleton<IPersistenceContextProvider>(persistence);
        builder.Services.AddSingleton<IDataSource<GameConfiguration>, GameConfigurationDataSource>();

        this._app = builder.Build();
        this._app.UseRouting();
        this._app.UseAdminPanelAuth();

        // As in the admin panel of the all-in-one setup.
        this._app.MapControllers().RequireAuthorization();
        this._app.MapGet("/test-cookie", async (HttpContext http) =>
        {
            var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "admin"), new Claim(ClaimTypes.Role, AdminRoles.Administrator), new Claim(ClaimTypes.Role, AdminRoles.Viewer)], CookieAuthenticationDefaults.AuthenticationScheme);
            await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)).ConfigureAwait(false);
        });
        await this._app.StartAsync().ConfigureAwait(false);
        this._http = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = new Uri(this._app.Urls.Single()) };
        return this._http;
    }
}
