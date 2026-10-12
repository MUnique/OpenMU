// <copyright file="GensBattleZonePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The rules of the gens battle zone:
/// Only gens members can enter it, and the kills between the members of different gens in it
/// change their contribution points, without making the killer an outlaw.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensBattleZonePlugIn_Name), Description = nameof(PlugInResources.GensBattleZonePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A7C2E915-3B48-4F6D-8E21-6D9B0F4C5A83")]
public class GensBattleZonePlugIn : IPlayerKillPenaltyExemptionPlugIn, IWarpGateEnteringPlugIn, IAttackableGotKilledPlugIn
{
    /// <inheritdoc />
    public void CheckExemption(Player attacker, Player defender, PlayerKillPenaltyExemptionArgs eventArgs)
    {
        if (GensFeaturePlugIn.GetConfiguration(attacker.GameContext) is { } configuration
            && AreEnemiesInBattleZone(attacker, defender, configuration))
        {
            eventArgs.IsExempted = true;
        }
    }

    /// <inheritdoc />
    public async ValueTask WarpGateEnteringAsync(Player player, ExitGate targetGate, CancelEventArgs eventArgs)
    {
        if (GensFeaturePlugIn.GetConfiguration(player.GameContext) is not { } configuration
            || targetGate.Map is not { } map
            || !configuration.IsBattleZone(map.Number)
            || (await player.GetGensMemberAsync().ConfigureAwait(false))?.Gens is GensType.Duprian or GensType.Vanert)
        {
            return;
        }

        eventArgs.Cancel = true;
        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GensBattleZoneMembersOnly)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask AttackableGotKilledAsync(IAttackable attackable, IAttacker? killer)
    {
        if (attackable is not Player victim
            || (killer as Player ?? (killer as Monster)?.SummonedBy) is not { } killerPlayer
            || killerPlayer == victim
            || (killerPlayer.DuelRoom is { } duelRoom && duelRoom.AreDuelists(killerPlayer, victim))
            || GensFeaturePlugIn.GetConfiguration(victim.GameContext) is not { } configuration
            || !AreEnemiesInBattleZone(killerPlayer, victim, configuration)
            || killerPlayer.SelectedCharacter is not { } killerCharacter
            || victim.SelectedCharacter is not { } victimCharacter)
        {
            return;
        }

        var killCount = await CountKillAsync(killerPlayer, killerCharacter, victimCharacter, configuration).ConfigureAwait(false);
        if (killCount >= configuration.AbuseLimitKillCount)
        {
            await killerPlayer.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GensKillAbuseLimitFormat), victimCharacter.Name).ConfigureAwait(false);
            return;
        }

        if (killCount >= configuration.AbuseWarningKillCount)
        {
            await killerPlayer.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GensKillAbuseWarningFormat), victimCharacter.Name, killCount, configuration.AbuseLimitKillCount).ConfigureAwait(false);
        }

        var killerMember = killerPlayer.GensMember!;
        var victimMember = victim.GensMember!;
        var (gain, loss) = configuration.GetKillContribution(killerPlayer.Level, victim.Level, killerMember.Rank, victimMember.Rank);
        if (victimMember.Contribution < configuration.MinimumVictimContribution)
        {
            gain = 0;
        }

        var gained = await ChangeContributionAsync(killerPlayer, gain, configuration).ConfigureAwait(false);
        var lost = await ChangeContributionAsync(victim, -loss, configuration).ConfigureAwait(false);
        if (gained != 0)
        {
            await killerPlayer.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GensContributionGainedFormat), gained).ConfigureAwait(false);
        }

        if (lost != 0)
        {
            await victim.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GensContributionLostFormat), -lost).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Determines whether the players are members of different gens, and both are in the same map of the battle zone.
    /// </summary>
    private static bool AreEnemiesInBattleZone(Player attacker, Player defender, GensConfiguration configuration)
    {
        var attackerGens = attacker.GetGens();
        var defenderGens = defender.GetGens();
        return attackerGens != GensType.None
               && defenderGens != GensType.None
               && attackerGens != defenderGens
               && attacker.CurrentMap is { } map
               && map == defender.CurrentMap
               && configuration.IsBattleZone(map.Definition.Number);
    }

    /// <summary>
    /// Counts the kill of the victim by the killer, and returns the count of the recent kills, including this one.
    /// The count starts again, when the last kill is longer ago than the <see cref="GensConfiguration.AbuseResetTime"/>.
    /// </summary>
    private static async ValueTask<int> CountKillAsync(Player killer, Character killerCharacter, Character victimCharacter, GensConfiguration configuration)
    {
        return await killer.RunPersistenceExclusiveAsync(async () =>
        {
            var now = DateTime.UtcNow;
            var abuse = await killer.PersistenceContext.GetGensAbuseAsync(killerCharacter.Id, victimCharacter.Id).ConfigureAwait(false);
            if (abuse is null)
            {
                abuse = killer.PersistenceContext.CreateNew<GensAbuse>();
                abuse.KillerId = killerCharacter.Id;
                abuse.VictimId = victimCharacter.Id;
            }
            else if (abuse.LastKillAt + configuration.AbuseResetTime < now)
            {
                abuse.KillCount = 0;
            }

            abuse.KillCount++;
            abuse.LastKillAt = now;
            return abuse.KillCount;
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Changes the contribution points of the player, which never get negative, updates its rank,
    /// saves it and shows it. The rank which depends on the ranking position is updated with the next ranking.
    /// </summary>
    /// <returns>The actual change of the contribution points.</returns>
    private static async ValueTask<int> ChangeContributionAsync(Player player, int change, GensConfiguration configuration)
    {
        var actualChange = await player.RunPersistenceExclusiveAsync(async () =>
        {
            if (player.GensMember is not { Gens: not GensType.None } member)
            {
                return 0;
            }

            var previousContribution = member.Contribution;
            member.Contribution = Math.Max(member.Contribution + change, 0);
            member.Rank = configuration.GetRank(member.Contribution, member.RankingPosition);
            await player.SaveProgressAsync().ConfigureAwait(false);
            return member.Contribution - previousContribution;
        }).ConfigureAwait(false);

        await player.ShowChangedGensAsync().ConfigureAwait(false);
        return actualChange;
    }
}
