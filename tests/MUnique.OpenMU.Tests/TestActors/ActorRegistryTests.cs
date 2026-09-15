// <copyright file="ActorRegistryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.TestActors;

using System.Threading.Tasks;
using System.Threading;
using MUnique.OpenMU.GameLogic.TestActors;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Tests;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

/// <summary>
/// Tests the registry: one actor per account, whatever else already animates it.
/// </summary>
[TestFixture]
public class ActorRegistryTests
{
    /// <summary>
    /// An account which is held by the login server (a connected client, or an actor on another
    /// game server) is refused.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task AccountHeldByTheLoginServerIsRefusedAsync()
    {
        var fixture = await Fixture.CreateAsync(connectedAccounts: ["test1"]).ConfigureAwait(false);

        var result = await fixture.Registry.SpawnAsync(0, "test1", null).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.InUse));
        Assert.That(fixture.FactoryCalls, Is.EqualTo(0));
    }

    /// <summary>
    /// An account animated by a population bot is refused, although bots never touch the login
    /// server.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task AccountAnimatedByABotIsRefusedAsync()
    {
        var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var bot = await PlayerTestHelper.CreatePlayerAsync(fixture.GameContext).ConfigureAwait(false);
        bot.Account!.LoginName = "test1";
        bot.Account.IsBot = true;
        fixture.PlayersInWorld.Add(bot);

        var result = await fixture.Registry.SpawnAsync(0, "test1", null).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.InUse));
        Assert.That(fixture.FactoryCalls, Is.EqualTo(0));
    }

    /// <summary>
    /// A bot on ANOTHER game server of this process is found too: the bot population is split over
    /// the servers, and two players driving one character would corrupt it.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task AccountAnimatedOnAnotherGameServerIsRefusedAsync()
    {
        var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var bot = await PlayerTestHelper.CreatePlayerAsync(fixture.GameContext).ConfigureAwait(false);
        bot.Account!.LoginName = "bot0001";
        bot.Account.IsBot = true;
        fixture.PlayersOnOtherServer.Add(bot);

        var result = await fixture.Registry.SpawnAsync(0, "bot0001", null).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.InUse));
        Assert.That(result.Error, Does.Contain("a bot on game server 1"));
        Assert.That(fixture.FactoryCalls, Is.EqualTo(0));
    }

    /// <summary>
    /// A game server this process does not host is reported as such.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task UnknownGameServerIsReportedAsync()
    {
        var fixture = await Fixture.CreateAsync().ConfigureAwait(false);

        var result = await fixture.Registry.SpawnAsync(7, "test1", null).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.UnknownServer));
    }

    /// <summary>
    /// Ten concurrent spawns of one account produce exactly one actor.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ConcurrentSpawnsProduceExactlyOneActorAsync()
    {
        var fixture = await Fixture.CreateAsync().ConfigureAwait(false);

        var results = await Task.WhenAll(Enumerable.Range(0, 10)
            .Select(_ => Task.Run(async () => await fixture.Registry.SpawnAsync(0, "test1", null).ConfigureAwait(false))))
            .ConfigureAwait(false);

        Assert.That(results.Count(r => r.Ok), Is.EqualTo(1));
        Assert.That(results.Where(r => !r.Ok).Select(r => r.Code), Is.All.EqualTo(ActorErrorCodes.InUse));
        Assert.That(fixture.Registry.List().Count, Is.EqualTo(1));
        Assert.That(fixture.FactoryCalls, Is.EqualTo(1));
    }

    /// <summary>
    /// A spawned actor is found by its login name, and stopping it takes it out of the list.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task SpawnStopRoundTripAsync()
    {
        var fixture = await Fixture.CreateAsync().ConfigureAwait(false);

        var spawn = await fixture.Registry.SpawnAsync(0, "test1", null).ConfigureAwait(false);
        Assert.That(spawn.Ok, Is.True, spawn.Error);
        Assert.That(fixture.Registry.Find("test1"), Is.Not.Null);

        var stop = await fixture.Registry.StopAsync("test1").ConfigureAwait(false);

        Assert.That(stop.Ok, Is.True, stop.Error);
        Assert.That(fixture.Registry.Find("test1"), Is.Null);
        Assert.That(fixture.Registry.List(), Is.Empty);

        var unknown = await fixture.Registry.StopAsync("test1").ConfigureAwait(false);
        Assert.That(unknown.Code, Is.EqualTo(ActorErrorCodes.UnknownActor));
    }

    /// <summary>
    /// The shutdown path stops every actor.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task StopAllStopsEveryActorAsync()
    {
        var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        await fixture.Registry.SpawnAsync(0, "test1", null).ConfigureAwait(false);
        await fixture.Registry.SpawnAsync(0, "test2", null).ConfigureAwait(false);

        var stopped = await fixture.Registry.StopAllAsync().ConfigureAwait(false);

        Assert.That(stopped, Is.EqualTo(2));
        Assert.That(fixture.Registry.List(), Is.Empty);
    }

    private sealed class Fixture
    {
        private Fixture(IGameContext gameContext, ActorRegistry registry, List<Player> playersInWorld, List<Player> playersOnOtherServer, Func<int> factoryCalls)
        {
            this.GameContext = gameContext;
            this.Registry = registry;
            this.PlayersInWorld = playersInWorld;
            this.PlayersOnOtherServer = playersOnOtherServer;
            this.FactoryCallCounter = factoryCalls;
        }

        public IGameContext GameContext { get; }

        public ActorRegistry Registry { get; }

        public List<Player> PlayersInWorld { get; }

        public List<Player> PlayersOnOtherServer { get; }

        public int FactoryCalls => this.FactoryCallCounter();

        private Func<int> FactoryCallCounter { get; }

        public static async ValueTask<Fixture> CreateAsync(IEnumerable<string>? connectedAccounts = null)
        {
            var gameContext = ActorTestHelper.CreateGameContext();
            var playersInWorld = new List<Player>();
            var playersOnOtherServer = new List<Player>();
            var loginServer = new FakeLoginServer(connectedAccounts);

            var serverContext = new Mock<IGameServerContext>();
            serverContext.Setup(c => c.Id).Returns(0);
            serverContext.Setup(c => c.LoginServer).Returns(loginServer);
            serverContext.Setup(c => c.GetPlayersAsync()).Returns(() => ValueTask.FromResult<IList<Player>>(playersInWorld.ToList()));

            // A second game server of the same process, as the local stack runs it.
            var otherContext = new Mock<IGameServerContext>();
            otherContext.Setup(c => c.Id).Returns((byte)1);
            otherContext.Setup(c => c.LoginServer).Returns(loginServer);
            otherContext.Setup(c => c.GetPlayersAsync()).Returns(() => ValueTask.FromResult<IList<Player>>(playersOnOtherServer.ToList()));

            var locator = new Mock<IGameServerContextLocator>();
            locator.Setup(l => l.GetContext(0)).Returns(serverContext.Object);
            locator.Setup(l => l.GetContext(It.Is<int>(id => id != 0))).Returns((IGameServerContext?)null);
            locator.Setup(l => l.GetContexts()).Returns([(0, serverContext.Object), (1, otherContext.Object)]);

            var factory = new CountingActorFactory(gameContext);
            var registry = new ActorRegistry(locator.Object, factory, new NullLogger<ActorRegistry>());
            await Task.CompletedTask.ConfigureAwait(false);
            return new Fixture(gameContext, registry, playersInWorld, playersOnOtherServer, () => factory.Calls);
        }
    }

    private sealed class CountingActorFactory : IActorFactory
    {
        private readonly IGameContext _gameContext;
        private int _calls;

        public CountingActorFactory(IGameContext gameContext)
        {
            this._gameContext = gameContext;
        }

        public int Calls => this._calls;

        public async ValueTask<ScriptedPlayer?> CreateAsync(IGameServerContext context, string loginName, byte? characterSlot)
        {
            var call = Interlocked.Increment(ref this._calls);
            return await ActorTestHelper.CreateActorAsync(this._gameContext, loginName, $"Actor{call}").ConfigureAwait(false);
        }
    }

    private sealed class FakeLoginServer : ILoginServer
    {
        private readonly HashSet<string> _connected;

        public FakeLoginServer(IEnumerable<string>? connectedAccounts)
        {
            this._connected = new HashSet<string>(connectedAccounts ?? [], StringComparer.OrdinalIgnoreCase);
        }

        public Task<bool> TryLoginAsync(string accountName, byte serverId)
        {
            lock (this._connected)
            {
                return Task.FromResult(this._connected.Add(accountName));
            }
        }

        public ValueTask LogOffAsync(string accountName, byte serverId)
        {
            lock (this._connected)
            {
                this._connected.Remove(accountName);
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask<Dictionary<string, byte>> GetSnapshotAsync()
        {
            lock (this._connected)
            {
                return ValueTask.FromResult(this._connected.ToDictionary(a => a, _ => (byte)0));
            }
        }
    }
}
