// <copyright file="BuffHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Handles buff application and management for the offline player.
/// </summary>
public sealed class BuffHandler
{
    private const int BuffSlotCount = 3;

    private static readonly TargetedSkillDefaultPlugin DefaultPlugin = new();

    private readonly OfflinePlayer _player;
    private readonly IMuHelperSettings? _config;

    private int _nextSlotIndex;
    private bool _buffTimerTriggered;
    private DateTime? _nextPeriodicBuffTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuffHandler"/> class.
    /// </summary>
    /// <param name="player">The offline player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    public BuffHandler(OfflinePlayer player, IMuHelperSettings? config)
    {
        this._player = player;
        this._config = config;
    }

    /// <summary>
    /// Gets the configured buff skill IDs from the settings.
    /// </summary>
    public IList<int> ConfiguredBuffIds
    {
        get
        {
            if (this._config is null)
            {
                return [];
            }

            return [this._config.BuffSkill0Id, this._config.BuffSkill1Id, this._config.BuffSkill2Id];
        }
    }

    /// <summary>
    /// Checks and applies buffs if configured and needed.
    /// </summary>
    /// <returns>True, if the loop can continue to the next step; False, if a buff was cast and the tick should end.</returns>
    public async ValueTask<bool> PerformBuffsAsync()
    {
        if (this._config is null)
        {
            return true;
        }

        var buffIds = this.ConfiguredBuffIds;
        if (buffIds.Count == 0)
        {
            return true;
        }

        this.UpdatePeriodicBuffTimer();

        for (int i = 0; i < BuffSlotCount; i++)
        {
            var slotIndex = (this._nextSlotIndex + i) % BuffSlotCount;
            int buffId = buffIds[slotIndex];
            if (buffId == 0)
            {
                continue;
            }

            var skillEntry = this._player.SkillList?.GetSkill((ushort)buffId);
            if (skillEntry?.Skill?.MagicEffectDef is null)
            {
                continue;
            }

            if (await this.TryApplyBuffAsync(skillEntry).ConfigureAwait(false))
            {
                this._nextSlotIndex = (slotIndex + 1) % BuffSlotCount;
                this._buffTimerTriggered = false;
                return false;
            }
        }

        this._nextSlotIndex = 0;
        this._buffTimerTriggered = false;
        return true;
    }

    /// <summary>
    /// Attempts to apply the buff to self and, if applicable, to party members.
    /// </summary>
    /// <returns>True if a buff was applied and the tick should end.</returns>
    private async ValueTask<bool> TryApplyBuffAsync(SkillEntry skillEntry)
    {
        if (await this.TryApplySelfBuffAsync(skillEntry).ConfigureAwait(false))
        {
            return true;
        }

        if (this.IsSelfOnlySkill(skillEntry))
        {
            return false;
        }

        return await this.TryApplyPartyBuffAsync(skillEntry).ConfigureAwait(false);
    }

    /// <summary>
    /// Attempts to apply the buff to the player.
    /// For <see cref="SkillTarget.ImplicitParty"/> skills, delegates to the skill plugin
    /// which handles applying to all visible party members at once.
    /// </summary>
    /// <returns>True if the buff was applied.</returns>
    private async ValueTask<bool> TryApplySelfBuffAsync(SkillEntry skillEntry)
    {
        if (!this.NeedsBuff(this._player, skillEntry))
        {
            return false;
        }

        if (skillEntry.Skill?.Target == SkillTarget.ImplicitParty)
        {
            var strategy = this._player.GameContext.PlugInManager
                               .GetStrategy<short, ITargetedSkillPlugin>(skillEntry.Skill!.Number)
                           ?? DefaultPlugin;

            await strategy.PerformSkillAsync(this._player, this._player, (ushort)skillEntry.Skill.Number).ConfigureAwait(false);
            return true;
        }

        await this._player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(
            p => p.ShowSkillAnimationAsync(this._player, this._player, skillEntry.Skill!, true),
            includeThis: true).ConfigureAwait(false);
        await this._player.ApplyMagicEffectAsync(this._player, skillEntry).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Attempts to apply the buff to the first party member that needs it.
    /// </summary>
    /// <returns>True if the buff was applied to a party member.</returns>
    private async ValueTask<bool> TryApplyPartyBuffAsync(SkillEntry skillEntry)
    {
        if (this._config is not { SupportParty: true } || this._player.Party is not { } party)
        {
            return false;
        }

        foreach (var member in party.PartyList.OfType<IAttackable>())
        {
            if (member == this._player)
            {
                continue;
            }

            if (!this.NeedsPartyBuff(member, skillEntry))
            {
                continue;
            }

            await (member as IObservable ?? this._player).ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(
                p => p.ShowSkillAnimationAsync(this._player, member, skillEntry.Skill!, true),
                includeThis: true).ConfigureAwait(false);
            await member.ApplyMagicEffectAsync(this._player, skillEntry).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the buff should be applied to the player.
    /// Returns true if the effect is not yet active, or if it is active
    /// and the periodic re-buff timer has triggered with <see cref="IMuHelperSettings.BuffOnDuration"/> enabled.
    /// </summary>
    private bool NeedsBuff(IAttackable target, SkillEntry skillEntry)
    {
        if (!this.IsTargetReachable(target))
        {
            return false;
        }

        if (!this.IsEffectActive(target, skillEntry))
        {
            return true;
        }

        return this._config!.BuffOnDuration && this._buffTimerTriggered;
    }

    /// <summary>
    /// Determines whether the buff should be applied to a party member.
    /// Returns true if the effect is not yet active, or if it is active
    /// and the periodic re-buff timer has triggered with <see cref="IMuHelperSettings.BuffDurationForParty"/> enabled.
    /// </summary>
    private bool NeedsPartyBuff(IAttackable target, SkillEntry skillEntry)
    {
        if (!this.IsTargetReachable(target))
        {
            return false;
        }

        if (!this.IsEffectActive(target, skillEntry))
        {
            return true;
        }

        return this._config!.BuffDurationForParty && this._buffTimerTriggered;
    }

    private bool IsSelfOnlySkill(SkillEntry skillEntry)
    {
        if (skillEntry.Skill is not { } skill)
        {
            return true;
        }

        return skill.Target == SkillTarget.ImplicitPlayer
               || skill.TargetRestriction == SkillTargetRestriction.Self;
    }

    private bool IsTargetReachable(IAttackable target)
    {
        return target.IsActive() && this._player.IsInRange(target, this._config!.HuntingRange);
    }

    private bool IsEffectActive(IAttackable target, SkillEntry skillEntry)
    {
        var effectDef = skillEntry.Skill?.MagicEffectDef;
        if (effectDef is null)
        {
            return false;
        }

        return target.MagicEffectList.ActiveEffects.Values
            .Any(e => e.Definition == effectDef);
    }

    private void UpdatePeriodicBuffTimer()
    {
        if (this._config is null || this._config.BuffCastIntervalSeconds <= 0)
        {
            return;
        }

        this._nextPeriodicBuffTime ??= DateTime.UtcNow.AddSeconds(this._config.BuffCastIntervalSeconds);

        if (DateTime.UtcNow >= this._nextPeriodicBuffTime)
        {
            this._buffTimerTriggered = true;
            this._nextPeriodicBuffTime = DateTime.UtcNow.AddSeconds(this._config.BuffCastIntervalSeconds);
        }
    }
}