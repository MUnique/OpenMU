// <copyright file="DuelHealthUpdatePlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.GameServer.RemoteView.Duel;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for <see cref="DuelHealthUpdatePlugIn"/>.
/// </summary>
[TestFixture]
public class DuelHealthUpdatePlugInTests
{
    private const ushort RequesterId = 0x1001;
    private const ushort OpponentId = 0x2002;

    /// <summary>
    /// Verifies the reproduction scenario from issue #1622: a spectator receives the second fighter's
    /// full health and shield as 100% against the second fighter's own maximums, not against the first
    /// fighter's larger maximums.
    /// </summary>
    [Test]
    public async Task SpectatorReceivesSecondFighterPercentagesAgainstOwnMaximumsForIssue1622Async()
    {
        var context = CreateGameServerContext();
        var (spectator, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(context);
        var (requester, _) = await CreatePlayerWithStatsAsync(context, RequesterId, 120614, 120614, 169442, 169442).ConfigureAwait(false);
        var (opponent, _) = await CreatePlayerWithStatsAsync(context, OpponentId, 110731, 110731, 163780, 163780).ConfigureAwait(false);
        await using var duelRoom = CreateDuelRoom(requester, opponent);

        await new DuelHealthUpdatePlugIn(spectator).UpdateHealthAsync(duelRoom).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(DuelHealthUpdate.Length));
        DuelHealthUpdate packet = data.AsMemory();
        Assert.Multiple(() =>
        {
            Assert.That(packet.Header.Type, Is.EqualTo(DuelHealthUpdate.HeaderType));
            Assert.That(packet.Header.Code, Is.EqualTo(DuelHealthUpdate.Code));
            Assert.That(packet.Header.Length, Is.EqualTo(DuelHealthUpdate.Length));
            Assert.That(packet.Header.SubCode, Is.EqualTo(DuelHealthUpdate.SubCode));
            Assert.That(packet.Player1Id, Is.EqualTo(RequesterId));
            Assert.That(packet.Player2Id, Is.EqualTo(OpponentId));
            Assert.That(packet.Player1HealthPercentage, Is.EqualTo(100));
            Assert.That(packet.Player2HealthPercentage, Is.EqualTo(100));
            Assert.That(packet.Player1ShieldPercentage, Is.EqualTo(100));
            Assert.That(packet.Player2ShieldPercentage, Is.EqualTo(100));
        });
    }

    /// <summary>
    /// Verifies that a spectator receives accurate independent health and shield percentages when both
    /// duelists have different maximums and partial values, including float-precision edge values.
    /// </summary>
    /// <param name="requesterCurrentHp">The requester's current health.</param>
    /// <param name="requesterMaxHp">The requester's maximum health.</param>
    /// <param name="requesterCurrentSd">The requester's current shield.</param>
    /// <param name="requesterMaxSd">The requester's maximum shield.</param>
    /// <param name="opponentCurrentHp">The opponent's current health.</param>
    /// <param name="opponentMaxHp">The opponent's maximum health.</param>
    /// <param name="opponentCurrentSd">The opponent's current shield.</param>
    /// <param name="opponentMaxSd">The opponent's maximum shield.</param>
    /// <param name="expectedPlayer1Hp">Expected player 1 health percentage.</param>
    /// <param name="expectedPlayer2Hp">Expected player 2 health percentage.</param>
    /// <param name="expectedPlayer1Sd">Expected player 1 shield percentage.</param>
    /// <param name="expectedPlayer2Sd">Expected player 2 shield percentage.</param>
    [TestCase(500, 1000, 100, 400, 1000, 2000, 400, 800, 50, 50, 25, 50)]
    [TestCase(80, 100, 30, 50, 250, 500, 100, 1000, 80, 50, 60, 10)]
    [TestCase(123, 123, 123, 123, 1, 3, 2, 3, 100, 33, 100, 66)]
    public async Task SpectatorReceivesDistinctPercentagesWhenMaximumsDifferAsync(
        float requesterCurrentHp,
        float requesterMaxHp,
        float requesterCurrentSd,
        float requesterMaxSd,
        float opponentCurrentHp,
        float opponentMaxHp,
        float opponentCurrentSd,
        float opponentMaxSd,
        byte expectedPlayer1Hp,
        byte expectedPlayer2Hp,
        byte expectedPlayer1Sd,
        byte expectedPlayer2Sd)
    {
        var context = CreateGameServerContext();
        var (spectator, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(context);
        var (requester, _) = await CreatePlayerWithStatsAsync(context, RequesterId, requesterCurrentHp, requesterMaxHp, requesterCurrentSd, requesterMaxSd).ConfigureAwait(false);
        var (opponent, _) = await CreatePlayerWithStatsAsync(context, OpponentId, opponentCurrentHp, opponentMaxHp, opponentCurrentSd, opponentMaxSd).ConfigureAwait(false);
        await using var duelRoom = CreateDuelRoom(requester, opponent);

        await new DuelHealthUpdatePlugIn(spectator).UpdateHealthAsync(duelRoom).ConfigureAwait(false);

        DuelHealthUpdate packet = output.ToArray().AsMemory();
        Assert.Multiple(() =>
        {
            Assert.That(packet.Player1Id, Is.EqualTo(RequesterId));
            Assert.That(packet.Player2Id, Is.EqualTo(OpponentId));
            Assert.That(packet.Player1HealthPercentage, Is.EqualTo(expectedPlayer1Hp));
            Assert.That(packet.Player2HealthPercentage, Is.EqualTo(expectedPlayer2Hp));
            Assert.That(packet.Player1ShieldPercentage, Is.EqualTo(expectedPlayer1Sd));
            Assert.That(packet.Player2ShieldPercentage, Is.EqualTo(expectedPlayer2Sd));
        });
    }

    /// <summary>
    /// Verifies that when invoked from either duelist's perspective, player 1 is the viewing duelist
    /// (with <see cref="ViewExtensions.ConstantPlayerId"/>) and both percentages use each duelist's own maximums.
    /// </summary>
    /// <param name="viewerIsOpponent">If set to <c>true</c>, invokes the view for the opponent; otherwise for the requester.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task DuelistPerspectiveUsesOwnMaximumsForBothPlayersAsync(bool viewerIsOpponent)
    {
        var context = CreateGameServerContext();
        var (requester, requesterOutput) = await CreatePlayerWithStatsAsync(context, RequesterId, 600, 1200, 150, 500).ConfigureAwait(false);
        var (opponent, opponentOutput) = await CreatePlayerWithStatsAsync(context, OpponentId, 150, 200, 180, 300).ConfigureAwait(false);
        await using var duelRoom = CreateDuelRoom(requester, opponent);

        var viewer = viewerIsOpponent ? opponent : requester;
        var output = viewerIsOpponent ? opponentOutput : requesterOutput;
        await new DuelHealthUpdatePlugIn(viewer).UpdateHealthAsync(duelRoom).ConfigureAwait(false);

        DuelHealthUpdate packet = output.ToArray().AsMemory();
        Assert.Multiple(() =>
        {
            Assert.That(packet.Player1Id, Is.EqualTo(ViewExtensions.ConstantPlayerId));
            Assert.That(packet.Player2Id, Is.EqualTo(viewerIsOpponent ? RequesterId : OpponentId));
            Assert.That(packet.Player1HealthPercentage, Is.EqualTo(viewerIsOpponent ? 75 : 50));
            Assert.That(packet.Player2HealthPercentage, Is.EqualTo(viewerIsOpponent ? 50 : 75));
            Assert.That(packet.Player1ShieldPercentage, Is.EqualTo(viewerIsOpponent ? 60 : 30));
            Assert.That(packet.Player2ShieldPercentage, Is.EqualTo(viewerIsOpponent ? 30 : 60));
        });
    }

    /// <summary>
    /// Verifies that zero maximum health or shield (for example classic PvP without shield) produces 0%
    /// without division-by-zero artifacts while leaving the other player's percentages unaffected.
    /// </summary>
    /// <param name="opponentCurrentHp">The opponent's current health.</param>
    /// <param name="opponentMaxHp">The opponent's maximum health.</param>
    /// <param name="opponentCurrentSd">The opponent's current shield.</param>
    /// <param name="opponentMaxSd">The opponent's maximum shield.</param>
    /// <param name="expectedOpponentHp">Expected opponent health percentage.</param>
    /// <param name="expectedOpponentSd">Expected opponent shield percentage.</param>
    [TestCase(0, 100, 0, 0, 0, 0)]
    [TestCase(75, 100, 50, 0, 75, 0)]
    [TestCase(100, 0, 0, 0, 0, 0)]
    [TestCase(0, 0, 40, 200, 0, 20)]
    public async Task ZeroMaximumHealthOrShieldProducesZeroPercentageAsync(
        float opponentCurrentHp,
        float opponentMaxHp,
        float opponentCurrentSd,
        float opponentMaxSd,
        byte expectedOpponentHp,
        byte expectedOpponentSd)
    {
        var context = CreateGameServerContext();
        var (spectator, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(context);
        var (requester, _) = await CreatePlayerWithStatsAsync(context, RequesterId, 90, 100, 0, 0).ConfigureAwait(false);
        var (opponent, _) = await CreatePlayerWithStatsAsync(context, OpponentId, opponentCurrentHp, opponentMaxHp, opponentCurrentSd, opponentMaxSd).ConfigureAwait(false);
        await using var duelRoom = CreateDuelRoom(requester, opponent);

        await new DuelHealthUpdatePlugIn(spectator).UpdateHealthAsync(duelRoom).ConfigureAwait(false);

        DuelHealthUpdate packet = output.ToArray().AsMemory();
        Assert.Multiple(() =>
        {
            Assert.That(packet.Player1HealthPercentage, Is.EqualTo(90));
            Assert.That(packet.Player1ShieldPercentage, Is.Zero);
            Assert.That(packet.Player2HealthPercentage, Is.EqualTo(expectedOpponentHp));
            Assert.That(packet.Player2ShieldPercentage, Is.EqualTo(expectedOpponentSd));
        });
    }

    /// <summary>
    /// Verifies that negative or above-maximum health and shield values are clamped to [0, 100]
    /// instead of wrapping around a byte.
    /// </summary>
    [Test]
    public async Task OutOfRangeHealthAndShieldValuesAreClampedToBytePercentageRangeAsync()
    {
        var context = CreateGameServerContext();
        var (spectator, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(context);
        var (requester, _) = await CreatePlayerWithStatsAsync(context, RequesterId, -25, 1000, -5, 500).ConfigureAwait(false);
        var (opponent, _) = await CreatePlayerWithStatsAsync(context, OpponentId, 300, 100, 260, 100).ConfigureAwait(false);
        await using var duelRoom = CreateDuelRoom(requester, opponent);

        await new DuelHealthUpdatePlugIn(spectator).UpdateHealthAsync(duelRoom).ConfigureAwait(false);

        DuelHealthUpdate packet = output.ToArray().AsMemory();
        Assert.Multiple(() =>
        {
            Assert.That(packet.Player1HealthPercentage, Is.Zero);
            Assert.That(packet.Player1ShieldPercentage, Is.Zero);
            Assert.That(packet.Player2HealthPercentage, Is.EqualTo(100));
            Assert.That(packet.Player2ShieldPercentage, Is.EqualTo(100));
        });
    }

    /// <summary>
    /// Verifies that no packet is written and no exception is thrown when the viewer's connection is null.
    /// </summary>
    [Test]
    public async Task DisconnectedViewerSendsNoPacketAsync()
    {
        var context = CreateGameServerContext();
        var (spectator, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(context);
        await spectator.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await spectator.DisconnectAsync().ConfigureAwait(false);
        Assert.That(spectator.Connection, Is.Null);

        var (requester, _) = await CreatePlayerWithStatsAsync(context, RequesterId, 100, 100, 100, 100).ConfigureAwait(false);
        var (opponent, _) = await CreatePlayerWithStatsAsync(context, OpponentId, 100, 100, 100, 100).ConfigureAwait(false);
        await using var duelRoom = CreateDuelRoom(requester, opponent);

        await new DuelHealthUpdatePlugIn(spectator).UpdateHealthAsync(duelRoom).ConfigureAwait(false);

        Assert.That(output.Length, Is.Zero);
    }

    /// <summary>
    /// Verifies that no packet is sent and no exception is thrown when either fighter has null attributes.
    /// </summary>
    /// <param name="requesterHasNullAttributes">Whether the requester lacks initialized attributes.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async Task FighterWithNullAttributesSendsNoPacketAsync(bool requesterHasNullAttributes)
    {
        var context = CreateGameServerContext();
        var (spectator, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(context);
        var requester = requesterHasNullAttributes
            ? CastleSiegeRemoteViewTestHelper.CreatePlayer(context).Player
            : (await CreatePlayerWithStatsAsync(context, RequesterId, 100, 100, 100, 100).ConfigureAwait(false)).Player;
        var opponent = requesterHasNullAttributes
            ? (await CreatePlayerWithStatsAsync(context, OpponentId, 100, 100, 100, 100).ConfigureAwait(false)).Player
            : CastleSiegeRemoteViewTestHelper.CreatePlayer(context).Player;
        await using var duelRoom = CreateDuelRoom(requester, opponent);

        await new DuelHealthUpdatePlugIn(spectator).UpdateHealthAsync(duelRoom).ConfigureAwait(false);

        Assert.That(output.Length, Is.Zero);
    }

    private static IGameServerContext CreateGameServerContext()
    {
        var configuration = PlayerTestHelper.CreateGameConfiguration();
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        var mapInitializer = new MapInitializer(configuration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var context = new GameServerContext(
            new BasicModel.GameServerDefinition
            {
                GameConfiguration = configuration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            new InMemoryPersistenceContextProvider(),
            mapInitializer,
            NullLoggerFactory.Instance,
            plugInManager,
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = context.PlugInManager;
        mapInitializer.PathFinderPool = context.PathFinderPool;
        return context;
    }

    private static async ValueTask<(RemotePlayer Player, MemoryStream Output)> CreatePlayerWithStatsAsync(
        IGameServerContext gameContext,
        ushort id,
        float currentHealth,
        float maximumHealth,
        float currentShield,
        float maximumShield)
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer(gameContext);
        var character = CreateCharacterWithEditableStats(gameContext);
        await PlayerTestHelper.EnterWorldAsync(player, character).ConfigureAwait(false);
        player.Id = id;

        // Set maximums before current values so Player.OnAttributeValueChanged does not clamp out-of-range test inputs.
        player.Attributes![Stats.MaximumHealth] = maximumHealth;
        player.Attributes[Stats.CurrentHealth] = currentHealth;
        player.Attributes[Stats.MaximumShield] = maximumShield;
        player.Attributes[Stats.CurrentShield] = currentShield;
        return (player, output);
    }

    private static Character CreateCharacterWithEditableStats(IGameContext gameContext)
    {
        var characterMock = new Mock<Character>();
        characterMock.SetupAllProperties();
        characterMock.Setup(c => c.LearnedSkills).Returns(new List<SkillEntry>());
        characterMock.Setup(c => c.Attributes).Returns(new List<StatAttribute>());
        characterMock.Setup(c => c.DropItemGroups).Returns(new List<DropItemGroup>());

        var inventoryMock = new Mock<ItemStorage>();
        inventoryMock.SetupAllProperties();
        inventoryMock.Setup(i => i.Items).Returns(new List<Item>());

        var characterClassMock = new Mock<CharacterClass>();
        characterClassMock.Setup(c => c.StatAttributes).Returns(
            new List<StatAttributeDefinition>
            {
                new(Stats.Level, 1, false),
                new(Stats.CurrentHealth, 0, false),
                new(Stats.MaximumHealth, 0, false),
                new(Stats.CurrentShield, 0, false),
                new(Stats.MaximumShield, 0, false),
            });
        characterClassMock.Setup(c => c.AttributeCombinations).Returns(new List<AttributeRelationship>());
        characterClassMock.Setup(c => c.BaseAttributeValues).Returns(new List<ConstValueAttribute>());

        var character = characterMock.Object;
        character.Inventory = inventoryMock.Object;
        character.CurrentMap = gameContext.Configuration.Maps.FirstOrDefault(m => m.Number == 0);
        character.CharacterClass = characterClassMock.Object;
        foreach (var attributeDef in character.CharacterClass.StatAttributes)
        {
            character.Attributes.Add(new StatAttribute(attributeDef.Attribute!, attributeDef.BaseValue));
        }

        return character;
    }

    private static DuelRoom CreateDuelRoom(Player requester, Player opponent)
    {
        return new DuelRoom(new BasicModel.DuelArea { Index = 0 }, requester, opponent);
    }
}
