// <copyright file="GameServerAdministrationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Administration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>Verifies live mutations use player state and reject stale identities and invalid item options.</summary>
[TestFixture]
public sealed class GameServerAdministrationTests
{
    private Player _player = null!;
    private GameServerAdministration _administration = null!;

    /// <summary>Creates a live player and a context which resolves that player by name.</summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        this._player.Account!.LoginName = "tester";
        this._player.SelectedCharacter!.Name = "Hero";
        this._player.SelectedCharacter.Id = Guid.NewGuid();
        this._player.GameContext.Configuration.MaximumLevel = 400;
        this._player.GameContext.Configuration.MaximumMasterLevel = 200;
        var context = new Mock<IGameContext>();
        context.Setup(c => c.GetPlayerByCharacterName("Hero")).Returns(this._player);
        context.Setup(c => c.GetPlayersAsync()).ReturnsAsync(new List<Player> { this._player });
        context.SetupGet(c => c.PlugInManager).Returns(this._player.GameContext.PlugInManager);
        context.SetupGet(c => c.MiniGames).Returns(this._player.GameContext.MiniGames);
        context.SetupGet(c => c.Configuration).Returns(this._player.GameContext.Configuration);
        this._administration = new(context.Object);
    }

    /// <summary>Disposes the test player and its game context.</summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        await this._player.DisposeAsync().ConfigureAwait(false);
        await ((GameContext)this._player.GameContext).DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>The channel updates the live attribute collection for regular and master levels.</summary>
    [TestCase(false, 100)]
    [TestCase(true, 50)]
    public async Task SetsLiveLevelAsync(bool master, int level)
    {
        var result = await this._administration.SetLevelAsync("Hero", this._player.SelectedCharacter!.Id, level, master, CancellationToken.None).ConfigureAwait(false);
        Assert.That(result.Succeeded, Is.True, result.Error);
        Assert.That(this._player.Attributes![master ? Stats.MasterLevel : Stats.Level], Is.EqualTo(level));
    }

    /// <summary>Stale identities and out-of-range levels do not mutate live stats.</summary>
    [Test]
    public async Task RejectsStaleIdentityAndInvalidLevelAsync()
    {
        var before = this._player.Level;
        Assert.That((await this._administration.SetLevelAsync("Hero", Guid.NewGuid(), 100, false, CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.False);
        Assert.That((await this._administration.SetLevelAsync("Hero", this._player.SelectedCharacter!.Id, 401, false, CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.False);
        Assert.That(this._player.Level, Is.EqualTo(before));
    }

    /// <summary>Item delivery creates a tracked item in the live player's own context and notifies through inventory actions.</summary>
    [Test]
    public async Task GivesPersistentItemToLiveInventoryAsync()
    {
        var definition = this.AddItemDefinition();
        var result = await this._administration.GiveItemAsync("Hero", this._player.SelectedCharacter!.Id, new(0, 1, 7), CancellationToken.None).ConfigureAwait(false);
        Assert.That(result.Succeeded, Is.True, result.Error);
        var item = this._player.Inventory!.Items.Single();
        Assert.That(item.Definition, Is.SameAs(definition));
        Assert.That(item.Level, Is.EqualTo(7));
        Assert.That(item, Is.Not.TypeOf<TemporaryItem>());
        Assert.That(item.GetId(), Is.Not.EqualTo(Guid.Empty));
    }

    /// <summary>Invalid options are rejected before an item is created.</summary>
    [Test]
    public async Task RejectsUnsupportedOptionsAsync()
    {
        this.AddItemDefinition();
        var id = this._player.SelectedCharacter!.Id;
        foreach (var item in new[] { new ItemSpecification(0, 1, 16), new(0, 1, Luck: true), new(0, 1, Skill: true), new(0, 1, OptionLevel: 7), new(0, 1, ExcellentOptions: 1), new(0, 1, AncientSet: 1) })
        {
            Assert.That((await this._administration.GiveItemAsync("Hero", id, item, CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.False);
        }

        Assert.That(this._player.Inventory!.Items, Is.Empty);
    }

    /// <summary>Reads filter before paging and never substitute persisted state for a disconnected character.</summary>
    [Test]
    public async Task FiltersAndRejectsUnstableSessionsAsync()
    {
        var page = await this._administration.GetPlayersAsync(null, null, 0, 1, CancellationToken.None).ConfigureAwait(false);
        Assert.That(page.Total, Is.EqualTo(1));
        Assert.That(page.Items.Single().Character, Is.EqualTo("Hero"));
        Assert.That((await this._administration.GetPlayersAsync(42, null, 0, 1, CancellationToken.None).ConfigureAwait(false)).Total, Is.Zero);
        await this._player.PlayerState.TryAdvanceToAsync(PlayerState.NpcDialogOpened).ConfigureAwait(false);
        Assert.That(await this._administration.GetPlayerAsync("Hero", CancellationToken.None).ConfigureAwait(false), Is.Null);
        Assert.That((await this._administration.SetLevelAsync("Hero", this._player.SelectedCharacter!.Id, 100, false, CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.False);
    }

    /// <summary>Live administration waits for the same player lock as persistence and packet handlers.</summary>
    [Test]
    public async Task SerializesWithPlayerPersistenceAsync()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var holding = Task.Run(async () => await this._player.RunPersistenceExclusiveAsync(async () =>
        {
            entered.SetResult();
            await release.Task.ConfigureAwait(false);
        }).ConfigureAwait(false));
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        var changing = this._administration.SetLevelAsync("Hero", this._player.SelectedCharacter!.Id, 100, false, CancellationToken.None).AsTask();
        try
        {
            Assert.That(changing.IsCompleted, Is.False);
        }
        finally
        {
            release.SetResult();
            await holding.ConfigureAwait(false);
        }

        Assert.That((await changing.ConfigureAwait(false)).Succeeded, Is.True);
    }

    /// <summary>Only active plugin operations are listed; read calls cannot execute a write.</summary>
    [Test]
    public async Task PluginDiscoveryAndReadIsolationAsync()
    {
        var plugin = new TestAdminOperation();
        this._player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IAdminOperationPlugIn>(plugin);
        var catalog = await this._administration.GetAdminOperationsAsync(CancellationToken.None).ConfigureAwait(false);
        Assert.That(catalog.Single().Name, Is.EqualTo(plugin.Key));
        Assert.ThrowsAsync<ArgumentException>(async () => await this._administration.ReadAdminOperationAsync(plugin.GetType().GUID, plugin.Key, "{}", CancellationToken.None).ConfigureAwait(false));
        Assert.That(plugin.Executions, Is.Zero);
        var preview = await this._administration.PreviewAdminOperationAsync(plugin.GetType().GUID, plugin.Key, "{}", CancellationToken.None).ConfigureAwait(false);
        plugin.State = 2;
        Assert.ThrowsAsync<ArgumentException>(async () => await this._administration.ExecuteAdminOperationAsync(plugin.GetType().GUID, plugin.Key, "{}", preview, CancellationToken.None).ConfigureAwait(false));
        Assert.That(plugin.Executions, Is.Zero);
        preview = await this._administration.PreviewAdminOperationAsync(plugin.GetType().GUID, plugin.Key, "{}", CancellationToken.None).ConfigureAwait(false);
        await this._administration.ExecuteAdminOperationAsync(plugin.GetType().GUID, plugin.Key, "{}", preview, CancellationToken.None).ConfigureAwait(false);
        Assert.That(plugin.Executions, Is.EqualTo(1));
        this._player.GameContext.PlugInManager.DeactivatePlugIn<TestAdminOperation>();
        Assert.That(await this._administration.GetAdminOperationsAsync(CancellationToken.None).ConfigureAwait(false), Is.Empty);
    }

    /// <summary>Teleportation uses normal movement and rejects blocked or dead destinations.</summary>
    [Test]
    public async Task TeleportChecksLiveTerrainAndLifeAsync()
    {
        this._player.IsAlive = true;
        var map = this._player.CurrentMap!;
        map.Terrain.WalkMap[120, 121] = false;
        var id = this._player.SelectedCharacter!.Id;
        Assert.That((await this._administration.TeleportAsync("Hero", id, map.MapId, 120, 121, CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.False);
        map.Terrain.WalkMap[120, 121] = true;
        Assert.That((await this._administration.TeleportAsync("Hero", id, map.MapId, 120, 121, CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.True);
        Assert.That(this._player.Position.X, Is.EqualTo(120));
        this._player.IsAlive = false;
        Assert.That((await this._administration.TeleportAsync("Hero", id, map.MapId, 100, 100, CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.False);
    }

    /// <summary>Disabled events are untouched; restarting an active event requires explicit intent.</summary>
    [Test]
    public async Task EventStartChecksScopeAndRestartAsync()
    {
        var starter = new TestEventStarter();
        this._player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPeriodicMiniGameStartPlugIn>(starter);
        Assert.That((await this._administration.StartEventAsync("BloodCastle", false, new("BloodCastle", "Blood Castle", true, true, null), CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.False);
        Assert.That(starter.Disposals, Is.Zero);
        starter.Enabled = true;
        starter.Active = true;
        Assert.That((await this._administration.StartEventAsync("BloodCastle", false, new("BloodCastle", "Blood Castle", true, true, null), CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.False);
        Assert.That(starter.Disposals, Is.Zero);
        Assert.That((await this._administration.StartEventAsync("BloodCastle", true, new("BloodCastle", "Blood Castle", true, true, null), CancellationToken.None).ConfigureAwait(false)).Succeeded, Is.True);
        Assert.That(starter.Disposals, Is.EqualTo(1));
        Assert.That(starter.Starts, Is.EqualTo(1));
    }

    /// <summary>A population read completes while progress persistence holds the player lock.</summary>
    [Test]
    public async Task SnapshotDoesNotWaitForPlayerPersistenceAsync()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var holding = Task.Run(async () => await this._player.RunPersistenceExclusiveAsync(async () =>
        {
            entered.SetResult();
            await release.Task.ConfigureAwait(false);
        }).ConfigureAwait(false));
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        try
        {
            var snapshot = await this._administration.GetPlayerSnapshotAsync(CancellationToken.None).AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(snapshot.Single().Character, Is.EqualTo("Hero"));
        }
        finally
        {
            release.SetResult();
            await holding.ConfigureAwait(false);
        }
    }

    /// <summary>The game-side event gate rejects stale impact state even after a front-end check.</summary>
    [Test]
    public async Task EventRejectsStalePreviewAsync()
    {
        var starter = new TestEventStarter { Enabled = true, Active = true };
        this._player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPeriodicMiniGameStartPlugIn>(starter);
        foreach (var expected in new[] { new LiveEventInfo("BloodCastle", "Blood Castle", true, false, null), new LiveEventInfo("BloodCastle", "Blood Castle", true, true, null, 3) })
        {
            var result = await this._administration.StartEventAsync("BloodCastle", true, expected, CancellationToken.None).ConfigureAwait(false);
            Assert.That(result.Succeeded, Is.False);
        }

        Assert.That(starter.Disposals, Is.Zero);
        Assert.That(starter.Starts, Is.Zero);
    }

    private ItemDefinition AddItemDefinition()
    {
        var item = this._player.PersistenceContext.CreateNew<ItemDefinition>();
        item.Group = 0;
        item.Number = 1;
        item.MaximumItemLevel = 15;
        item.Width = 1;
        item.Height = 1;
        item.Durability = 20;
        this._player.GameContext.Configuration.Items.Add(item);
        return item;
    }

    /// <summary>A controllable event starter used to verify live start safety.</summary>
    [Guid("D9A9F7B4-1543-43DD-B13D-97BDC137D71C")]
    private sealed class TestEventStarter : IPeriodicMiniGameStartPlugIn
    {
        /// <summary>Gets or sets whether the event is enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets whether the event is running.</summary>
        public bool Active { get; set; }

        /// <summary>Gets the number of start requests.</summary>
        public int Starts { get; private set; }

        /// <summary>Gets the number of cleanup requests.</summary>
        public int Disposals { get; private set; }

        /// <inheritdoc />
        public MUnique.OpenMU.DataModel.Configuration.MiniGameType Key => MUnique.OpenMU.DataModel.Configuration.MiniGameType.BloodCastle;

#pragma warning disable S1172 // This test double must keep the parameters required by the plugin interface.
        bool IPeriodicMiniGameStartPlugIn.IsEnabledOn(IGameContext gameContext) => this.Enabled;
#pragma warning restore S1172

#pragma warning disable S1172 // This test double must keep the parameters required by the plugin interface.
        bool IPeriodicMiniGameStartPlugIn.IsEventActive(IGameContext gameContext) => this.Active;
#pragma warning restore S1172

        /// <inheritdoc />
        public void ForceStart() => this.Starts++;

#pragma warning disable S1172 // This test double must keep the parameters required by the plugin interface.
        ValueTask IPeriodicMiniGameStartPlugIn.DisposeRunningGamesAsync(IGameContext gameContext)
        {
            this.Disposals++;
            return ValueTask.CompletedTask;
        }
#pragma warning restore S1172

#pragma warning disable S1172 // This test double must keep the parameters required by the plugin interface.
        ValueTask MUnique.OpenMU.GameLogic.PlugIns.IPeriodicTaskPlugIn.ExecuteTaskAsync(GameContext gameContext) => ValueTask.CompletedTask;
#pragma warning restore S1172

#pragma warning disable S1172 // This test double must keep the parameters required by the plugin interface.
        ValueTask<TimeSpan?> IPeriodicMiniGameStartPlugIn.GetDurationUntilNextStartAsync(IGameContext gameContext, MUnique.OpenMU.DataModel.Configuration.MiniGameDefinition miniGameDefinition)
            => ValueTask.FromResult<TimeSpan?>(null);
#pragma warning restore S1172

#pragma warning disable S1172 // This test double must keep the parameters required by the plugin interface.
        ValueTask<MiniGameContext?> IPeriodicMiniGameStartPlugIn.GetMiniGameContextAsync(IGameContext gameContext, MUnique.OpenMU.DataModel.Configuration.MiniGameDefinition miniGameDefinition)
            => ValueTask.FromResult<MiniGameContext?>(null);
#pragma warning restore S1172
    }

    /// <summary>A contributed write operation used to verify preview and read-channel isolation.</summary>
    [Guid("C99A0D71-F911-4E40-9411-B9DE7E1CB838")]
    private sealed class TestAdminOperation : IAdminOperationPlugIn
    {
        /// <summary>Gets or sets the previewed state.</summary>
        public int State { get; set; } = 1;

        /// <summary>Gets the number of applied writes.</summary>
        public int Executions { get; private set; }

        /// <inheritdoc />
        public string Key => "test_reset";

        /// <inheritdoc />
        public AdminPluginOperationInfo Operation => new(this.GetType().GUID, this.Key, "Reset test state.", "Administrator", false, true, false, new JsonObject { ["type"] = "object" });

#pragma warning disable S1172 // This test double must keep the parameters required by the plugin interface.
        ValueTask<JsonObject> IAdminOperationPlugIn.PreviewAsync(IGameContext context, JsonObject arguments, CancellationToken cancellationToken)
            => ValueTask.FromResult(new JsonObject { ["before"] = this.State, ["after"] = 0 });
#pragma warning restore S1172

#pragma warning disable S1172 // This test double must keep the parameters required by the plugin interface.
        ValueTask<JsonObject> IAdminOperationPlugIn.ExecuteAsync(IGameContext context, JsonObject arguments, JsonObject? expectedPreview, CancellationToken cancellationToken)
        {
            if (expectedPreview?["before"]?.GetValue<int>() != this.State)
            {
                throw new ArgumentException("The approved state changed.", nameof(expectedPreview));
            }

            this.Executions++;
            this.State = 0;
            return ValueTask.FromResult(new JsonObject { ["state"] = 0 });
        }
#pragma warning restore S1172
    }
}
