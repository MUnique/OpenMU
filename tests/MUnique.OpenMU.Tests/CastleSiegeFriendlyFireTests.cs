// <copyright file="CastleSiegeFriendlyFireTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that players of the same side of a Castle Siege battle can't damage each other.
/// </summary>
[TestFixture]
public class CastleSiegeFriendlyFireTests
{
    private static readonly Guid CastleSiegePlugInId = new("B7F62FA9-59E6-49E9-B499-0358A14957CF");

    /// <summary>
    /// Verifies that only players of the same side on the siege map during the battle are on the same side.
    /// </summary>
    [Test]
    public async ValueTask OnlyTheSameSideDuringTheBattleIsFriendlyAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        var attacker = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var ally = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var otherAttacker = await fixture.CreatePlayerAsync(3, CastleSiegeJoinSide.Attack2).ConfigureAwait(false);
        var spectator = await fixture.CreatePlayerAsync(4, CastleSiegeJoinSide.None).ConfigureAwait(false);
        var otherSpectator = await fixture.CreatePlayerAsync(5, CastleSiegeJoinSide.None).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(fixture.Context.AreOnSameSide(attacker, ally), Is.True);
            Assert.That(fixture.Context.AreOnSameSide(attacker, otherAttacker), Is.False, "Different attacking sides.");
            Assert.That(fixture.Context.AreOnSameSide(attacker, spectator), Is.False, "The spectator has no side.");
            Assert.That(fixture.Context.AreOnSameSide(spectator, otherSpectator), Is.False, "Players without side aren't a side.");
        });

        ally.SetCurrentMapSilently(fixture.OtherMap);
        Assert.That(fixture.Context.AreOnSameSide(attacker, ally), Is.False, "The ally left the siege map.");

        ally.SetCurrentMapSilently(fixture.Map);
        fixture.Context.CurrentState = CastleSiegeState.End;
        Assert.That(fixture.Context.AreOnSameSide(attacker, ally), Is.False, "Outside of the battle.");
    }

    /// <summary>
    /// Verifies that an attack on a player of the same side doesn't hit.
    /// </summary>
    [Test]
    public async ValueTask AttackOnTheSameSideIsPreventedAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        var attacker = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var ally = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var healthBefore = ally.Attributes![Stats.CurrentHealth];

        var hitInfo = await ally.AttackByAsync(attacker, null, false).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(hitInfo, Is.Null);
            Assert.That(ally.Attributes[Stats.CurrentHealth], Is.EqualTo(healthBefore));
        });
    }

    /// <summary>
    /// Verifies that an attack on a player of an opposing side still hits.
    /// </summary>
    [Test]
    public async ValueTask AttackOnAnOpposingSideIsNotPreventedAsync()
    {
        var fixture = await SiegeFixture.CreateAsync().ConfigureAwait(false);
        var attacker = await fixture.CreatePlayerAsync(1, CastleSiegeJoinSide.Attack1).ConfigureAwait(false);
        var defender = await fixture.CreatePlayerAsync(2, CastleSiegeJoinSide.Defense).ConfigureAwait(false);

        var hitInfo = await defender.AttackByAsync(attacker, null, false).ConfigureAwait(false);

        Assert.That(hitInfo, Is.Not.Null);
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
