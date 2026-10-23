// <copyright file="McpIntegrationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Mcp;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Tests.AdminAuth;

/// <summary>Exercises the real HTTP transport, API-key authentication and read operations.</summary>
[TestFixture]
public class McpIntegrationTests
{
    private const string ViewerKey = "omu_viewer-key-for-mcp-integration-tests";
    private const string CashShopKey = "omu_cashshop-key-for-mcp-integration-tests";
    private WebApplication _app = null!;
    private HttpClient _http = null!;
    private InMemoryApiKeyRepository _keys = null!;
    private Guid _configurationId;

    /// <summary>Starts a minimal admin host with real in-memory persistence.</summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AdminPanel:Mcp:Enabled"] = "true",
            ["AdminPanel:Auth:DataProtectionKeyPath"] = Path.Combine(Path.GetTempPath(), "openmu-mcp-test-keys"),
        });
        this._keys = new InMemoryApiKeyRepository();
        await this._keys.AddAsync(new ApiKey { Name = "test-viewer", KeyHash = ApiKeyGenerator.Hash(ViewerKey), Roles = AdminRoles.Viewer }).ConfigureAwait(false);
        await this._keys.AddAsync(new ApiKey { Name = "test-cashshop", KeyHash = ApiKeyGenerator.Hash(CashShopKey), Roles = AdminRoles.CashShop }).ConfigureAwait(false);
        await this._keys.AddAsync(new ApiKey { Name = "test-admin", KeyHash = ApiKeyGenerator.Hash("omu_mcp-admin-test"), Roles = AdminRoles.Administrator }).ConfigureAwait(false);
        builder.Services.AddSingleton<IApiKeyRepository>(this._keys);
        builder.Services.AddSingleton(Mock.Of<IModalService>());
        builder.Services.AddSingleton(Mock.Of<IToastService>());
        builder.Services.AddAdminPanelAuth(builder.Configuration);
        builder.Services.AddAdminReadOperations();
        builder.Services.AddAdminMcp(builder.Configuration);
        builder.Services.AddMcpServer().WithTools<RestrictedTestTools>();
        var persistence = new InMemoryPersistenceContextProvider();
        using var context = persistence.CreateNewContext();
        var config = context.CreateNew<GameConfiguration>();
        this._configurationId = config.GetId();
        config.ExperienceRate = 3.0f;
        var item = context.CreateNew<MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition>();
        item.Name = "Test Sword";
        config.Items.Add(item);
        var plugin = context.CreateNew<PlugInConfiguration>();
        plugin.CustomConfiguration = "secret-plugin-token";
        config.PlugInConfigurations.Add(plugin);
        var account = context.CreateNew<Account>();
        account.LoginName = "tester";
        account.EMail = "test@example.org";
        account.PasswordHash = "secret-password-hash";
        account.SecurityCode = "secret-security-code";
        account.VaultPassword = "secret-vault-password";
        var character = context.CreateNew<Character>();
        character.Name = "Hero";
        account.Characters.Add(character);
        var guild = context.CreateNew<MUnique.OpenMU.DataModel.Entities.Guild>();
        guild.Name = "Knights";
        await context.SaveChangesAsync().ConfigureAwait(false);
        var configurations = new Mock<IDataSource<GameConfiguration>>();
        configurations.Setup(c => c.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(config);
        builder.Services.AddSingleton(configurations.Object);
        builder.Services.AddSingleton<IPersistenceContextProvider>(persistence);
        var login = new Mock<ILoginServer>();
        login.Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte> { ["tester"] = 4 });
        builder.Services.AddSingleton(login.Object);
        var servers = new Mock<IServerProvider>();
        servers.SetupGet(s => s.Servers).Returns(new List<IManageableServer>());
        builder.Services.AddSingleton(servers.Object);
        builder.Services.AddSingleton<IAdminReadModel, TestReadModel>();
        builder.Services.AddSingleton(new PlugInManager([], Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance, null, null));
        this._app = builder.Build();
        this._app.UseRouting();
        this._app.UseAdminPanelAuth();
        this._app.MapAdminMcp();
        this._app.MapGet("/test-cookie", async (HttpContext http) =>
        {
            var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "admin"), new Claim(ClaimTypes.Role, AdminRoles.Viewer)], CookieAuthenticationDefaults.AuthenticationScheme);
            await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)).ConfigureAwait(false);
        });
        await this._app.StartAsync().ConfigureAwait(false);
        this._http = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = new Uri(this._app.Urls.Single()) };
    }

    /// <summary>Stops the HTTP listener.</summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        this._http?.Dispose();
        if (this._app is { } app)
        {
            await app.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Setup mode never bypasses MCP authentication or roles, and browser cookies are rejected.</summary>
    [Test]
    public async Task RequiresViewerApiKeyEvenWithoutAdminUsersAsync()
    {
        using var anonymous = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        Assert.That(anonymous.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        using var cookie = await this._http.GetAsync("/test-cookie").ConfigureAwait(false);
        using var cookieOnly = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        Assert.That(cookieOnly.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        this._http.DefaultRequestHeaders.Add("X-Api-Key", "invalid-key");
        using var invalid = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        Assert.That(invalid.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        this._http.DefaultRequestHeaders.Remove("X-Api-Key");
        this._http.DefaultRequestHeaders.Add("X-Api-Key", CashShopKey);
        using var wrongRole = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        Assert.That(wrongRole.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    /// <summary>Every phase-one tool can be discovered and called through the official C# client.</summary>
    [Test]
    public async Task CallsAllReadToolsAndNeverReturnsSecretsAsync()
    {
        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        var tools = await client.ListToolsAsync().ConfigureAwait(false);
        Assert.That(tools.Count, Is.EqualTo(15));
        var arguments = new Dictionary<string, Dictionary<string, object?>>
        {
            ["accounts_get"] = new() { ["loginName"] = "tester" },
            ["characters_get"] = new() { ["characterName"] = "Hero" },
            ["guilds_get"] = new() { ["name"] = "Knights" },
            ["config_describe"] = new() { ["type"] = "GameConfiguration" },
            ["config_query"] = new() { ["type"] = "GameConfiguration" },
            ["config_get"] = new() { ["type"] = "GameConfiguration", ["id"] = this._configurationId, ["depth"] = 2 },
            ["config_search"] = new() { ["text"] = "experience" },
        };
        foreach (var tool in tools)
        {
            var result = await client.CallToolAsync(tool.Name, arguments.GetValueOrDefault(tool.Name) ?? []).ConfigureAwait(false);
            Assert.That(result.IsError, Is.Not.True, tool.Name + ": " + JsonSerializer.Serialize(result));
            var json = JsonSerializer.Serialize(result);
            Assert.That(json, Does.Not.Contain("secret-"), tool.Name);
            Assert.That(tool.ProtocolTool.Annotations?.ReadOnlyHint, Is.True, tool.Name);
            Assert.That(tool.ProtocolTool.Annotations?.DestructiveHint, Is.False, tool.Name);
        }
    }

    /// <summary>Invalid paging, types, filters, depth and unavailable live filters fail clearly.</summary>
    [TestCase("accounts_search", "limit", 101)]
    [TestCase("guilds_search", "offset", -1)]
    [TestCase("players_list", "mapNumber", 0)]
    [TestCase("config_query", "type", "Account")]
    [TestCase("config_query", "type", "PlugInConfiguration")]
    public async Task RejectsInvalidArgumentsAsync(string tool, string parameter, object value)
    {
        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync(tool, new Dictionary<string, object?> { [parameter] = value }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
    }

    /// <summary>Search filters apply before paging, and return totals.</summary>
    [Test]
    public async Task SearchesEmailAndQueriesTypedConfigurationFieldsAsync()
    {
        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        var accounts = await client.CallToolAsync("accounts_search", new Dictionary<string, object?> { ["text"] = "EXAMPLE.ORG" }).ConfigureAwait(false);
        Assert.That(JsonSerializer.Serialize(accounts), Does.Contain("tester"));
        var matches = await client.CallToolAsync("config_query", new Dictionary<string, object?> { ["type"] = "GameConfiguration", ["field"] = "ExperienceRate", ["op"] = "gt", ["value"] = "2", ["fields"] = "ExperienceRate" }).ConfigureAwait(false);
        Assert.That(matches.IsError, Is.Not.True);
        Assert.That(JsonSerializer.Serialize(matches), Does.Contain("ExperienceRate"));
        var excluded = await client.CallToolAsync("config_query", new Dictionary<string, object?> { ["type"] = "GameConfiguration", ["fields"] = "PlugInConfigurations" }).ConfigureAwait(false);
        Assert.That(excluded.IsError, Is.True);
    }

    /// <summary>Disabling an existing key revokes access on the next request.</summary>
    [Test]
    public async Task RevokedKeyIsRejectedAsync()
    {
        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        await client.ListToolsAsync().ConfigureAwait(false);
        (await this._keys.GetAllAsync().ConfigureAwait(false)).Single(k => k.Name == "test-viewer").IsDisabled = true;
        this._http.DefaultRequestHeaders.Add("Authorization", "Bearer " + ViewerKey);
        using var response = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    /// <summary>The real tools/list schema is held stable for clients.</summary>
    [Test]
    public async Task ToolSchemaMatchesSnapshotAsync()
    {
        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        var tools = await client.ListToolsAsync().ConfigureAwait(false);
        var schema = JsonSerializer.SerializeToNode(tools.OrderBy(t => t.Name).Select(t => new { t.Name, t.ProtocolTool.InputSchema, t.ProtocolTool.Annotations }));
        var expected = JsonNode.Parse(await File.ReadAllTextAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "Mcp", "tools.json")).ConfigureAwait(false));
        Assert.That(JsonNode.DeepEquals(schema, expected), Is.True, schema!.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>Tool-level role checks filter discovery and reject direct calls by name.</summary>
    [Test]
    public async Task FiltersRestrictedToolsAndRechecksCallsAsync()
    {
        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        Assert.That((await client.ListToolsAsync().ConfigureAwait(false)).Select(t => t.Name), Does.Not.Contain("test_admin_only"));
        Assert.ThrowsAsync<McpProtocolException>(async () =>
            await client.CallToolAsync("test_admin_only", new Dictionary<string, object?>()).ConfigureAwait(false));
        await using var administrator = await McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(this._http.BaseAddress!, "/mcp"),
            AdditionalHeaders = new Dictionary<string, string> { ["Authorization"] = "Bearer omu_mcp-admin-test" },
        }, this._http)).ConfigureAwait(false);
        Assert.That((await administrator.ListToolsAsync().ConfigureAwait(false)).Select(t => t.Name), Does.Contain("test_admin_only"));
    }

    /// <summary>Only the offending API client consumes its rate allowance.</summary>
    [Test]
    public async Task RateLimitsPerKeyAndRejectsLargeBodiesAsync()
    {
        this._app.Services.GetRequiredService<AdminMcpOptions>().RequestsPerMinute = 2;
        this._http.DefaultRequestHeaders.Add("X-Api-Key", ViewerKey);
        using var first = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        using var second = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        using var limited = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        Assert.That(limited.StatusCode, Is.EqualTo(HttpStatusCode.TooManyRequests));
        this._http.DefaultRequestHeaders.Remove("X-Api-Key");
        this._http.DefaultRequestHeaders.Add("X-Api-Key", "omu_mcp-admin-test");
        using var otherKey = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        Assert.That(otherKey.StatusCode, Is.Not.EqualTo(HttpStatusCode.TooManyRequests));
        using var content = new StringContent(new string('x', 65537));
        using var oversized = await this._http.PostAsync("/mcp", content).ConfigureAwait(false);
        Assert.That(oversized.StatusCode, Is.EqualTo(HttpStatusCode.RequestEntityTooLarge));
    }

    /// <summary>Disabled means the HTTP route is not registered.</summary>
    [Test]
    public async Task DisabledByDefaultAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddAdminMcp(new ConfigurationBuilder().Build());
        await using var app = builder.Build();
        app.MapAdminMcp();
        await app.StartAsync().ConfigureAwait(false);
        using var http = new HttpClient(new HttpClientHandler { UseProxy = false }) { BaseAddress = new Uri(app.Urls.Single()) };
        using var response = await http.GetAsync("/mcp").ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    private async ValueTask<McpClient> ConnectAsync()
    {
        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(this._http.BaseAddress!, "/mcp"),
            AdditionalHeaders = new Dictionary<string, string> { ["X-Api-Key"] = ViewerKey },
        }, this._http);
        return await McpClient.CreateAsync(transport).ConfigureAwait(false);
    }
}
