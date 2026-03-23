// <copyright file="BuffHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Handles buff application and management for the offline leveling player.
/// </summary>
public sealed class BuffHandler
{
    private const int BuffSlotCount = 3;

    private readonly OfflineLevelingPlayer _player;
    private readonly IMuHelperSettings? _config;

    private int _buffSkillIndex;
    private bool _buffTimerTriggered;
    private DateTime? _nextPeriodicBuffTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuffHandler"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    public BuffHandler(OfflineLevelingPlayer player, IMuHelperSettings? config)
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

        this._buffSkillIndex = 0;
        for (int i = 0; i < BuffSlotCount; i++)
        {
            int buffId = buffIds[this._buffSkillIndex];
            if (buffId == 0)
            {
                this.MoveNextSlot();
                continue;
            }

            var skillEntry = this._player.SkillList?.GetSkill((ushort)buffId);
            if (skillEntry?.Skill?.MagicEffectDef is null)
            {
                this.MoveNextSlot();
                continue;
            }

            if (await this.PerformSelfBuffAsync(skillEntry).ConfigureAwait(false))
            {
                return false;
            }

            if (await this.PerformPartyBuffAsync(skillEntry).ConfigureAwait(false))
            {
                return false;
            }

            this.MoveNextSlot();
        }

        return true;
    }

    private static bool IsSkillQualifiedForTarget(SkillEntry skillEntry)
    {
        if (skillEntry.Skill is not { } skill)
        {
            return false;
        }

        return skill.Target != SkillTarget.ImplicitPlayer
               && skill.TargetRestriction != SkillTargetRestriction.Self;
    }

    private void UpdatePeriodicBuffTimer()
    {
        if (this._config is null || this._config.BuffOnDuration || this._config.BuffCastIntervalSeconds <= 0)
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

    private void MoveNextSlot()
    {
        this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
        if (this._buffSkillIndex == 0)
        {
            this._buffTimerTriggered = false;
        }
    }

    private bool ShouldApplyBuff(IAttackable target, SkillEntry skillEntry, bool isPartyMember)
    {
        if (skillEntry.Skill?.MagicEffectDef is not { } effectDef)
        {
            return false;
        }

        if (!target.IsActive() || !this._player.IsInRange(target, this._config!.HuntingRange))
        {
            return false;
        }

        var alreadyActive = target.MagicEffectList.ActiveEffects.Values
            .Any(e => e.Definition == effectDef);

        if (isPartyMember)
        {
            return this._config!.BuffDurationForParty ? !alreadyActive : this._buffTimerTriggered;
        }

        return !alreadyActive || this._buffTimerTriggered;
    }

    private async ValueTask<bool> PerformSelfBuffAsync(SkillEntry skillEntry)
    {
        if (!this.ShouldApplyBuff(this._player, skillEntry, false))
        {
            return false;
        }

        if (skillEntry.Skill?.Target == SkillTarget.ImplicitParty)
        {
            return await this.PerformImplicitPartyBuffAsync(skillEntry).ConfigureAwait(false);
        }

        await this._player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(
            p => p.ShowSkillAnimationAsync(this._player, this._player, skillEntry.Skill!, true),
            includeThis: true).ConfigureAwait(false);
        await this._player.ApplyMagicEffectAsync(this._player, skillEntry).ConfigureAwait(false);
        this.MoveNextSlot();
        return true;
    }

    private async ValueTask<bool> PerformImplicitPartyBuffAsync(SkillEntry skillEntry)
    {
        var strategy = this._player.GameContext.PlugInManager
                           .GetStrategy<short, ITargetedSkillPlugin>(skillEntry.Skill!.Number)
                       ?? new TargetedSkillDefaultPlugin();

        await strategy.PerformSkillAsync(this._player, this._player, (ushort)skillEntry.Skill.Number).ConfigureAwait(false);
        this.MoveNextSlot();
        return true;
    }

    private async ValueTask<bool> PerformPartyBuffAsync(SkillEntry skillEntry)
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

            if (!IsSkillQualifiedForTarget(skillEntry))
            {
                continue;
            }

            if (!this.ShouldApplyBuff(member, skillEntry, true))
            {
                continue;
            }

            await (member as IObservable ?? this._player).ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(
                p => p.ShowSkillAnimationAsync(this._player, member, skillEntry.Skill!, true),
                includeThis: true).ConfigureAwait(false);
            await member.ApplyMagicEffectAsync(this._player, skillEntry).ConfigureAwait(false);
            this.MoveNextSlot();
            return true;
        }

        return false;
    }
}