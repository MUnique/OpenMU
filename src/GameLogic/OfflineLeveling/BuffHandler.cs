// <copyright file="BuffHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Handles buff application and management for the offline leveling player.
/// </summary>
public sealed class BuffHandler
{
    private const int BuffSlotCount = 3;

    private readonly OfflineLevelingPlayer _player;
    private readonly MuHelperPlayerConfiguration? _config;

    private int _buffSkillIndex;
    private bool _buffTimerTriggered;
    private DateTime? _nextPeriodicBuffTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuffHandler"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    public BuffHandler(OfflineLevelingPlayer player, MuHelperPlayerConfiguration? config)
    {
        this._player = player;
        this._config = config;
    }

    /// <summary>
    /// Checks and applies self-buffs if configured and needed.
    /// </summary>
    /// <returns>True, if the loop can continue to the next step; False, if a buff was cast and the tick should end.</returns>
    public async ValueTask<bool> PerformBuffsAsync()
    {
        if (this._config is null)
        {
            return true;
        }

        int[] buffIds = [this._config.BuffSkill0Id, this._config.BuffSkill1Id, this._config.BuffSkill2Id];
        if (buffIds.All(id => id == 0))
        {
            return true;
        }

        if (!this._config.BuffOnDuration && this._config.BuffCastIntervalSeconds > 0)
        {
            this._nextPeriodicBuffTime ??= DateTime.UtcNow.AddSeconds(this._config.BuffCastIntervalSeconds);

            if (DateTime.UtcNow >= this._nextPeriodicBuffTime)
            {
                this._buffTimerTriggered = true;
                this._nextPeriodicBuffTime = DateTime.UtcNow.AddSeconds(this._config.BuffCastIntervalSeconds);
            }
        }

        for (int i = 0; i < BuffSlotCount; i++)
        {
            int buffId = buffIds[this._buffSkillIndex];
            if (buffId == 0)
            {
                // Slot is empty, skip to the next one immediately.
                this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
                if (this._buffSkillIndex == 0)
                {
                    this._buffTimerTriggered = false;
                }

                continue;
            }

            var skillEntry = this._player.SkillList?.GetSkill((ushort)buffId);
            if (skillEntry?.Skill?.MagicEffectDef is null)
            {
                // Skill not found or has no effect, skip to the next one immediately.
                this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
                if (this._buffSkillIndex == 0)
                {
                    this._buffTimerTriggered = false;
                }

                continue;
            }

            bool alreadyActive = this._player.MagicEffectList.ActiveEffects.Values
                .Any(e => e.Definition == skillEntry.Skill.MagicEffectDef);

            if (!alreadyActive || this._buffTimerTriggered)
            {
                await this._player.ApplyMagicEffectAsync(this._player, skillEntry).ConfigureAwait(false);

                // Move index for the next tick so we don't spam the same slot.
                this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
                if (this._buffSkillIndex == 0)
                {
                    this._buffTimerTriggered = false;
                }

                return false;
            }

            // If the current buff is already active and no timer was triggered, we cycle to the next slot
            // immediately to avoid wasting 500ms (one full tick) on an already-active buff.
            this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
            if (this._buffSkillIndex == 0)
            {
                this._buffTimerTriggered = false;
                break;
            }
        }

        return true;
    }
}
