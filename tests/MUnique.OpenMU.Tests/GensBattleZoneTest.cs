// <copyright file="GensBattleZoneTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Gens;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Tests for the <see cref="GensBattleZonePlugIn"/> and the <see cref="GensRankingPlugIn"/>.
/// </summary>
[TestFixture]
public class GensBattleZoneTest
{
    /// <summary>
    /// The number of the map of the test players.
    /// </summary>
    private const short MapNumber = 0;

    private readonly GensBattleZonePlugIn _plugIn = new();

    /// <summary>
    /// Tests that the attacks and kills between members of different gens in the battle zone are exempted from the player killer penalty.
    /// </summary>
    [Test]
    public async Task ExemptionBetweenEnemiesInBattleZoneAsync()
    {
        var (killer, victim) = await CreateEnemiesAsync().ConfigureAwait(false);

        Assert.That(killer.IsExemptedFromPlayerKillPenalty(victim), Is.True);

        victim.GensMember!.Gens = GensType.Duprian;
        Assert.That(killer.IsExemptedFromPlayerKillPenalty(victim), Is.False, "Members of the same gens.");

        victim.GensMember.Gens = GensType.None;
        Assert.That(killer.IsExemptedFromPlayerKillPenalty(victim), Is.False, "The victim isn't a member.");

        victim.GensMember.Gens = GensType.Vanert;
        GetConfiguration(killer).BattleZoneMapNumbers.Clear();
        Assert.That(killer.IsExemptedFromPlayerKillPenalty(victim), Is.False, "Outside of the battle zone.");
    }

    /// <summary>
    /// Tests that a kill between members of different gens in the battle zone doesn't make the killer an outlaw.
    /// </summary>
    [Test]
    public async Task KillDoesNotMakeAnOutlawAsync()
    {
        var (killer, victim) = await CreateEnemiesAsync().ConfigureAwait(false);
        var stateBefore = killer.SelectedCharacter!.State;

        await killer.AfterKilledPlayerAsync(victim).ConfigureAwait(false);

        Assert.That(killer.SelectedCharacter.State, Is.EqualTo(stateBefore));
        Assert.That(killer.SelectedCharacter.PlayerKillCount, Is.Zero);
    }

    /// <summary>
    /// Tests that only gens members can enter the battle zone.
    /// </summary>
    [Test]
    public async Task OnlyMembersCanEnterTheBattleZoneAsync()
    {
        var (member, nonMember) = await CreateEnemiesAsync().ConfigureAwait(false);
        nonMember.GensMember!.Gens = GensType.None;
        var gate = new ExitGate { Map = member.CurrentMap!.Definition };

        var memberArgs = new CancelEventArgs();
        await this._plugIn.WarpGateEnteringAsync(member, gate, memberArgs).ConfigureAwait(false);
        var nonMemberArgs = new CancelEventArgs();
        await this._plugIn.WarpGateEnteringAsync(nonMember, gate, nonMemberArgs).ConfigureAwait(false);

        Assert.That(memberArgs.Cancel, Is.False);
        Assert.That(nonMemberArgs.Cancel, Is.True);
    }

    /// <summary>
    /// Tests that the warp command checks the warp gate plugins, too, before the costs are paid.
    /// </summary>
    [Test]
    public async Task WarpCommandToTheBattleZoneIsDeniedForNonMembersAsync()
    {
        var (member, nonMember) = await CreateEnemiesAsync().ConfigureAwait(false);
        nonMember.GensMember!.Gens = GensType.None;
        nonMember.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IWarpGateEnteringPlugIn>(this._plugIn);
        nonMember.Money = 1000;
        var warpInfo = new WarpInfo { Costs = 100, Gate = new ExitGate { Map = member.CurrentMap!.Definition } };

        await new WarpAction().WarpToAsync(nonMember, warpInfo).ConfigureAwait(false);

        Assert.That(nonMember.Money, Is.EqualTo(1000));
        Assert.That(nonMember.PlayerState.CurrentState, Is.Not.EqualTo(PlayerState.ChangingMap));
    }

    /// <summary>
    /// Tests that a kill changes the contribution points and the rank of the killer and the victim.
    /// </summary>
    [Test]
    public async Task KillChangesTheContributionAsync()
    {
        var (killer, victim) = await CreateEnemiesAsync().ConfigureAwait(false);
        killer.GensMember!.Contribution = 498;
        victim.GensMember!.Contribution = 2;

        await this._plugIn.AttackableGotKilledAsync(victim, killer).ConfigureAwait(false);

        Assert.That(killer.GensMember.Contribution, Is.EqualTo(503));
        Assert.That(killer.GensMember.Rank, Is.EqualTo(13));
        Assert.That(victim.GensMember.Contribution, Is.Zero, "The contribution doesn't get negative.");

        await this._plugIn.AttackableGotKilledAsync(victim, killer).ConfigureAwait(false);
        Assert.That(killer.GensMember.Contribution, Is.EqualTo(503), "A victim without contribution points gives no points.");
    }

    /// <summary>
    /// Tests that repeated kills of the same victim stop to change the contribution points at the limit,
    /// and that the count starts again after the reset time.
    /// </summary>
    [Test]
    public async Task RepeatedKillsAreLimitedAsync()
    {
        var (killer, victim) = await CreateEnemiesAsync().ConfigureAwait(false);
        var configuration = GetConfiguration(killer);
        victim.GensMember!.Contribution = 400;

        for (var i = 1; i < configuration.AbuseLimitKillCount; i++)
        {
            await this._plugIn.AttackableGotKilledAsync(victim, killer).ConfigureAwait(false);
        }

        var contributionBeforeLimit = killer.GensMember!.Contribution;
        Assert.That(contributionBeforeLimit, Is.EqualTo(10 + ((configuration.AbuseLimitKillCount - 1) * 5)));

        await this._plugIn.AttackableGotKilledAsync(victim, killer).ConfigureAwait(false);
        Assert.That(killer.GensMember.Contribution, Is.EqualTo(contributionBeforeLimit));

        var abuse = await killer.PersistenceContext.GetGensAbuseAsync(killer.SelectedCharacter!.Id, victim.SelectedCharacter!.Id).ConfigureAwait(false);
        abuse!.LastKillAt = DateTime.UtcNow - configuration.AbuseResetTime - TimeSpan.FromMinutes(1);
        await this._plugIn.AttackableGotKilledAsync(victim, killer).ConfigureAwait(false);
        Assert.That(killer.GensMember.Contribution, Is.EqualTo(contributionBeforeLimit + 5));
        Assert.That(abuse.KillCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the ranking orders the members of each gens by their contribution points,
    /// and updates the ranking position and rank of the online members.
    /// </summary>
    [Test]
    public async Task RankingAsync()
    {
        var (duprian, vanert) = await CreateEnemiesAsync().ConfigureAwait(false);
        var secondDuprian = await CreateMemberAsync((GameContext)duprian.GameContext, GensType.Duprian).ConfigureAwait(false);
        duprian.GensMember!.Contribution = 10000;
        secondDuprian.GensMember!.Contribution = 20000;
        vanert.GensMember!.Contribution = 600;
        await duprian.SaveProgressAsync().ConfigureAwait(false);
        await secondDuprian.SaveProgressAsync().ConfigureAwait(false);
        await vanert.SaveProgressAsync().ConfigureAwait(false);

        await new GensRankingPlugIn().ExecuteTaskAsync((GameContext)duprian.GameContext).ConfigureAwait(false);

        Assert.That(secondDuprian.GensMember.RankingPosition, Is.EqualTo(1));
        Assert.That(secondDuprian.GensMember.Rank, Is.EqualTo(1));
        Assert.That(duprian.GensMember.RankingPosition, Is.EqualTo(2));
        Assert.That(duprian.GensMember.Rank, Is.EqualTo(2));
        Assert.That(vanert.GensMember.RankingPosition, Is.EqualTo(1), "Each gens has its own ranking.");
        Assert.That(vanert.GensMember.Rank, Is.EqualTo(13), "Below 10000 points, the rank depends on the points only.");
    }

    private static GensConfiguration GetConfiguration(Player player)
    {
        return GensFeaturePlugIn.GetConfiguration(player.GameContext)!;
    }

    private static async ValueTask<(Player Killer, Player Victim)> CreateEnemiesAsync()
    {
        var first = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = (GameContext)first.GameContext;
        var configuration = new GensConfiguration { BattleZoneMapNumbers = new List<short> { MapNumber } };
        gameContext.FeaturePlugIns.AddPlugIn(new GensFeaturePlugIn { Configuration = configuration }, true);
        gameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerKillPenaltyExemptionPlugIn>(new GensBattleZonePlugIn());
        await MakeMemberAsync(first, GensType.Duprian).ConfigureAwait(false);

        var second = await CreateMemberAsync(gameContext, GensType.Vanert).ConfigureAwait(false);
        return (first, second);
    }

    private static async ValueTask<Player> CreateMemberAsync(GameContext gameContext, GensType gens)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await MakeMemberAsync(player, gens).ConfigureAwait(false);
        return player;
    }

    private static async ValueTask MakeMemberAsync(Player player, GensType gens)
    {
        player.SelectedCharacter!.Id = Guid.NewGuid();
        player.Attributes![Stats.Level] = 100;
        player.CurrentMap = await player.GameContext.GetMapAsync((ushort)MapNumber).ConfigureAwait(false);
        var member = player.PersistenceContext.CreateNew<GensMember>();
        member.CharacterId = player.SelectedCharacter.Id;
        member.Gens = gens;
        member.Contribution = 10;
        member.Rank = 14;
        member.JoinedAt = DateTime.UtcNow;
        player.GensMember = member;
    }
}
