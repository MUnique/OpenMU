// <copyright file="BuffHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;

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
    /// Checks and applies buffs if configured and needed.
    /// </summary>
    /// <returns>True, if the loop can continue to the next step; False, if a buff was cast and the tick should end.</returns>
    public async ValueTask<bool> PerformBuffsAsync()
    {
        if (this._config is null)
        {
            return true;
        }

        var buffIds = this.GetConfiguredBuffIds();
        if (buffIds.Count == 0)
        {
            return true;
        }

        this.UpdatePeriodicBuffTimer();

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

            // Try to buff self
            if (await this.TryBuffTargetAsync(this._player, skillEntry, false).ConfigureAwait(false))
            {
                return false;
            }

            // Try to buff party members
            if (await this.TryBuffPartyMembersAsync(skillEntry).ConfigureAwait(false))
            {
                return false;
            }

            // Move to next slot if no one needed this buff
            this.MoveNextSlot();
        }

        return true;
    }

    private List<int> GetConfiguredBuffIds()
    {
        if (this._config is null)
        {
            return [];
        }

        return [this._config.BuffSkill0Id, this._config.BuffSkill1Id, this._config.BuffSkill2Id];
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

    private async ValueTask<bool> TryBuffPartyMembersAsync(SkillEntry skillEntry)
    {
        if (this._config is not { SupportParty: true } || this._player.Party is not { } party)
        {
            return false;
        }

        foreach (var member in party.PartyList.OfType<IAttackable>())
        {
            if (member == (IAttackable)this._player)
            {
                continue;
            }

            if (await this.TryBuffTargetAsync(member, skillEntry, true).ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }

    private void MoveNextSlot()
    {
        this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
        if (this._buffSkillIndex == 0)
        {
            this._buffTimerTriggered = false;
        }
    }

    private async ValueTask<bool> TryBuffTargetAsync(IAttackable target, SkillEntry skillEntry, bool isPartyMember)
    {
        if (skillEntry.Skill?.MagicEffectDef is not { } effectDef)
        {
            return false;
        }

        if (!target.IsActive() || !this._player.IsInRange(target, 8))
        {
            return false;
        }

        bool alreadyActive = target.MagicEffectList.ActiveEffects.Values
            .Any(e => e.Definition == effectDef);

        bool shouldApply;
        if (isPartyMember)
        {
            shouldApply = this._config!.BuffDurationForParty ? !alreadyActive : this._buffTimerTriggered;
        }
        else
        {
            shouldApply = !alreadyActive || this._buffTimerTriggered;
        }

        if (shouldApply)
        {
            await target.ApplyMagicEffectAsync(this._player, skillEntry).ConfigureAwait(false);
            this.MoveNextSlot();
            return true;
        }

        return false;
    }
}
