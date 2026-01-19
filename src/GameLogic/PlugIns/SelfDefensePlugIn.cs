// <copyright file="SelfDefensePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates the state of the active self defenses on every second and every hit.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.SelfDefensePlugIn_Name), Description = nameof(PlugInResources.SelfDefensePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("BA4753EA-4D2B-488C-BB6B-4A127E28630A")]
public class SelfDefensePlugIn : IPeriodicTaskPlugIn, IAttackableGotHitPlugIn, ISupportCustomConfiguration<SelfDefensePlugInConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public SelfDefensePlugInConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var timedOut = gameContext.SelfDefenseState.Where(s => s.Value < DateTime.UtcNow).ToList();
        foreach (var (pair, _) in timedOut)
        {
            if (gameContext.SelfDefenseState.Remove(pair, out _))
            {
                await this.EndSelfDefenseAsync(pair.Attacker, pair.Defender).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        // do nothing.
    }

    /// <inheritdoc />
    public void AttackableGotHit(IAttackable attackable, IAttacker attacker, HitInfo hitInfo)
    {
        var defender = attackable as Player ?? (attackable as Monster)?.SummonedBy;
        var attackerPlayer = attacker as Player ?? (attackable as Monster)?.SummonedBy;
        if (defender is null || attackerPlayer is null)
        {
            return;
        }

        if (defender.SelectedCharacter?.State >= HeroState.PlayerKiller1stStage)
        {
            // PKs have no right to self-defense.
            return;
        }

        if (attackerPlayer.DuelRoom is not null || defender.DuelRoom is not null)
        {
            // No self-defense during a duel.
            return;
        }

        if (attackerPlayer.CurrentMiniGame?.AllowPlayerKilling is true)
        {
            // e.g. during chaos castle
            return;
        }

        if (attackerPlayer.IsSelfDefenseActive(defender))
        {
            // Attacking during self defense period does not initiate another self defense.
            return;
        }

        if (hitInfo is { HealthDamage: 0, ShieldDamage: 0 } || hitInfo.Attributes.HasFlag(DamageAttributes.Reflected))
        {
            return;
        }

        var timeout = DateTime.UtcNow.Add(this.Configuration?.SelfDefenseTimeOut ?? TimeSpan.FromMinutes(1));
        var gameContext = defender.GameContext;
        gameContext.SelfDefenseState.AddOrUpdate(
            (attackerPlayer, defender),
            tuple =>
            {
                _ = this.BeginSelfDefenseAsync(attackerPlayer, defender);
                return timeout;
            },
            (_, _) => timeout);
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return CreateDefaultConfiguration();
    }

    private static SelfDefensePlugInConfiguration CreateDefaultConfiguration()
    {
        return new SelfDefensePlugInConfiguration();
    }

    private async ValueTask BeginSelfDefenseAsync(Player attacker, Player defender)
    {
        await defender.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SelfDefenseInitiatedFormat), attacker.Name, defender.Name).ConfigureAwait(false);
        await attacker.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SelfDefenseInitiatedFormat), attacker.Name, defender.Name).ConfigureAwait(false);
    }

    private async ValueTask EndSelfDefenseAsync(Player attacker, Player defender)
    {
        await defender.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SelfDefenseDiminishesFormat), defender.Name, attacker.Name).ConfigureAwait(false);
        await attacker.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SelfDefenseDiminishesFormat), defender.Name, attacker.Name).ConfigureAwait(false);
    }
}