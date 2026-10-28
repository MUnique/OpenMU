// <copyright file="McpIntegrationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Collections;
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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Mcp;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Tests.AdminAuth;

/// <summary>Exercises the real HTTP transport, API-key authentication and read operations.</summary>
[TestFixture]
public sealed class McpIntegrationTests : System.IDisposable
{
    private const string ViewerKey = "omu_viewer-key-for-mcp-integration-tests";
    private const string CashShopKey = "omu_cashshop-key-for-mcp-integration-tests";
    private WebApplication _app = null!;
    private HttpClient _http = null!;
    private InMemoryApiKeyRepository _keys = null!;
    private Guid _configurationId;
    private Guid _guildId;

    /// <summary>Gets or sets whether this host explicitly enables operational write tools.</summary>
    public bool EnableWrites { get; set; }

    /// <summary>Gets the running host for operational integration tests.</summary>
    internal WebApplication App => this._app;

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
            ["AdminPanel:Mcp:ReadOnly"] = this.EnableWrites ? "false" : "true",
            ["AdminPanel:Auth:DataProtectionKeyPath"] = Path.Combine(Path.GetTempPath(), "openmu-mcp-test-keys"),
        });
        this._keys = new InMemoryApiKeyRepository();
        await this._keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "test-viewer", KeyHash = ApiKeyGenerator.Hash(ViewerKey), Roles = AdminRoles.Viewer }).ConfigureAwait(false);
        await this._keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "test-cashshop", KeyHash = ApiKeyGenerator.Hash(CashShopKey), Roles = AdminRoles.CashShop }).ConfigureAwait(false);
        await this._keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "test-admin", KeyHash = ApiKeyGenerator.Hash("omu_mcp-admin-test"), Roles = AdminRoles.Administrator }).ConfigureAwait(false);
        await this._keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "unrelated", KeyHash = ApiKeyGenerator.Hash("omu_unrelated-role"), Roles = "Unrelated" }).ConfigureAwait(false);
        builder.Services.AddSingleton<IApiKeyRepository>(this._keys);
        builder.Services.AddSingleton(Mock.Of<IModalService>());
        builder.Services.AddSingleton(Mock.Of<IToastService>());
        builder.Services.AddAdminPanelAuth(builder.Configuration);
        builder.Services.AddAdminReadOperations();
        builder.Services.AddControllers(options => options.Filters.Add<AdminOperationExceptionFilter>()).AddApplicationPart(typeof(AccountController).Assembly);
        builder.Services.AddSingleton<IAdminAuditRepository, InMemoryAdminAuditRepository>();
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
        var map = context.CreateNew<GameMapDefinition>();
        map.Number = 0;
        config.Maps.Add(map);
        var monster = context.CreateNew<MonsterDefinition>();
        monster.Number = 1;
        config.Monsters.Add(monster);
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
        this._guildId = guild.GetId();
        await context.SaveChangesAsync().ConfigureAwait(false);
        var configurations = new Mock<IDataSource<GameConfiguration>>();
        configurations.Setup(c => c.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(config);
        builder.Services.AddSingleton(configurations.Object);
        builder.Services.AddSingleton<IPersistenceContextProvider>(persistence);
        var login = new Mock<ILoginServer>();
        login.Setup(l => l.GetSnapshotAsync()).ReturnsAsync(new Dictionary<string, byte> { ["tester"] = 4 });
        builder.Services.AddSingleton(login.Object);
        builder.Services.AddSingleton(Mock.Of<IGuildServer>());
        var servers = new Mock<IServerProvider>();
        servers.SetupGet(s => s.Servers).Returns(new List<IManageableServer>());
        builder.Services.AddSingleton(servers.Object);
        builder.Services.AddSingleton<IAdminReadModel, TestReadModel>();
        builder.Services.AddSingleton(services => new PlugInManager([], Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance, null, new ByDataSourceReferenceHandler(services.GetRequiredService<IDataSource<GameConfiguration>>())));
        this._app = builder.Build();
        this._app.UseRouting();
        // Model a container proxy with RFC 5737 documentation addresses; no traffic is sent to them.
        this._app.Use((http, next) =>
        {
            if (http.Request.Headers["X-Test-Remote-IP"].FirstOrDefault() is { } address)
            {
                http.Connection.RemoteIpAddress = IPAddress.Parse(address);
            }

            return next(http);
        });
        var forwardedHeaders = new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedProto };
        forwardedHeaders.KnownProxies.Add(IPAddress.Parse("192.0.2.10"));
        this._app.UseForwardedHeaders(forwardedHeaders);
        this._app.UseAdminMcpProtection();
        this._app.UseAdminPanelAuth();
        this._app.MapAdminMcp();
        this._app.MapControllers();
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
        this._http.DefaultRequestHeaders.Add("X-Api-Key", "omu_unrelated-role");
        using var wrongRole = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        Assert.That(wrongRole.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    /// <summary>Every phase-one tool can be discovered and called through the official C# client.</summary>
    [Test]
    public async Task CallsAllReadToolsAndNeverReturnsSecretsAsync()
    {
        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        var tools = await client.ListToolsAsync().ConfigureAwait(false);
        Assert.That(tools.Count, Is.EqualTo(25));
        var servers = new List<IManageableServer>();
        foreach (var id in new[] { 0, 4 })
        {
            var server = new Mock<IGameServer>();
            server.SetupGet(s => s.Id).Returns(id);
            server.SetupGet(s => s.ServerState).Returns(ServerState.Started);
            var live = server.As<IGameServerAdministration>();
            var player = new LivePlayerInfo(Guid.NewGuid(), "tester", "Hero", 0, 1, 2, 10, 0, null, false);
            live.Setup(s => s.GetPlayerAsync("Hero", It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(player);
            live.Setup(s => s.GetPlayerSnapshotAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(new[] { player });
            live.Setup(s => s.GetEventsAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(Array.Empty<LiveEventInfo>());
            live.Setup(s => s.GetAdminOperationsAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(Array.Empty<AdminPluginOperationInfo>());
            servers.Add(server.Object);
        }

        Mock.Get(this._app.Services.GetRequiredService<IServerProvider>()).SetupGet(s => s.Servers).Returns(servers);
        var arguments = new Dictionary<string, Dictionary<string, object?>>
        {
            ["drops_explain"] = new() { ["monsterNumber"] = 1, ["mapNumber"] = 0 },
            ["spawns_list"] = new() { ["mapNumber"] = 0 },
            ["accounts_get"] = new() { ["loginName"] = "tester" },
            ["characters_get"] = new() { ["characterName"] = "Hero" },
            ["players_get"] = new() { ["characterName"] = "Hero" },
            ["guilds_get"] = new() { ["name"] = "Knights" },
            ["config_describe"] = new() { ["type"] = "GameConfiguration" },
            ["config_query"] = new() { ["type"] = "GameConfiguration" },
            ["config_get"] = new() { ["type"] = "GameConfiguration", ["id"] = this._configurationId, ["depth"] = 2 },
            ["config_search"] = new() { ["text"] = "experience" },
            ["content_list"] = new() { ["type"] = "Maps" },
        };
        foreach (var tool in tools.Where(t => t.Name != "plugin_operations_read"))
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
        Assert.That(limited.Headers.RetryAfter?.Delta, Is.GreaterThan(TimeSpan.Zero).And.LessThanOrEqualTo(TimeSpan.FromMinutes(1)));
        this._http.DefaultRequestHeaders.Remove("X-Api-Key");
        this._http.DefaultRequestHeaders.Add("X-Api-Key", "omu_mcp-admin-test");
        using var otherKey = await this._http.PostAsJsonAsync("/mcp", new { }).ConfigureAwait(false);
        Assert.That(otherKey.StatusCode, Is.Not.EqualTo(HttpStatusCode.TooManyRequests));
        Assert.That(otherKey.Headers.RetryAfter, Is.Null);
        using var content = new StringContent(new string('x', 65537));
        using var oversized = await this._http.PostAsync("/mcp", content).ConfigureAwait(false);
        Assert.That(oversized.StatusCode, Is.EqualTo(HttpStatusCode.RequestEntityTooLarge));
    }

    /// <summary>Guild filtering uses the separately registered guild service with a live member.</summary>
    [Test]
    public async Task ListsOnlineGuildMembersUsingGuildServiceAsync()
    {
        var persistence = this._app.Services.GetRequiredService<IPersistenceContextProvider>();
        var config = await this._app.Services.GetRequiredService<IDataSource<GameConfiguration>>().GetOwnerAsync().ConfigureAwait(false);
        using var context = persistence.CreateNewPlayerContext(config);
        var account = await context.GetAccountByLoginNameAsync("tester").ConfigureAwait(false);
        Assert.That(account, Is.Not.Null);
        var gameContext = new Mock<IGameServerContext>();
        gameContext.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        gameContext.SetupGet(c => c.Configuration).Returns(config);
        gameContext.SetupGet(c => c.PersistenceContextProvider).Returns(persistence);
        gameContext.SetupGet(c => c.PlugInManager).Returns(this._app.Services.GetRequiredService<PlugInManager>());
        await using var player = new Player(gameContext.Object)
        {
            Account = account,
            GuildStatus = new GuildMemberStatus(7, GuildPosition.NormalMember),
        };
        gameContext.Setup(c => c.GetPlayersAsync()).ReturnsAsync(new List<Player> { player });
        var server = new Mock<IManageableServer>();
        server.SetupGet(s => s.Id).Returns(4);
        server.SetupGet(s => s.Type).Returns(ServerType.GameServer);
        server.As<IGameServerContextProvider>().SetupGet(s => s.Context).Returns(gameContext.Object);
        Mock.Get(this._app.Services.GetRequiredService<IServerProvider>()).SetupGet(s => s.Servers).Returns(new List<IManageableServer> { server.Object });
        var guildServer = Mock.Get(this._app.Services.GetRequiredService<IGuildServer>());
        guildServer.Setup(g => g.GetGuildIdAsync(this._guildId)).ReturnsAsync(7U);

        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("players_list", new Dictionary<string, object?> { ["guildName"] = "Knights" }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.Not.True);
        var data = JsonSerializer.SerializeToNode(result.StructuredContent);
        Assert.That(data?["total"]?.GetValue<int>(), Is.EqualTo(1));
        Assert.That(data?["items"]?[0]?["account"]?.GetValue<string>(), Is.EqualTo("tester"));
        Assert.That(data?["items"]?[0]?["guildId"]?.GetValue<uint>(), Is.EqualTo(7));
        Assert.That(data?["items"]?[0]?["liveDataAvailable"]?.GetValue<bool>(), Is.True);
        guildServer.Verify(g => g.GetGuildIdAsync(this._guildId), Times.Once);

        var missing = await client.CallToolAsync("players_list", new Dictionary<string, object?> { ["guildName"] = "Missing" }).ConfigureAwait(false);
        Assert.That(JsonSerializer.SerializeToNode(missing.StructuredContent)?["total"]?.GetValue<int>(), Is.Zero);
    }

    /// <summary>Distinct API keys with the same display name have independent rate allowances.</summary>
    [Test]
    public async Task DuplicateKeyNamesHaveIndependentRateLimitsAsync()
    {
        const string secondKey = "omu_second-viewer-key-for-mcp-integration-tests";
        await this._keys.AddAsync(new ApiKey
        {
            Id = Guid.NewGuid(),
            Name = "test-viewer",
            KeyHash = ApiKeyGenerator.Hash(secondKey),
            Roles = AdminRoles.Viewer,
        }).ConfigureAwait(false);
        this._app.Services.GetRequiredService<AdminMcpOptions>().RequestsPerMinute = 2;

        Assert.That(await this.PingAsync(ViewerKey).ConfigureAwait(false), Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await this.PingAsync(ViewerKey).ConfigureAwait(false), Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await this.PingAsync(ViewerKey).ConfigureAwait(false), Is.EqualTo(HttpStatusCode.TooManyRequests));
        Assert.That(await this.PingAsync(secondKey).ConfigureAwait(false), Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await this.PingAsync(secondKey).ConfigureAwait(false), Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await this.PingAsync(secondKey).ConfigureAwait(false), Is.EqualTo(HttpStatusCode.TooManyRequests));
        Assert.That(await this.PingAsync(ViewerKey).ConfigureAwait(false), Is.EqualTo(HttpStatusCode.TooManyRequests));
    }

    /// <summary>Only a trusted TLS proxy can make a remote HTTP connection secure.</summary>
    [TestCase("192.0.2.10", "https", HttpStatusCode.OK)]
    [TestCase("192.0.2.10", "http", HttpStatusCode.Forbidden)]
    [TestCase("192.0.2.20", "https", HttpStatusCode.Forbidden)]
    public async Task ChecksProxyTransportBeforeAuthenticationAsync(string remoteAddress, string scheme, HttpStatusCode expected)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/mcp");
        request.Headers.Add("X-Test-Remote-IP", remoteAddress);
        request.Headers.Add("X-Forwarded-Proto", scheme);
        request.Headers.Add("X-Api-Key", ViewerKey);
        request.Headers.Accept.ParseAdd("application/json, text/event-stream");
        request.Content = JsonContent.Create(new { jsonrpc = "2.0", id = 1, method = "ping" });
        using var response = await this._http.SendAsync(request).ConfigureAwait(false);
        Assert.That(response.StatusCode, Is.EqualTo(expected));

        using var anonymous = new HttpRequestMessage(HttpMethod.Post, "/mcp");
        anonymous.Content = JsonContent.Create(new { jsonrpc = "2.0", id = 1, method = "ping" });
        anonymous.Headers.Accept.ParseAdd("application/json, text/event-stream");
        anonymous.Headers.Add("X-Test-Remote-IP", remoteAddress);
        anonymous.Headers.Add("X-Forwarded-Proto", scheme);
        using var anonymousResponse = await this._http.SendAsync(anonymous).ConfigureAwait(false);
        Assert.That(anonymousResponse.StatusCode, Is.EqualTo(expected == HttpStatusCode.OK ? HttpStatusCode.Unauthorized : HttpStatusCode.Forbidden));
    }

    /// <summary>Configuration reads use the panel's current cached owner, including its identifiers.</summary>
    [Test]
    public async Task ConfigurationReadsUseSharedDataSourceAsync()
    {
        using var context = new InMemoryPersistenceContextProvider().CreateNewContext();
        var cached = context.CreateNew<GameConfiguration>();
        cached.ExperienceRate = 42;
        var source = Mock.Get(this._app.Services.GetRequiredService<IDataSource<GameConfiguration>>());
        source.Setup(c => c.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(cached);
        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        var query = await client.CallToolAsync("config_query", new Dictionary<string, object?> { ["type"] = "GameConfiguration", ["fields"] = "ExperienceRate" }).ConfigureAwait(false);
        var queryData = JsonSerializer.SerializeToNode(query.StructuredContent);
        Assert.That(queryData?["items"]?[0]?["id"]?.GetValue<string>(), Is.EqualTo(cached.GetId().ToString()));
        Assert.That(queryData?["items"]?[0]?["ExperienceRate"]?.GetValue<float>(), Is.EqualTo(42));
        var result = await client.CallToolAsync("config_get", new Dictionary<string, object?> { ["type"] = "GameConfiguration", ["id"] = cached.GetId() }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.Not.True);
        Assert.That(JsonSerializer.SerializeToNode(result.StructuredContent)?["item"]?["ExperienceRate"]?.GetValue<float>(), Is.EqualTo(42));
        source.Verify(c => c.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()), Times.Exactly(2));
    }

    /// <summary>Concurrent collection edits restart a read, or produce a bounded, safe retry error.</summary>
    [TestCase("config_query", 1)]
    [TestCase("config_get", 1)]
    [TestCase("config_query", 3)]
    [TestCase("config_get", 3)]
    public async Task ConfigurationCollectionEditsAreRetriedAsync(string tool, int conflicts)
    {
        using var context = new InMemoryPersistenceContextProvider().CreateNewContext();
        var item = context.CreateNew<ItemDefinition>();
        item.Name = "Surviving Sword";
        var discarded = context.CreateNew<ItemDefinition>();
        discarded.Name = "Discarded partial item";
        var enumerations = 0;
        var collection = new Mock<ICollection<ItemDefinition>>();
        collection.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() =>
            ++enumerations <= conflicts ? ModifiedItems(discarded).GetEnumerator() : ((IEnumerable)new[] { item }).GetEnumerator());
        var cached = new Mock<GameConfiguration>();
        var id = Guid.NewGuid();
        cached.As<MUnique.OpenMU.Persistence.IIdentifiable>().SetupGet(c => c.Id).Returns(id);
        cached.SetupGet(c => c.Items).Returns(collection.Object);
        var source = Mock.Get(this._app.Services.GetRequiredService<IDataSource<GameConfiguration>>());
        source.Setup(c => c.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(cached.Object);

        await using var client = await this.ConnectAsync().ConfigureAwait(false);
        var arguments = tool == "config_query"
            ? new Dictionary<string, object?> { ["type"] = "ItemDefinition", ["fields"] = "Name" }
            : new Dictionary<string, object?> { ["type"] = "GameConfiguration", ["id"] = id };
        var result = await client.CallToolAsync(tool, arguments).ConfigureAwait(false);
        var json = JsonSerializer.Serialize(result);
        Assert.That(result.IsError == true, Is.EqualTo(conflicts == 3));
        Assert.That(enumerations, Is.EqualTo(conflicts == 3 ? 3 : 2));
        Assert.That(json, Does.Not.Contain(discarded.Name).And.Not.Contain("secret-config-edit-detail"));
        Assert.That(json, Does.Contain(conflicts == 3 ? "Retry this read" : item.Name));
        source.Verify(c => c.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
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

    private static IEnumerable<ItemDefinition> ModifiedItems(ItemDefinition item)
    {
        yield return item;
        throw new InvalidOperationException("secret-config-edit-detail");
    }

    private async ValueTask<HttpStatusCode> PingAsync(string key)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/mcp");
        request.Headers.Add("X-Api-Key", key);
        request.Headers.Accept.ParseAdd("application/json, text/event-stream");
        request.Content = JsonContent.Create(new { jsonrpc = "2.0", id = 1, method = "ping" });
        using var response = await this._http.SendAsync(request).ConfigureAwait(false);
        return response.StatusCode;
    }

    internal async ValueTask<McpClient> ConnectAsync(string key = ViewerKey)
    {
        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(this._http.BaseAddress!, "/mcp"),
            AdditionalHeaders = new Dictionary<string, string> { ["X-Api-Key"] = key },
        }, this._http);
        return await McpClient.CreateAsync(transport).ConfigureAwait(false);
    }
}
