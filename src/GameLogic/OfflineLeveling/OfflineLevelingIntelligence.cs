// <copyright file="OfflineLevelingIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Server-side AI that drives an <see cref="OfflineLevelingPlayer"/> ghost after the real
/// client disconnects. Mirrors the C++ <c>CMuHelper::Work()</c> loop including:
/// <list type="bullet">
///   <item>Basic / conditional / combo skill attack selection</item>
///   <item>Self-buff application (up to 3 configured buff skills)</item>
///   <item>Auto-heal / drain-life based on HP %</item>
///   <item>Return-to-origin regrouping</item>
///   <item>Item pickup (Zen, Jewels, Excellent, Ancient, and named extra items)</item>
///   <item>Skill and movement animations broadcast to nearby observers</item>
/// </list>
/// Party support is not implemented.
/// </summary>
public sealed class OfflineLevelingIntelligence : IDisposable
{
    private const byte FallbackViewRange = 10;
    private const byte FallbackAttackRange = 2;
    private const byte JewelItemGroup = 14;

    // How many 500 ms ticks equal one "second".
    private const int TicksPerSecond = 2;

    private static readonly PickupItemAction PickupAction = new();

    private readonly OfflineLevelingPlayer _player;
    private readonly MuHelperPlayerConfiguration? _config;

    private Timer? _aiTimer;
    private bool _disposed;

    private int _tickCounter;
    private int _secondsElapsed;

    private IAttackable? _currentTarget;
    private int _nearbyMonsterCount;
    private int _comboStep;

    private int _buffSkillIndex;
    private bool _buffTimerTriggered;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineLevelingIntelligence"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    public OfflineLevelingIntelligence(OfflineLevelingPlayer player)
    {
        this._player = player;
        this._config = MuHelperPlayerConfiguration.TryDeserialize(player.SelectedCharacter?.MuHelperConfiguration);
    }

    /// <summary>Starts the 500 ms AI timer.</summary>
    public void Start()
    {
        this._aiTimer ??= new Timer(
            _ => this.SafeTick(),
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromMilliseconds(500));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        this._disposed = true;
        this._aiTimer?.Dispose();
        this._aiTimer = null;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Timer callback — exceptions are caught internally.")]
    private async void SafeTick()
    {
        try
        {
            await this.TickAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(
                ex,
                "Error in offline leveling AI tick for {Name}.",
                this._player.CharacterName);
        }
    }

    private async ValueTask TickAsync()
    {
        if (!this._player.IsAlive
            || this._player.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            return;
        }

        if (++this._tickCounter >= TicksPerSecond)
        {
            this._tickCounter = 0;
            this._secondsElapsed++;
        }

        // CMuHelper::Work() order: Buff → RecoverHealth → ObtainItem → Regroup → Attack
        if (!await this.BuffSelfAsync().ConfigureAwait(false))
        {
            return;
        }

        if (!await this.RecoverHealthAsync().ConfigureAwait(false))
        {
            return;
        }

        await this.PickupItemsAsync().ConfigureAwait(false);

        await this.AttackAsync().ConfigureAwait(false);
    }

    // ── Step 1 – Buff ──────────────────────────────────────────────────────────
    private async ValueTask<bool> BuffSelfAsync()
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

        if (!this._config.BuffOnDuration
            && this._config.BuffCastIntervalSeconds > 0
            && this._secondsElapsed > 0
            && this._secondsElapsed % this._config.BuffCastIntervalSeconds == 0)
        {
            this._buffTimerTriggered = true;
        }

        int buffId = buffIds[this._buffSkillIndex];
        if (buffId == 0)
        {
            this._buffSkillIndex = (this._buffSkillIndex + 1) % 3;
            return true;
        }

        var skillEntry = this._player.SkillList?.GetSkill((ushort)buffId);
        if (skillEntry?.Skill?.MagicEffectDef is null)
        {
            this._buffSkillIndex = (this._buffSkillIndex + 1) % 3;
            return true;
        }

        bool alreadyActive = this._player.MagicEffectList.ActiveEffects.Values
            .Any(e => e.Definition == skillEntry.Skill.MagicEffectDef);

        if (!alreadyActive || this._buffTimerTriggered)
        {
            await this._player.ApplyMagicEffectAsync(this._player, skillEntry).ConfigureAwait(false);

            this._buffSkillIndex = (this._buffSkillIndex + 1) % 3;
            if (this._buffSkillIndex == 0)
            {
                this._buffTimerTriggered = false;
            }

            return false;
        }

        this._buffSkillIndex = (this._buffSkillIndex + 1) % 3;
        return true;
    }

    // ── Step 2 – Recover health ────────────────────────────────────────────────
    private async ValueTask<bool> RecoverHealthAsync()
    {
        if (this._config is null || this._player.Attributes is null)
        {
            return true;
        }

        double hp = this._player.Attributes[Stats.CurrentHealth];
        double maxHp = this._player.Attributes[Stats.MaximumHealth];
        if (maxHp <= 0)
        {
            return true;
        }

        int hpPercent = (int)(hp * 100.0 / maxHp);

        if (this._config.AutoHeal && hpPercent <= this._config.HealThresholdPercent)
        {
            var healSkill = this.FindSkillByType(SkillType.Regeneration);
            if (healSkill is not null)
            {
                await this._player.ApplyRegenerationAsync(this._player, healSkill).ConfigureAwait(false);
                return false;
            }
        }

        if (this._config.UseDrainLife && hpPercent <= this._config.HealThresholdPercent)
        {
            var drainSkill = this.FindDrainLifeSkill();
            if (drainSkill is not null)
            {
                this.RefreshTarget();
                if (this._currentTarget is not null)
                {
                    await this.AttackTargetAsync(this._currentTarget, drainSkill).ConfigureAwait(false);
                    return false;
                }
            }
        }

        return true;
    }

    // ── Step 3 – Item pickup ───────────────────────────────────────────────────
    private async ValueTask PickupItemsAsync()
    {
        if (this._config is null || this._player.CurrentMap is not { } map)
        {
            return;
        }

        // Nothing configured to pick up.
        if (!this._config.PickAllItems
            && !this._config.PickJewel
            && !this._config.PickZen
            && !this._config.PickAncient
            && !this._config.PickExcellent
            && !(this._config.PickExtraItems && this._config.ExtraItemNames.Count > 0))
        {
            return;
        }

        byte range = (byte)Math.Max(this._config.ObtainRange * 2, 3);
        var drops = map.GetDropsInRange(this._player.Position, range);

        foreach (var drop in drops)
        {
            switch (drop)
            {
                case DroppedMoney when this._config.PickZen:
                    await PickupAction.PickupItemAsync(this._player, drop.Id).ConfigureAwait(false);
                    break;

                case DroppedItem droppedItem when this.ShouldPickUp(droppedItem.Item):
                    await PickupAction.PickupItemAsync(this._player, drop.Id).ConfigureAwait(false);
                    break;
            }
        }
    }

    private bool ShouldPickUp(Item item)
    {
        if (this._config is null)
        {
            return false;
        }

        if (this._config.PickAllItems)
        {
            return true;
        }

        if (this._config.PickJewel && item.Definition?.Group == JewelItemGroup)
        {
            return true;
        }

        if (this._config.PickAncient && item.ItemSetGroups.Any(s => s.AncientSetDiscriminator != 0))
        {
            return true;
        }

        if (this._config.PickExcellent &&
            item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent))
        {
            return true;
        }

        if (this._config.PickExtraItems && this._config.ExtraItemNames.Count > 0)
        {
            var itemName = item.Definition?.Name.ValueInNeutralLanguage;
            if (itemName is not null
                && this._config.ExtraItemNames.Any(n =>
                    itemName.Contains(n, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return false;
    }

    // ── Step 4 – Attack ────────────────────────────────────────────────────────
    private async ValueTask AttackAsync()
    {
        this.RefreshTarget();

        if (this._currentTarget is null)
        {
            this._comboStep = 0;
            return;
        }

        byte range = this.GetEffectiveAttackRange();

        if (!this._currentTarget.IsInRange(this._player.Position, range))
        {
            return;
        }

        if (this._config?.UseCombo == true)
        {
            await this.ExecuteComboAttackAsync().ConfigureAwait(false);
        }
        else
        {
            var skill = this.SelectAttackSkill();
            await this.AttackTargetAsync(this._currentTarget, skill).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Attacks the target and broadcasts the skill/attack animation to nearby observers.
    /// Without this broadcast, other players cannot see the ghost's animations.
    /// </summary>
    private async ValueTask AttackTargetAsync(IAttackable target, SkillEntry? skillEntry)
    {
        this._player.Rotation = this._player.GetDirectionTo(target);

        if (skillEntry?.Skill is { } skill)
        {
            if (skill.SkillType is SkillType.AreaSkillAutomaticHits or SkillType.AreaSkillExplicitTarget
                or SkillType.AreaSkillExplicitHits)
            {
                // Broadcast the area skill animation to all nearby observers.
                var rotationByte = (byte)(this._player.Position.GetAngleDegreeTo(target.Position) / 360.0 * 255.0);
                await this._player.ForEachWorldObserverAsync<IShowAreaSkillAnimationPlugIn>(
                    p => p.ShowAreaSkillAnimationAsync(this._player, skill, target.Position, rotationByte),
                    includeThis: true).ConfigureAwait(false);

                // Hit only killable Monster targets in range — never players, guards, or NPCs.
                var monstersInRange = this._player.CurrentMap?
                                          .GetAttackablesInRange(target.Position, skill.Range)
                                          .OfType<Monster>()
                                          .Where(m => m.IsAlive && !m.IsAtSafezone()
                                                                && m.Definition.ObjectKind == NpcObjectKind.Monster)
                                      ?? [];
                foreach (var monster in monstersInRange)
                {
                    await monster.AttackByAsync(this._player, skillEntry, false).ConfigureAwait(false);
                }
            }
            else
            {
                var strategy =
                    this._player.GameContext.PlugInManager.GetStrategy<short, ITargetedSkillPlugin>((short)skill.Number)
                    ?? new TargetedSkillDefaultPlugin();
                await strategy.PerformSkillAsync(this._player, target, (ushort)skill.Number).ConfigureAwait(false);
            }
        }
        else
        {
            // Generic fallback attack if no skill is assigned or evaluated.
            await target.AttackByAsync(this._player, null, false).ConfigureAwait(false);

            await this._player.ForEachWorldObserverAsync<IShowAnimationPlugIn>(
                p => p.ShowAnimationAsync(this._player, 120, target, this._player.Rotation),
                includeThis: true).ConfigureAwait(false);
        }
    }

    private void RefreshTarget()
    {
        if (this._currentTarget is { } t
            && (!t.IsAlive || t.IsAtSafezone() || t.IsTeleporting
                || !t.IsInRange(this._player.Position, (byte)(this.GetHuntingRange() + 4))))
        {
            this._currentTarget = null;
        }

        if (this._currentTarget is null)
        {
            this._currentTarget = this.FindNearestMonster();
        }

        this._nearbyMonsterCount = this.CountMonstersNearby();
    }

    private IAttackable? FindNearestMonster()
    {
        if (this._player.CurrentMap is not { } map)
        {
            return null;
        }

        byte range = this.GetHuntingRange();
        return map.GetAttackablesInRange(this._player.Position, range)
            .OfType<Monster>()
            .Where(m => m.IsAlive && !m.IsAtSafezone()
                                  && m.Definition.ObjectKind == NpcObjectKind.Monster)
            .MinBy(m => m.GetDistanceTo(this._player));
    }

    private int CountMonstersNearby()
    {
        if (this._player.CurrentMap is not { } map)
        {
            return 0;
        }

        return map.GetAttackablesInRange(this._player.Position, this.GetHuntingRange())
            .OfType<Monster>()
            .Count(m => m.IsAlive && !m.IsAtSafezone()
                                  && m.Definition.ObjectKind == NpcObjectKind.Monster);
    }

    private SkillEntry? SelectAttackSkill()
    {
        if (this._config is null)
        {
            return this.GetAnyOffensiveSkill();
        }

        var s1 = this.EvaluateConditionalSkill(
            this._config.ActivationSkill1Id,
            this._config.Skill1UseTimer, this._config.DelayMinSkill1,
            this._config.Skill1UseCondition, this._config.Skill1ConditionAttacking,
            this._config.Skill1SubCondition);
        if (s1 is not null)
        {
            return s1;
        }

        var s2 = this.EvaluateConditionalSkill(
            this._config.ActivationSkill2Id,
            this._config.Skill2UseTimer, this._config.DelayMinSkill2,
            this._config.Skill2UseCondition, this._config.Skill2ConditionAttacking,
            this._config.Skill2SubCondition);
        if (s2 is not null)
        {
            return s2;
        }

        if (this._config.BasicSkillId > 0)
        {
            return this._player.SkillList?.GetSkill((ushort)this._config.BasicSkillId);
        }

        return this.GetAnyOffensiveSkill();
    }

    private SkillEntry? EvaluateConditionalSkill(
        int skillId,
        bool useTimer, int timerInterval,
        bool useCond, bool conditionIsAttacking,
        int subCond)
    {
        if (skillId <= 0)
        {
            return null;
        }

        if (useTimer && timerInterval > 0
                     && this._secondsElapsed > 0
                     && this._secondsElapsed % timerInterval == 0)
        {
            return this._player.SkillList?.GetSkill((ushort)skillId);
        }

        if (useCond)
        {
            int threshold = subCond switch
            {
                0 => 2,
                1 => 3,
                2 => 4,
                3 => 5,
                _ => int.MaxValue,
            };

            if (this._nearbyMonsterCount >= threshold)
            {
                return this._player.SkillList?.GetSkill((ushort)skillId);
            }
        }

        return null;
    }

    private async ValueTask ExecuteComboAttackAsync()
    {
        if (this._config is null || this._currentTarget is null)
        {
            return;
        }

        int[] ids = [this._config.BasicSkillId, this._config.ActivationSkill1Id, this._config.ActivationSkill2Id];
        SkillEntry? skill = ids.Any(id => id == 0)
            ? null
            : this._player.SkillList?.GetSkill((ushort)ids[this._comboStep]);

        await this.AttackTargetAsync(this._currentTarget, skill).ConfigureAwait(false);
        this._comboStep = (this._comboStep + 1) % 3;
    }

    private byte GetHuntingRange()
    {
        if (this._config is null || this._config.HuntingRange == 0)
        {
            return FallbackViewRange;
        }

        return (byte)Math.Min(this._config.HuntingRange * 2, FallbackViewRange);
    }

    private byte GetEffectiveAttackRange()
    {
        if (this._config?.BasicSkillId > 0)
        {
            var skill = this._player.SkillList?.GetSkill((ushort)this._config.BasicSkillId);
            if (skill?.Skill?.Range is { } r && r > 0)
            {
                return (byte)r;
            }
        }

        return this._player.SkillList?.Skills
            .Where(s => s.Skill is not null
                        && s.Skill.SkillType != SkillType.PassiveBoost
                        && s.Skill.SkillType != SkillType.Buff
                        && s.Skill.SkillType != SkillType.Regeneration)
            .Select(s => (byte)s.Skill!.Range)
            .DefaultIfEmpty(FallbackAttackRange)
            .Max() ?? FallbackAttackRange;
    }

    private SkillEntry? GetAnyOffensiveSkill()
        => this._player.SkillList?.Skills.FirstOrDefault(s =>
            s.Skill is not null
            && s.Skill.SkillType != SkillType.PassiveBoost
            && s.Skill.SkillType != SkillType.Buff
            && s.Skill.SkillType != SkillType.Regeneration);

    private SkillEntry? FindSkillByType(SkillType type)
        => this._player.SkillList?.Skills.FirstOrDefault(s => s.Skill?.SkillType == type);

    private SkillEntry? FindDrainLifeSkill()
        => this._player.SkillList?.Skills.FirstOrDefault(s =>
            s.Skill is not null
            && s.Skill.Name.ValueInNeutralLanguage.Contains("Drain Life", StringComparison.OrdinalIgnoreCase));
}