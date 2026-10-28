// <copyright file="CastleSiegePlayerKillTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that the fights between the sides of a Castle Siege battle are exempted from the player killer penalty.
/// </summary>
[TestFixture]
public class CastleSiegePlayerKillTests
{
    private static readonly Guid CastleSiegePlugInId = new("B7F62FA9-59E6-49E9-B499-0358A14957CF");

    /// <summary>
    /// Verifies that only players of different sides on the siege map during the battle are enemies.
    /// </summary>
    [Test]
    public async ValueTask OnlyDifferentSidesDuringTheBattleAreOpposingAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        var attacker = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var otherAttacker = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Attack2).ConfigureAwait(false);
        var ally = await fixture.CreatePlayerAsync(3, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var defender = await fixture.CreatePlayerAsync(4, CastleSiegeJoinSide.Defense).ConfigureAwait(false);
        var spectator = await fixture.CreatePlayerAsync(5, CastleSiegeJoinSide.None).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.AreOnOpposingSides(attacker, defender), Is.True);
            Assert.That(fixture.Context.AreOnOpposingSides(defender, attacker), Is.True);
            Assert.That(fixture.Context.AreOnOpposingSides(attacker, otherAttacker), Is.True, "Attacking sides fight each other.");
            Assert.That(fixture.Context.AreOnOpposingSides(attacker, ally), Is.False, "Same side.");
            Assert.That(fixture.Context.AreOnOpposingSides(attacker, spectator), Is.False, "The spectator has no side.");
        });

        fixture.Context.CurrentState = CastleSiegeState.Ready;
        Assert.That(fixture.Context.AreOnOpposingSides(attacker, defender), Is.False, "Outside of the battle.");
    }

    /// <summary>
    /// Verifies that a player who left the siege map isn't an enemy anymore.
    /// </summary>
    [Test]
    public async ValueTask PlayerOutsideTheSiegeMapIsNotOpposingAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        var attacker = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var defender = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Defense).ConfigureAwait(false);
        defender.SetCurrentMapSilently(fixture.OtherMap);

        Assert.That(fixture.Context.AreOnOpposingSides(attacker, defender), Is.False);
    }

    /// <summary>
    /// Verifies that the plugin exempts the attacks between opposing sides, but not between the same side.
    /// </summary>
    [Test]
    public async ValueTask OpposingSidesAreExemptedAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        var attacker = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var ally = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var defender = await fixture.CreatePlayerAsync(3, CastleSiegeJoinSide.Defense).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(attacker.IsExemptedFromPlayerKillPenalty(defender), Is.True);
            Assert.That(attacker.IsExemptedFromPlayerKillPenalty(ally), Is.False);
        });
    }

    /// <summary>
    /// Verifies that killing a player of an opposing side during the battle doesn't make the killer an outlaw.
    /// </summary>
    [Test]
    public async ValueTask KillOfAnOpposingPlayerDoesNotMakeAnOutlawAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        var attacker = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var defender = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Defense).ConfigureAwait(false);
        var stateBefore = attacker.SelectedCharacter!.State;

        await attacker.AfterKilledPlayerAsync(defender).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(attacker.SelectedCharacter.State, Is.EqualTo(stateBefore));
            Assert.That(attacker.SelectedCharacter.PlayerKillCount, Is.Zero);
        });
    }

    /// <summary>
    /// Verifies that killing a player of the same side still counts as a player kill.
    /// </summary>
    [Test]
    public async ValueTask KillOfAnAllyStillMakesAnOutlawAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        var attacker = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var ally = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);

        await attacker.AfterKilledPlayerAsync(ally).ConfigureAwait(false);

        Assert.That(attacker.SelectedCharacter!.PlayerKillCount, Is.EqualTo(1));
    }

    private sealed class SiegeFixture
    {
        public SiegeFixture(IGameContext gameContext, CastleSiegeContext context, GameMap map, GameMap otherMap)
        {
            this.GameContext = gameContext;
            this.Context = context;
            this.Map = map;
            this.OtherMap = otherMap;
        }

        public IGameContext GameContext { get; }

        public CastleSiegeContext Context { get; }

        public GameMap Map { get; }

        public GameMap OtherMap { get; }

        public static async ValueTask<SiegeFixture> CreateAsync()
        {
            var gameContext = GameContextTestHelper.CreateGameContext(
            [
                new PlugInConfiguration { TypeId = CastleSiegePlugInId, IsActive = true },
            ]);
            var siegeMap = new BasicModel.GameMapDefinition { Number = 30, TerrainData = new byte[ushort.MaxValue + 3] };
            gameContext.Configuration.Maps.Add(siegeMap);
            var configuration = new BasicModel.CastleSiegeConfiguration
            {
                Enabled = true,
                CastleSiegeMapDefinition = siegeMap,
            };
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.Ready, DayOfWeek = DayOfWeek.Monday });
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry { State = CastleSiegeState.Start, DayOfWeek = DayOfWeek.Tuesday });
            gameContext.Configuration.CastleSiegeConfiguration = configuration;

            var plugIn = gameContext.PlugInManager.GetActivePlugInsOf<IPeriodicTaskPlugIn>().OfType<CastleSiegePlugIn>().Single();
            await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);
            var context = plugIn.GetContext(gameContext) ?? throw new InvalidOperationException("The context wasn't created.");
            context.CurrentState = CastleSiegeState.Start;

            var map = new GameMap(siegeMap, TimeSpan.FromMinutes(1), 8);
            var otherMap = new GameMap(gameContext.Configuration.Maps.First(), TimeSpan.FromMinutes(1), 8);
            return new SiegeFixture(gameContext, context, map, otherMap);
        }

        public async ValueTask<Player> CreatePlayerAsync(uint guildId, CastleSiegeJoinSide side)
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(this.GameContext).ConfigureAwait(false);
            player.SelectedCharacter!.Id = Guid.NewGuid();
            player.GuildStatus = new GuildMemberStatus(guildId, GuildPosition.NormalMember);
            player.Position = new Point((byte)guildId, 20);
            player.SetCurrentMapSilently(this.Map);
            this.Context.PlayerJoinSides[player.SelectedCharacter.Id] = side;
            return player;
        }
    }
}
