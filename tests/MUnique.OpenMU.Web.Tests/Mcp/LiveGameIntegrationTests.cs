// <copyright file="LiveGameIntegrationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Mcp;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.Tests.AdminAuth;

/// <summary>Exercises live tools over authenticated MCP HTTP, including routing and negative cases.</summary>
[TestFixture]
public sealed class LiveGameIntegrationTests
{
    private McpIntegrationTests _host = null!;
    private Mock<IGameServerAdministration> _live = null!;
    private InMemoryAdminAuditRepository _audit = null!;
    private LivePlayerInfo _player = null!;
    private AdminPluginOperationInfo _read = null!;
    private AdminPluginOperationInfo _write = null!;

    /// <summary>Starts a real MCP host and registers one remote-style live administration channel.</summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._host = new McpIntegrationTests { EnableWrites = true };
        await this._host.SetUpAsync().ConfigureAwait(false);
        this._audit = (InMemoryAdminAuditRepository)this._host.App.Services.GetRequiredService<IAdminAuditRepository>();
        var keys = this._host.App.Services.GetRequiredService<IApiKeyRepository>();
        await keys.AddAsync(new ApiKey { Id = Guid.NewGuid(), Name = "live-operator", KeyHash = ApiKeyGenerator.Hash("omu_live-operator"), Roles = AdminRoles.Operator }).ConfigureAwait(false);
        var server = new Mock<IGameServer>();
        server.SetupGet(s => s.Id).Returns(4);
        server.SetupGet(s => s.Type).Returns(ServerType.GameServer);
        server.SetupGet(s => s.ServerState).Returns(ServerState.Started);
        this._live = server.As<IGameServerAdministration>();
        this._player = new(Guid.NewGuid(), "tester", "Hero", 0, 100, 100, 10, 2, 42, false);
        this._live.Setup(s => s.GetPlayerAsync("Hero", It.IsAny<CancellationToken>())).ReturnsAsync(() => this._player);
        this._live.Setup(s => s.GetPlayerSnapshotAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { this._player });
        this._live.Setup(s => s.GetEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync([new("BloodCastle", "Blood Castle", true, false, TimeSpan.FromMinutes(5))]);
        this._live.Setup(s => s.SetLevelAsync("Hero", this._player.CharacterId, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(GameServerAdminResult.Success);
        this._live.Setup(s => s.TeleportAsync("Hero", this._player.CharacterId, It.IsAny<ushort>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<CancellationToken>())).ReturnsAsync(GameServerAdminResult.Success);
        this._live.Setup(s => s.GiveItemAsync("Hero", this._player.CharacterId, It.IsAny<ItemSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(GameServerAdminResult.Success);
        this._live.Setup(s => s.StartEventAsync("BloodCastle", false, It.IsAny<LiveEventInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(GameServerAdminResult.Success);
        this._read = new(Guid.NewGuid(), "read_state", "Read event state.", AdminRoles.Viewer, true, false, true, new JsonObject { ["type"] = "object" });
        this._write = new(Guid.NewGuid(), "reset_state", "Reset event state.", AdminRoles.Administrator, false, true, false, new JsonObject { ["type"] = "object" });
        this._live.Setup(s => s.GetAdminOperationsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => new[] { this._read, this._write });
        this._live.Setup(s => s.ReadAdminOperationAsync(this._read.PlugInId, this._read.Name, "{}", It.IsAny<CancellationToken>())).ReturnsAsync(new JsonObject { ["state"] = 1 });
        this._live.Setup(s => s.PreviewAdminOperationAsync(this._write.PlugInId, this._write.Name, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new JsonObject { ["before"] = 1, ["after"] = 0 });
        this._live.Setup(s => s.ExecuteAdminOperationAsync(this._write.PlugInId, this._write.Name, It.IsAny<string>(), It.IsAny<JsonObject>(), It.IsAny<CancellationToken>())).ReturnsAsync(new JsonObject { ["state"] = 0 });
        Mock.Get(this._host.App.Services.GetRequiredService<IServerProvider>()).SetupGet(s => s.Servers).Returns([server.Object]);
        var configuration = await this._host.App.Services.GetRequiredService<IDataSource<GameConfiguration>>().GetOwnerAsync().ConfigureAwait(false);
        configuration.MaximumLevel = 400;
        configuration.MaximumMasterLevel = 200;
        configuration.Maps.Single().TerrainData = new byte[ushort.MaxValue + 3];
        using var context = this._host.App.Services.GetRequiredService<IPersistenceContextProvider>().CreateNewContext();
        var definition = context.CreateNew<ItemDefinition>();
        definition.Group = 0;
        definition.Number = 1;
        definition.MaximumItemLevel = 15;
        configuration.Items.Add(definition);
    }

    /// <summary>Stops the host.</summary>
    [TearDown]
    public async Task TearDownAsync() => await this._host.TearDownAsync().ConfigureAwait(false);

    /// <summary>Preview has no side effects; apply runs on the owner under a durable pending audit record.</summary>
    [Test]
    public async Task LevelPreviewApplyRoutesAndAuditsOnceAsync()
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var arguments = new Dictionary<string, object?> { ["characterName"] = "Hero", ["level"] = 100 };
        var preview = await client.CallToolAsync("players_set_level", arguments).ConfigureAwait(false);
        Assert.That(preview.IsError, Is.Not.True, JsonSerializer.Serialize(preview));
        this._live.Verify(s => s.SetLevelAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
        this._live.Setup(s => s.SetLevelAsync("Hero", this._player.CharacterId, 100, false, It.IsAny<CancellationToken>())).Returns(async () =>
        {
            Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Pending"));
            return GameServerAdminResult.Success;
        });
        Confirm(arguments, preview);
        Assert.That((await client.CallToolAsync("players_set_level", arguments).ConfigureAwait(false)).IsError, Is.Not.True);
        Assert.That((await client.CallToolAsync("players_set_level", arguments).ConfigureAwait(false)).IsError, Is.True);
        this._live.Verify(s => s.SetLevelAsync("Hero", this._player.CharacterId, 100, false, It.IsAny<CancellationToken>()), Times.Once);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Succeeded"));
    }

    /// <summary>Tokens cannot authorize changed arguments or a different caller.</summary>
    [Test]
    public async Task ConfirmationIsBoundToArgumentsAndCallerAsync()
    {
        await using var admin = await this.AdminAsync().ConfigureAwait(false);
        await using var other = await this._host.ConnectAsync("omu_live-operator").ConfigureAwait(false);
        var args = new Dictionary<string, object?> { ["characterName"] = "Hero", ["level"] = 100 };
        var preview = await admin.CallToolAsync("players_set_level", args).ConfigureAwait(false);
        Confirm(args, preview);
        Assert.That((await other.CallToolAsync("players_set_level", args).ConfigureAwait(false)).IsError, Is.True);
        args["level"] = 101;
        Assert.That((await admin.CallToolAsync("players_set_level", args).ConfigureAwait(false)).IsError, Is.True);
        this._live.Verify(s => s.SetLevelAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Regular and master levels validate different configured maxima before preview.</summary>
    [TestCase(401, false)]
    [TestCase(201, true)]
    [TestCase(0, false)]
    public async Task RejectsInvalidLevelAsync(int level, bool master)
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("players_set_level", new Dictionary<string, object?> { ["characterName"] = "Hero", ["level"] = level, ["master"] = master }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
    }

    /// <summary>A teleport preview applies to the exact owning character and destination.</summary>
    [Test]
    public async Task TeleportUsesLiveChannelAsync()
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var args = new Dictionary<string, object?> { ["characterName"] = "Hero", ["mapNumber"] = 0, ["x"] = 120, ["y"] = 121 };
        Confirm(args, await client.CallToolAsync("players_teleport", args).ConfigureAwait(false));
        Assert.That((await client.CallToolAsync("players_teleport", args).ConfigureAwait(false)).IsError, Is.Not.True);
        this._live.Verify(s => s.TeleportAsync("Hero", this._player.CharacterId, 0, 120, 121, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>Changing live ownership invalidates previews and prevents targeting a different character.</summary>
    [Test]
    public async Task ChangedIdentityInvalidatesPreviewAsync()
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var args = new Dictionary<string, object?> { ["characterName"] = "Hero", ["mapNumber"] = 0, ["x"] = 120, ["y"] = 121 };
        var preview = await client.CallToolAsync("players_teleport", args).ConfigureAwait(false);
        Confirm(args, preview);
        this._player = this._player with { CharacterId = Guid.NewGuid() };
        Assert.That((await client.CallToolAsync("players_teleport", args).ConfigureAwait(false)).IsError, Is.True);
        this._live.Verify(s => s.TeleportAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ushort>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Privileged characters require Administrator, including during preview.</summary>
    [Test]
    public async Task OperatorCannotEditGameMasterAsync()
    {
        this._player = this._player with { Privileged = true };
        await using var client = await this._host.ConnectAsync("omu_live-operator").ConfigureAwait(false);
        var result = await client.CallToolAsync("players_set_level", new Dictionary<string, object?> { ["characterName"] = "Hero", ["level"] = 100 }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>Live item grants use preview and route a structured specification to the owner.</summary>
    [Test]
    public async Task ItemGrantUsesLiveChannelAsync()
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var item = new ItemSpecification(0, 1, 7);
        var args = new Dictionary<string, object?> { ["characterName"] = "Hero", ["item"] = item };
        var preview = await client.CallToolAsync("players_give_item", args).ConfigureAwait(false);
        Assert.That(preview.IsError, Is.Not.True, JsonSerializer.Serialize(preview));
        Confirm(args, preview);
        Assert.That((await client.CallToolAsync("players_give_item", args).ConfigureAwait(false)).IsError, Is.Not.True);
        this._live.Verify(s => s.GiveItemAsync("Hero", this._player.CharacterId, item, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>An unavailable audit database prevents event starts.</summary>
    [Test]
    public async Task AuditFailurePreventsLiveWriteAsync()
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var arguments = new Dictionary<string, object?> { ["serverId"] = 4, ["eventType"] = "BloodCastle" };
        Confirm(arguments, await client.CallToolAsync("events_start", arguments).ConfigureAwait(false));
        this._audit.RejectWrites = true;
        Assert.That((await client.CallToolAsync("events_start", arguments).ConfigureAwait(false)).IsError, Is.True);
        this._live.Verify(s => s.StartEventAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<LiveEventInfo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Transport failures preserve Pending rather than claiming that a live write failed or succeeded.</summary>
    [Test]
    public async Task LostResponseLeavesPendingAuditAsync()
    {
        this._live.Setup(s => s.StartEventAsync("BloodCastle", false, It.IsAny<LiveEventInfo>(), It.IsAny<CancellationToken>())).ThrowsAsync(new TimeoutException());
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var arguments = new Dictionary<string, object?> { ["serverId"] = 4, ["eventType"] = "BloodCastle" };
        Confirm(arguments, await client.CallToolAsync("events_start", arguments).ConfigureAwait(false));
        var result = await client.CallToolAsync("events_start", arguments).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That(JsonSerializer.Serialize(result), Does.Contain("outcome is unknown"));
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Pending"));
    }

    /// <summary>Server refusals are recorded as failures, and do not report a start request.</summary>
    [Test]
    public async Task EventRefusalIsAuditedAsync()
    {
        this._live.Setup(s => s.StartEventAsync("BloodCastle", false, It.IsAny<LiveEventInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GameServerAdminResult(false, "Event disabled."));
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var arguments = new Dictionary<string, object?> { ["serverId"] = 4, ["eventType"] = "BloodCastle" };
        Confirm(arguments, await client.CallToolAsync("events_start", arguments).ConfigureAwait(false));
        var result = await client.CallToolAsync("events_start", arguments).ConfigureAwait(false);
        Assert.That(result.IsError, Is.True);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Failed"));
    }

    /// <summary>Live reads and filters work through a channel without an in-process game context.</summary>
    [Test]
    public async Task RemoteLiveReadsAndFiltersAsync()
    {
        await using var client = await this._host.ConnectAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("players_list", new Dictionary<string, object?> { ["mapNumber"] = 0 }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.Not.True, JsonSerializer.Serialize(result));
        Assert.That(JsonSerializer.Serialize(result), Does.Contain("Hero"));
        Assert.That((await client.CallToolAsync("players_get", new Dictionary<string, object?> { ["characterName"] = "Hero" }).ConfigureAwait(false)).IsError, Is.Not.True);
        Assert.That((await client.CallToolAsync("events_schedule", new Dictionary<string, object?> { ["serverId"] = 4 }).ConfigureAwait(false)).IsError, Is.Not.True);
        Assert.ThrowsAsync<McpProtocolException>(async () => await client.CallToolAsync("players_set_level", new Dictionary<string, object?> { ["characterName"] = "Hero", ["level"] = 100 }).ConfigureAwait(false));
    }

    /// <summary>The read-only guard blocks writes and previews even when the tool was registered earlier.</summary>
    [Test]
    public async Task ReadOnlyBlocksLivePreviewAndEventStartAsync()
    {
        this._host.App.Services.GetRequiredService<AdminMcpOptions>().ReadOnly = true;
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        Assert.That((await client.CallToolAsync("players_set_level", new Dictionary<string, object?> { ["characterName"] = "Hero", ["level"] = 100 }).ConfigureAwait(false)).IsError, Is.True);
        Assert.That((await client.CallToolAsync("events_start", new Dictionary<string, object?> { ["serverId"] = 4, ["eventType"] = "BloodCastle" }).ConfigureAwait(false)).IsError, Is.True);
        this._live.Verify(s => s.StartEventAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<LiveEventInfo>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>Plugin reads are automatically reachable; writes require exact owner opt-in and their declared role.</summary>
    [Test]
    public async Task PluginCatalogAndExecutionRespectOwnerAndRoleAsync()
    {
        await using var admin = await this.AdminAsync().ConfigureAwait(false);
        var args = new Dictionary<string, object?> { ["serverId"] = 4 };
        var catalog = await admin.CallToolAsync("plugin_operations_list", args).ConfigureAwait(false);
        Assert.That(JsonSerializer.Serialize(catalog), Does.Contain("read_state").And.Not.Contain("reset_state"));
        var writeArgs = this.PluginWriteArguments();
        Assert.That((await admin.CallToolAsync("plugin_operations_execute", writeArgs).ConfigureAwait(false)).IsError, Is.True);
        this.AllowPluginWrite();
        catalog = await admin.CallToolAsync("plugin_operations_list", args).ConfigureAwait(false);
        Assert.That(JsonSerializer.Serialize(catalog), Does.Contain("reset_state"));
        await using var op = await this._host.ConnectAsync("omu_live-operator").ConfigureAwait(false);
        Assert.That((await op.CallToolAsync("plugin_operations_execute", writeArgs).ConfigureAwait(false)).IsError, Is.True);
        var preview = await admin.CallToolAsync("plugin_operations_execute", writeArgs).ConfigureAwait(false);
        Assert.That(preview.IsError, Is.Not.True, JsonSerializer.Serialize(preview));
        this._live.Verify(s => s.ExecuteAdminOperationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JsonObject>(), It.IsAny<CancellationToken>()), Times.Never);
        Confirm(writeArgs, preview);
        var applied = await admin.CallToolAsync("plugin_operations_execute", writeArgs).ConfigureAwait(false);
        Assert.That(applied.IsError, Is.Not.True);
        Assert.That(JsonSerializer.SerializeToNode(applied.StructuredContent)?["result"]?["state"]?.GetValue<int>(), Is.Zero);
        this._live.Verify(s => s.ExecuteAdminOperationAsync(this._write.PlugInId, this._write.Name, "{}", It.IsAny<JsonObject>(), It.IsAny<CancellationToken>()), Times.Once);
        var audit = (await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single();
        Assert.That(audit.Operation, Is.EqualTo("plugin_operations_execute"));
        Assert.That(JsonNode.Parse(audit.ResultJson)?["result"]?["state"]?.GetValue<int>(), Is.Zero);
    }

    /// <summary>Writes cannot be smuggled through read-only tools; revoking owner permission invalidates apply.</summary>
    [Test]
    public async Task PluginReadCannotWriteAndRevocationPreventsApplyAsync()
    {
        this.AllowPluginWrite();
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var read = await client.CallToolAsync("plugin_operations_read", new Dictionary<string, object?> { ["serverId"] = 4, ["plugInId"] = this._read.PlugInId, ["name"] = this._read.Name }).ConfigureAwait(false);
        Assert.That(read.IsError, Is.Not.True);
        var wrong = await client.CallToolAsync("plugin_operations_read", new Dictionary<string, object?> { ["serverId"] = 4, ["plugInId"] = this._write.PlugInId, ["name"] = this._write.Name }).ConfigureAwait(false);
        Assert.That(wrong.IsError, Is.True);
        var args = this.PluginWriteArguments();
        Confirm(args, await client.CallToolAsync("plugin_operations_execute", args).ConfigureAwait(false));
        this._host.App.Services.GetRequiredService<AdminWriteSettings>().AllowedPluginWrites.Clear();
        Assert.That((await client.CallToolAsync("plugin_operations_execute", args).ConfigureAwait(false)).IsError, Is.True);
        this._live.Verify(s => s.ExecuteAdminOperationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<JsonObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Restart previews show impact, bind the caller and execute only once after confirmation.</summary>
    [Test]
    public async Task EventRestartRequiresBoundSingleUsePreviewAsync()
    {
        this._live.Setup(s => s.GetEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync([new("BloodCastle", "Blood Castle", true, true, null, 37)]);
        this._live.Setup(s => s.StartEventAsync("BloodCastle", true, It.IsAny<LiveEventInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(GameServerAdminResult.Success);
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var arguments = new Dictionary<string, object?> { ["serverId"] = 4, ["eventType"] = "BloodCastle", ["restart"] = true };
        var preview = await client.CallToolAsync("events_start", arguments).ConfigureAwait(false);
        Assert.That(JsonSerializer.Serialize(preview), Does.Contain("37").And.Contain("removesExistingParticipants"));
        this._live.Verify(s => s.StartEventAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<LiveEventInfo>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
        arguments["apply"] = true;
        Assert.That((await client.CallToolAsync("events_start", arguments).ConfigureAwait(false)).IsError, Is.True);
        Confirm(arguments, preview);
        await using var other = await this._host.ConnectAsync("omu_live-operator").ConfigureAwait(false);
        Assert.That((await other.CallToolAsync("events_start", arguments).ConfigureAwait(false)).IsError, Is.True);
        this._live.Verify(s => s.StartEventAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<LiveEventInfo>(), It.IsAny<CancellationToken>()), Times.Never);
        arguments.Remove("apply");
        arguments.Remove("confirmationToken");
        Confirm(arguments, await client.CallToolAsync("events_start", arguments).ConfigureAwait(false));
        Assert.That((await client.CallToolAsync("events_start", arguments).ConfigureAwait(false)).IsError, Is.Not.True);
        Assert.That((await client.CallToolAsync("events_start", arguments).ConfigureAwait(false)).IsError, Is.True);
        this._live.Verify(s => s.StartEventAsync("BloodCastle", true, It.Is<LiveEventInfo>(e => e.Active && e.Participants == 37), It.IsAny<CancellationToken>()), Times.Once);
        Assert.That((await this._audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Succeeded"));
    }

    /// <summary>Changed event state or arguments require a new preview before removing participants.</summary>
    [Test]
    public async Task EventChangesInvalidateConfirmationAsync()
    {
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var arguments = new Dictionary<string, object?> { ["serverId"] = 4, ["eventType"] = "BloodCastle", ["restart"] = true };
        Confirm(arguments, await client.CallToolAsync("events_start", arguments).ConfigureAwait(false));
        this._live.Setup(s => s.GetEventsAsync(It.IsAny<CancellationToken>())).ReturnsAsync([new("BloodCastle", "Blood Castle", true, true, null, 5)]);
        Assert.That((await client.CallToolAsync("events_start", arguments).ConfigureAwait(false)).IsError, Is.True);
        arguments.Remove("apply");
        arguments.Remove("confirmationToken");
        Confirm(arguments, await client.CallToolAsync("events_start", arguments).ConfigureAwait(false));
        arguments["restart"] = false;
        Assert.That((await client.CallToolAsync("events_start", arguments).ConfigureAwait(false)).IsError, Is.True);
        this._live.Verify(s => s.StartEventAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<LiveEventInfo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>One listing fetches a 5,000-player server snapshot once, then filters and pages locally.</summary>
    [Test]
    public async Task LargePopulationUsesOneSnapshotAsync()
    {
        var population = Enumerable.Range(0, 5000).Select(i => this._player with { Account = $"account{i:D4}", Character = $"Hero{i:D4}", Map = (ushort)(i % 2) }).ToArray();
        Mock.Get(this._host.App.Services.GetRequiredService<ILoginServer>()).Setup(s => s.GetSnapshotAsync())
            .ReturnsAsync(population.ToDictionary(p => p.Account, _ => (byte)4));
        this._live.Setup(s => s.GetPlayerSnapshotAsync(It.IsAny<CancellationToken>())).ReturnsAsync(population);
        await using var client = await this.AdminAsync().ConfigureAwait(false);
        var result = await client.CallToolAsync("players_list", new Dictionary<string, object?> { ["serverId"] = 4, ["mapNumber"] = 1, ["offset"] = 2400, ["limit"] = 100 }).ConfigureAwait(false);
        Assert.That(result.IsError, Is.Not.True, JsonSerializer.Serialize(result));
        var content = JsonSerializer.SerializeToNode(result.StructuredContent)!;
        Assert.That(content["total"]!.GetValue<int>(), Is.EqualTo(2500));
        Assert.That(content["items"]!.AsArray(), Has.Count.EqualTo(100));
        Assert.That(content["items"]![0]!["account"]!.GetValue<string>(), Is.EqualTo("account4801"));
        this._live.Verify(s => s.GetPlayerSnapshotAsync(It.IsAny<CancellationToken>()), Times.Once);
        this._live.Verify(s => s.GetPlayersAsync(It.IsAny<ushort?>(), It.IsAny<uint?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static void Confirm(Dictionary<string, object?> arguments, CallToolResult preview)
    {
        Assert.That(preview.IsError, Is.Not.True, JsonSerializer.Serialize(preview));
        arguments["apply"] = true;
        arguments["confirmationToken"] = JsonSerializer.SerializeToNode(preview.StructuredContent)!["confirmationToken"]!.GetValue<string>();
    }

    private void AllowPluginWrite() => this._host.App.Services.GetRequiredService<AdminWriteSettings>().AllowedPluginWrites[this._write.PlugInId.ToString("D")] = [this._write.Name];

    private Dictionary<string, object?> PluginWriteArguments() => new() { ["serverId"] = 4, ["plugInId"] = this._write.PlugInId, ["name"] = this._write.Name, ["arguments"] = "{}" };

    private ValueTask<McpClient> AdminAsync() => this._host.ConnectAsync("omu_mcp-admin-test");
}
