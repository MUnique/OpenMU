// <copyright file="McpOperationsIntegrationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Mcp;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.Tests.AdminAuth;

/// <summary>Verifies real MCP writes, audit durability, role isolation and read-only enforcement.</summary>
[TestFixture]
public sealed class McpOperationsIntegrationTests
{
    private McpIntegrationTests _host = null!;
    private InMemoryAdminAuditRepository _audit = null!;

    /// <summary>Starts a real HTTP host with explicitly enabled writes.</summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._host = new McpIntegrationTests { EnableWrites = true };
        await this._host.SetUpAsync().ConfigureAwait(false);
        this._audit = (InMemoryAdminAuditRepository)this._host.App.Services.GetRequiredService<IAdminAuditRepository>();
        var keys = (InMemoryApiKeyRepository)this._host.App.Services.GetRequiredService<IApiKeyRepository>();
        await keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "operator", KeyHash = ApiKeyGenerator.Hash("omu_operations-operator-test"), Roles = AdminRoles.Operator }).ConfigureAwait(false);
    }

    /// <summary>Stops the HTTP host.</summary>
    [TearDown]
    public async Task TearDownAsync() => await this._host.TearDownAsync().ConfigureAwait(false);

    /// <summary>Discovery and direct calls enforce independent role capabilities.</summary>
    [Test]
    public async Task ViewerAndCashShopKeysCannotEscalateAsync()
    {
        await using var viewer = await this._host.ConnectAsync().ConfigureAwait(false);
        var tools = await viewer.ListToolsAsync().ConfigureAwait(false);
        Assert.That(tools.All(t => t.ProtocolTool.Annotations?.ReadOnlyHint == true), Is.True);
        Assert.That(tools.Select(t => t.Name), Does.Not.Contain("server_start"));
        Assert.ThrowsAsync<McpProtocolException>(async () => await viewer.CallToolAsync("server_stop", new Dictionary<string, object?> { ["serverId"] = 0, ["serverType"] = "GameServer" }).ConfigureAwait(false));
        await using var cash = await this._host.ConnectAsync("omu_cashshop-key-for-mcp-integration-tests").ConfigureAwait(false);
        Assert.That((await cash.ListToolsAsync().ConfigureAwait(false)).Select(t => t.Name), Is.EquivalentTo(new[] { "cashshop_get_balance", "cashshop_grant" }));
        Assert.ThrowsAsync<McpProtocolException>(async () => await cash.CallToolAsync("accounts_get", new Dictionary<string, object?> { ["loginName"] = "tester" }).ConfigureAwait(false));
    }

    /// <summary>Even an already registered write handler cannot bypass the shared read-only guard.</summary>
    [Test]
    public async Task ReadOnlyPolicyRejectsWritesBeforeAuditOrSideEffectsAsync()
    {
        this._host.App.Services.GetRequiredService<AdminMcpOptions>().ReadOnly = true;
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("accounts_create", new Dictionary<string, object?> { ["loginName"] = "newbie", ["password"] = "private-pass" }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That(JsonSerializer.Serialize(result), Does.Contain("writes are disabled"));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>The audit attempt exists before a listener starts; the identifier is not a dictionary position.</summary>
    [Test]
    public async Task StartsActualServerIdAndAwaitsCompletionAsync()
    {
        var server = this.InstallServer(9);
        var completed = false;
        server.Setup(s => s.StartAsync()).Returns(async () =>
        {
            Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Pending"));
            await Task.Delay(10).ConfigureAwait(false);
            completed = true;
        });
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("server_start", new Dictionary<string, object?> { ["serverId"] = 9, ["serverType"] = "GameServer" }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.Not.True);
        Assert.That(completed, Is.True);
        var entry = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single();
        Assert.That(entry.Outcome, Is.EqualTo("Succeeded"));
        Assert.That(entry.Frontend, Is.EqualTo("mcp"));
        Assert.That(entry.PrincipalId, Is.Not.Empty);
        Assert.That(entry.ResultJson, Does.Contain("9"));
        var missing = await client.CallToolAsync("server_start", new Dictionary<string, object?> { ["serverId"] = 0, ["serverType"] = "GameServer" }).ConfigureAwait(false);
        Assert.That(missing.IsError, Is.True);
        server.Verify(s => s.StartAsync(), Times.Once);
    }

    /// <summary>Live player actions reach the owning server and record their outcomes.</summary>
    [Test]
    public async Task PlayerActionsRouteToOwningServerAsync()
    {
        var server = this.InstallServer(4);
        server.Setup(s => s.DisconnectAccountAsync("tester")).ReturnsAsync(true);
        server.Setup(s => s.BanPlayerAsync("Hero")).ReturnsAsync(true);
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var disconnected = await client.CallToolAsync("players_disconnect", new Dictionary<string, object?> { ["loginName"] = "tester" }).ConfigureAwait(false);
        Assert.That(disconnected.IsError, Is.Not.True);
        var banned = await client.CallToolAsync("players_ban", new Dictionary<string, object?> { ["characterName"] = "Hero", ["reason"] = "Confirmed abuse" }).ConfigureAwait(false);
        Assert.That(banned.IsError, Is.Not.True);
        server.Verify(s => s.DisconnectAccountAsync("tester"), Times.Once);
        server.Verify(s => s.BanPlayerAsync("Hero"), Times.Once);
        var entries = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items;
        Assert.That(entries.Select(e => e.Operation), Is.EquivalentTo(new[] { "players_disconnect", "players_ban" }));
        Assert.That(entries.All(e => e.Outcome == "Succeeded"), Is.True);
        this.InstallServer(9);
        var unavailable = await client.CallToolAsync("players_disconnect", new Dictionary<string, object?> { ["loginName"] = "tester" }).ConfigureAwait(false);
        Assert.That(unavailable.IsError, Is.True);
        Assert.That(JsonSerializer.Serialize(unavailable), Does.Contain("owning game server is unavailable"));
    }

    /// <summary>Without durable audit storage no side effect occurs.</summary>
    [Test]
    public async Task AuditFailurePreventsStartAsync()
    {
        var server = this.InstallServer(9);
        this._audit.RejectWrites = true;
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("server_start", new Dictionary<string, object?> { ["serverId"] = 9, ["serverType"] = "GameServer" }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        server.Verify(s => s.StartAsync(), Times.Never);
    }

    /// <summary>A failed completion update leaves a pending record and warns against blindly retrying.</summary>
    [Test]
    public async Task CompletionFailurePreservesUnknownOutcomeAsync()
    {
        var server = this.InstallServer(9);
        server.Setup(s => s.StartAsync()).Returns(ValueTask.CompletedTask);
        this._audit.RejectCompletion = true;
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("server_start", new Dictionary<string, object?> { ["serverId"] = 9, ["serverType"] = "GameServer" }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That(JsonSerializer.Serialize(result), Does.Contain("may have completed"));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Pending"));
        server.Verify(s => s.StartAsync(), Times.Once);
    }

    /// <summary>Credentials never appear in results or audit records; password resets require an offline account.</summary>
    [Test]
    public async Task AccountWritesExcludeSecretsAndRejectOnlineChangesAsync()
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        const string initial = "private-create-pass";
        const string reset = "private-reset-pass";
        var created = await client.CallToolAsync("accounts_create", new Dictionary<string, object?> { ["loginName"] = "newbie", ["password"] = initial }).ConfigureAwait(false);
        Assert.That(created.IsError, Is.Not.True);
        var changed = await client.CallToolAsync("accounts_reset_password", new Dictionary<string, object?> { ["loginName"] = "newbie", ["password"] = reset, ["reason"] = "Requested recovery" }).ConfigureAwait(false);
        Assert.That(changed.IsError, Is.Not.True);
        var online = await client.CallToolAsync("accounts_set_state", new Dictionary<string, object?> { ["loginName"] = "tester", ["state"] = "Banned", ["reason"] = "Testing safety" }).ConfigureAwait(false);
        Assert.That(online.IsError, Is.True);
        Assert.That(JsonSerializer.Serialize(online), Does.Contain("online"));
        var entries = await this._audit.ReadAsync(0, 100).ConfigureAwait(false);
        var json = JsonSerializer.Serialize(entries.Items);
        Assert.That(json, Does.Not.Contain(initial).And.Not.Contain(reset).And.Not.Contain("PasswordHash"));
        Assert.That(JsonSerializer.Serialize(created), Does.Not.Contain(initial));
        var source = this._host.App.Services.GetRequiredService<IDataSource<GameConfiguration>>();
        using var context = this._host.App.Services.GetRequiredService<IPersistenceContextProvider>().CreateNewPlayerContext(await source.GetOwnerAsync().ConfigureAwait(false));
        var account = await context.GetAccountByLoginNameAsync("newbie").ConfigureAwait(false);
        Assert.That(BCrypt.Net.BCrypt.Verify(reset, account!.PasswordHash), Is.True);
        Assert.That(entries.Items.Any(e => e.Operation == "accounts_set_state" && e.Outcome == "Failed"), Is.True);
    }

    /// <summary>Operators cannot use account editing or password reset to gain game master privileges.</summary>
    [Test]
    public async Task OperatorCannotPromoteAccountsAsync()
    {
        await using var client = await this._host.ConnectAsync("omu_operations-operator-test").ConfigureAwait(false);
        await client.CallToolAsync("accounts_create", new Dictionary<string, object?> { ["loginName"] = "newbie", ["password"] = "private-pass" }).ConfigureAwait(false);
        var promoted = await client.CallToolAsync("accounts_set_state", new Dictionary<string, object?> { ["loginName"] = "newbie", ["state"] = "GameMaster", ["reason"] = "Not permitted" }).ConfigureAwait(false);
        Assert.That(promoted.IsError, Is.True);
    }

    /// <summary>Reusing a reference creates exactly one pending grant and refuses changed arguments.</summary>
    [Test]
    public async Task CashGrantsAreIdempotentAsync()
    {
        await using var client = await this._host.ConnectAsync("omu_cashshop-key-for-mcp-integration-tests").ConfigureAwait(false);
        var args = new Dictionary<string, object?> { ["loginName"] = "tester", ["coinType"] = "WCoinC", ["amount"] = 10, ["reason"] = "Event reward", ["reference"] = "event-unique-reference" };
        Assert.That((await client.CallToolAsync("cashshop_grant", args).ConfigureAwait(false)).IsError, Is.Not.True);
        var repeated = await client.CallToolAsync("cashshop_grant", args).ConfigureAwait(false);
        Assert.That(repeated.IsError, Is.Not.True);
        Assert.That(JsonSerializer.Serialize(repeated), Does.Contain("AlreadyGranted"));
        args["amount"] = 11;
        Assert.That((await client.CallToolAsync("cashshop_grant", args).ConfigureAwait(false)).IsError, Is.True);
        var balance = await client.CallToolAsync("cashshop_get_balance", new Dictionary<string, object?> { ["loginName"] = "tester" }).ConfigureAwait(false);
        Assert.That(JsonSerializer.SerializeToNode(balance.StructuredContent)?["grants"]?.AsArray().Count, Is.EqualTo(1));
    }

    /// <summary>The complete enabled tool catalog has an explicit, reviewed protocol snapshot.</summary>
    [Test]
    public async Task OperationalSchemaMatchesSnapshotAsync()
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var tools = await client.ListToolsAsync().ConfigureAwait(false);
        var schema = JsonSerializer.SerializeToNode(tools.Where(t => t.Name != "test_admin_only").OrderBy(t => t.Name)
            .Select(t => new { t.Name, t.ProtocolTool.InputSchema, t.ProtocolTool.Annotations }));
        var expected = System.Text.Json.Nodes.JsonNode.Parse(await System.IO.File.ReadAllTextAsync(System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "Mcp", "operations-tools.json")).ConfigureAwait(false));
        Assert.That(System.Text.Json.Nodes.JsonNode.DeepEquals(schema, expected), Is.True, schema!.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>Operational errors keep their text and expose the invariant error kind over HTTP MCP.</summary>
    [TestCase(AdminOperationErrorKind.Conflict)]
    [TestCase(AdminOperationErrorKind.Forbidden)]
    [TestCase(AdminOperationErrorKind.InvalidInput)]
    [TestCase(AdminOperationErrorKind.Unavailable)]
    [TestCase(AdminOperationErrorKind.Uncertain)]
    public async Task OperationalErrorsIncludeKindAsync(AdminOperationErrorKind kind)
    {
        var server = this.InstallServer(9);
        server.Setup(s => s.StartAsync()).Returns(() => ValueTask.FromException(new AdminOperationException("Safe operational message.", kind)));
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("server_start", new Dictionary<string, object?> { ["serverId"] = 9, ["serverType"] = "GameServer" }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Content.OfType<TextContentBlock>().Single().Text, Is.EqualTo("An error occurred invoking 'server_start': Safe operational message."));
        Assert.That(result.Meta?["openmu/errorKind"]?.GetValue<string>(), Is.EqualTo(kind.ToString()));
    }

    /// <summary>Read validation uses the same machine-readable kind without changing its message.</summary>
    [Test]
    public async Task ReadValidationIncludesKindAsync()
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("accounts_get", new Dictionary<string, object?> { ["loginName"] = "missing" }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Meta?["openmu/errorKind"]?.GetValue<string>(), Is.EqualTo("InvalidInput"));
        Assert.That(result.Content.OfType<TextContentBlock>().Single().Text, Does.Contain("Account not found."));
    }

    /// <summary>Unexpected failures still use the SDK's generic error without disclosing host details.</summary>
    [Test]
    public async Task UnexpectedErrorsStayPrivateAsync()
    {
        var server = this.InstallServer(9);
        server.Setup(s => s.StartAsync()).Returns(() => ValueTask.FromException(new InvalidOperationException("private-database-details")));
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("server_start", new Dictionary<string, object?> { ["serverId"] = 9, ["serverType"] = "GameServer" }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That(JsonSerializer.Serialize(result), Does.Not.Contain("private-database-details"));
    }

    private ValueTask<McpClient> AdminAsync() => this._host.ConnectAsync("omu_mcp-admin-test");

    private Mock<IGameServer> InstallServer(int id)
    {
        var server = new Mock<IGameServer>();
        server.SetupGet(s => s.Id).Returns(id);
        server.SetupGet(s => s.Type).Returns(ServerType.GameServer);
        server.SetupGet(s => s.ServerState).Returns(ServerState.Stopped);
        var provider = Mock.Get(this._host.App.Services.GetRequiredService<IServerProvider>());
        provider.SetupGet(p => p.Servers).Returns(new List<IManageableServer> { server.Object });
        return server;
    }
}
