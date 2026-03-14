// <copyright file="OfflineLevelingIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Pathfinding;
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
    private const byte FallbackViewRange = 5;
    private const byte FallbackAttackRange = 2;
    private const byte MinPickupRange = 3;
    private const byte RegroupDistanceThreshold = 1;

    private const byte JewelItemGroup = 14;

    private const int TicksPerSecond = 2;
    private const int ComboFinisherDelayTicks = 3;
    private const int InterSkillDelayTicks = 1;

    private const int BuffSlotCount = 3;

    private const byte PhysicalAttackAnimationId = 120;
    private const short DrainLifeBaseSkillId = 214;
    private const short DrainLifeStrengthenerSkillId = 458;
    private const short DrainLifeMasterySkillId = 462;

    private static readonly PickupItemAction PickupAction = new();

    private readonly OfflineLevelingPlayer _player;
    private readonly MuHelperPlayerConfiguration? _config;

    private Timer? _aiTimer;
    private bool _disposed;

    // Tick State
    private int _tickCounter;
    private int _secondsElapsed;

    // Combat State
    private IAttackable? _currentTarget;
    private int _nearbyMonsterCount;
    private int _currentComboStep;
    private int _skillCooldownTicks;

    // Buff State
    private int _buffSkillIndex;
    private bool _buffTimerTriggered;

    // Movement State
    private readonly Point _originalPosition;
    private int _secondsAwayFromOrigin;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineLevelingIntelligence"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    public OfflineLevelingIntelligence(OfflineLevelingPlayer player)
    {
        this._player = player;
        this._config = MuHelperPlayerConfiguration.TryDeserialize(player.SelectedCharacter?.MuHelperConfiguration);
        this._originalPosition = player.Position;
    }

    /// <summary>Starts the 500 ms AI timer.</summary>
    public void Start()
    {
        this._aiTimer ??= new Timer(
            state => _ = this.SafeTickAsync(),
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
    private async Task SafeTickAsync()
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

        if (this._skillCooldownTicks > 0)
        {
            this._skillCooldownTicks--;
            return;
        }

        // CMuHelper::Work() order: Buff → RecoverHealth → ObtainItem → Regroup → Attack
        if (!await this.PerformBuffsAsync().ConfigureAwait(false))
        {
            return;
        }

        if (!await this.RecoverHealthAsync().ConfigureAwait(false))
        {
            return;
        }

        await this.PickupItemsAsync().ConfigureAwait(false);

        if (this._player.IsWalking)
        {
            return;
        }

        if (!await this.RegroupAsync().ConfigureAwait(false))
        {
            return;
        }

        await this.AttackTargetsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Checks and applies self-buffs if configured and needed.
    /// </summary>
    /// <returns>True, if the loop can continue to the next step; False, if a buff was cast and the tick should end.</returns>
    private async ValueTask<bool> PerformBuffsAsync()
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
            this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
            return true;
        }

        var skillEntry = this._player.SkillList?.GetSkill((ushort)buffId);
        if (skillEntry?.Skill?.MagicEffectDef is null)
        {
            this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
            return true;
        }

        bool alreadyActive = this._player.MagicEffectList.ActiveEffects.Values
            .Any(e => e.Definition == skillEntry.Skill.MagicEffectDef);

        if (!alreadyActive || this._buffTimerTriggered)
        {
            await this._player.ApplyMagicEffectAsync(this._player, skillEntry).ConfigureAwait(false);

            this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
            if (this._buffSkillIndex == 0)
            {
                this._buffTimerTriggered = false;
            }

            return false;
        }

        this._buffSkillIndex = (this._buffSkillIndex + 1) % BuffSlotCount;
        return true;
    }

    /// <summary>
    /// Manages HP recovery via Regeneration or Drain Life skills.
    /// </summary>
    /// <returns>True, if the loop can continue; False, if a recovery skill was used.</returns>
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
                    await this.PerformAttackAsync(this._currentTarget, drainSkill, false).ConfigureAwait(false);
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Scans for and picks up items within configurable range.
    /// </summary>
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

        byte range = (byte)Math.Max(this._config.ObtainRange, MinPickupRange);
        var drops = map.GetDropsInRange(this._player.Position, range);

        foreach (var drop in drops)
        {
            if (drop is DroppedMoney && this._config.PickZen)
            {
                await PickupAction.PickupItemAsync(this._player, drop.Id).ConfigureAwait(false);
            }
            else if (drop is DroppedItem droppedItem && this.ShouldPickUp(droppedItem.Item))
            {
                await PickupAction.PickupItemAsync(this._player, drop.Id).ConfigureAwait(false);
            }
            else
            {
                // Other drops are ignored by configuration.
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

    /// <summary>
    /// Returns the character to the original position if configured and distance/time thresholds are met.
    /// </summary>
    /// <returns>True, if the loop can continue; False, if a regrouping walk was initiated.</returns>
    private async ValueTask<bool> RegroupAsync()
    {
        if (this._config is null || !this._config.ReturnToOriginalPosition)
        {
            return true;
        }

        var distance = this._player.GetDistanceTo(this._originalPosition);
        if (distance > RegroupDistanceThreshold)
        {
            if (this._tickCounter == 0)
            {
                this._secondsAwayFromOrigin++;
            }
        }
        else
        {
            this._secondsAwayFromOrigin = 0;
            return true;
        }

        if (this._secondsAwayFromOrigin >= this._config.MaxSecondsAway || distance > this.GetHuntingRange())
        {
            await this.WalkToAsync(this._originalPosition).ConfigureAwait(false);
            this._secondsAwayFromOrigin = 0;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Main combat logic: selects a target and performs single or combo attacks.
    /// </summary>
    private async ValueTask AttackTargetsAsync()
    {
        this.RefreshTarget();

        if (this._currentTarget is null)
        {
            this._currentComboStep = 0;
            return;
        }

        byte range = this.GetEffectiveAttackRange();

        if (!this.IsTargetInAttackRange(this._currentTarget, range))
        {
            await this.MoveCloserToTargetAsync(this._currentTarget, range).ConfigureAwait(false);
            return;
        }

        if (this._config?.UseCombo == true)
        {
            await this.ExecuteComboAttackAsync().ConfigureAwait(false);
        }
        else
        {
            var skill = this.SelectAttackSkill();
            await this.PerformAttackAsync(this._currentTarget, skill, false).ConfigureAwait(false);
        }
    }

    private bool IsTargetInAttackRange(IAttackable target, byte range)
    {
        return target.IsInRange(this._player.Position, range);
    }

    private async ValueTask MoveCloserToTargetAsync(IAttackable target, byte range)
    {
        // Move closer to the target if it's within hunting range relative to origin.
        if (target.IsInRange(this._originalPosition, this.GetHuntingRange()))
        {
            var walkTarget = this._player.CurrentMap!.Terrain.GetRandomCoordinate(target.Position, range);
            await this.WalkToAsync(walkTarget).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Executes an attack against a target, handling skill types and observer animations.
    /// </summary>
    private async ValueTask PerformAttackAsync(IAttackable target, SkillEntry? skillEntry, bool isCombo)
    {
        this._player.Rotation = this._player.GetDirectionTo(target);

        if (skillEntry?.Skill is not { } skill)
        {
            await this.PerformPhysicalAttackAsync(target).ConfigureAwait(false);
            return;
        }

        if (skill.SkillType is SkillType.AreaSkillAutomaticHits or SkillType.AreaSkillExplicitTarget or SkillType.AreaSkillExplicitHits)
        {
            await this.PerformAreaSkillAttackAsync(target, skillEntry, isCombo).ConfigureAwait(false);
        }
        else
        {
            await this.PerformTargetedSkillAttackAsync(target, skill).ConfigureAwait(false);
        }
    }

    private async ValueTask PerformPhysicalAttackAsync(IAttackable target)
    {
        // Generic fallback attack if no skill is assigned or evaluated.
        await target.AttackByAsync(this._player, null, false).ConfigureAwait(false);

        await this._player.ForEachWorldObserverAsync<IShowAnimationPlugIn>(
            p => p.ShowAnimationAsync(this._player, PhysicalAttackAnimationId, target, this._player.Rotation),
            includeThis: true).ConfigureAwait(false);
    }

    private async ValueTask PerformAreaSkillAttackAsync(IAttackable target, SkillEntry skillEntry, bool isCombo)
    {
        var skill = skillEntry.Skill!;

        if (isCombo)
        {
            // Broadcast the combo finisher animation (the "boom") to all observers.
            await this._player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(
                p => p.ShowComboAnimationAsync(this._player, target),
                includeThis: true).ConfigureAwait(false);
        }

        // Broadcast the area skill animation to all nearby observers.
        // Rotation is converted from 360 degrees to a single byte (0-255) as per protocol.
        var rotationByte = (byte)(this._player.Position.GetAngleDegreeTo(target.Position) / 360.0 * 255.0);
        await this._player.ForEachWorldObserverAsync<IShowAreaSkillAnimationPlugIn>(
            p => p.ShowAreaSkillAnimationAsync(this._player, skill, target.Position, rotationByte),
            includeThis: true).ConfigureAwait(false);

        // Hit only killable Monster targets in range, never players, guards, or NPCs.
        var monstersInRange = this._player.CurrentMap?
                                  .GetAttackablesInRange(target.Position, skill.Range)
                                  .OfType<Monster>()
                                  .Where(m => m.IsAlive && !m.IsAtSafezone()
                                                        && m.Definition.ObjectKind == NpcObjectKind.Monster)
                              ?? [];
        foreach (var monster in monstersInRange)
        {
            await monster.AttackByAsync(this._player, skillEntry, isCombo).ConfigureAwait(false);
        }
    }

    private async ValueTask PerformTargetedSkillAttackAsync(IAttackable target, Skill skill)
    {
        var strategy =
            this._player.GameContext.PlugInManager.GetStrategy<short, ITargetedSkillPlugin>((short)skill.Number)
            ?? new TargetedSkillDefaultPlugin();
        await strategy.PerformSkillAsync(this._player, target, (ushort)skill.Number).ConfigureAwait(false);
    }

    private void RefreshTarget()
    {
        if (this._currentTarget is { } t
            && (!t.IsAlive || t.IsAtSafezone() || t.IsTeleporting
                || !t.IsInRange(this._originalPosition, this.GetHuntingRange())))
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
        return map.GetAttackablesInRange(this._originalPosition, range)
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

        return map.GetAttackablesInRange(this._originalPosition, this.GetHuntingRange())
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
            this._config.Skill1UseCondition,
            this._config.Skill1SubCondition);
        if (s1 is not null)
        {
            return s1;
        }

        var s2 = this.EvaluateConditionalSkill(
            this._config.ActivationSkill2Id,
            this._config.Skill2UseTimer, this._config.DelayMinSkill2,
            this._config.Skill2UseCondition,
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

    /// <summary>
    /// Evaluates if a skill should be used based on its configured conditions (Monsters count).
    /// </summary>
    private SkillEntry? EvaluateConditionalSkill(
        int skillId,
        bool useTimer, int timerInterval,
        bool useCond,
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
                0 => 2, // At least 2 monsters
                1 => 3, // At least 3 monsters
                2 => 4, // At least 4 monsters
                3 => 5, // At least 5 monsters
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
        if (this._currentTarget is null)
        {
            this._currentComboStep = 0;
            return;
        }

        var ids = this.GetConfiguredComboSkillIds();
        if (ids.Count == 0)
        {
            await this.PerformAttackAsync(this._currentTarget, this.GetAnyOffensiveSkill(), false).ConfigureAwait(false);
            return;
        }

        var skillId = (ushort)ids[this._currentComboStep % ids.Count];
        var skillEntry = this._player.SkillList?.GetSkill(skillId);

        // If the current combo skill is out of range, we reset the rotation to Basic skill.
        if (skillEntry?.Skill is { } currentSkill && !this._currentTarget.IsInRange(this._player.Position, (byte)currentSkill.Range + 1))
        {
            this._currentComboStep = 0;
            return;
        }

        bool isAreaSkill = skillEntry?.Skill?.SkillType is SkillType.AreaSkillAutomaticHits or SkillType.AreaSkillExplicitTarget or SkillType.AreaSkillExplicitHits;
        var comboState = this._player.ComboState;
        var stateBefore = comboState?.CurrentState;

        if (isAreaSkill)
        {
            bool isActuallyCombo = false;
            if (skillEntry?.Skill is { } skill && comboState is not null)
            {
                isActuallyCombo = await comboState.RegisterSkillAsync(skill).ConfigureAwait(false);
            }

            await this.PerformAttackAsync(this._currentTarget, skillEntry, isActuallyCombo).ConfigureAwait(false);

            if (isActuallyCombo)
            {
                // Combo finisher cooldown (3 ticks = 1.5s) to allow animation to finish.
                this._skillCooldownTicks = ComboFinisherDelayTicks;
                this._currentComboStep = 0;
            }
            else
            {
                // Mini-delay (1 tick = 500ms) between skills to help client processing.
                this._skillCooldownTicks = InterSkillDelayTicks;
                this._currentComboStep++;
            }
        }
        else
        {
            // Targeted skill handles its own registration in the plugin.
            await this.PerformAttackAsync(this._currentTarget, skillEntry, false).ConfigureAwait(false);

            var stateAfter = comboState?.CurrentState;

            // Detection: If it wasn't initial before and is initial now, it either finished or reset.
            if (stateBefore != comboState?.InitialState && stateAfter == comboState?.InitialState)
            {
                // Combo finished or reset.
                this._skillCooldownTicks = ComboFinisherDelayTicks;
                this._currentComboStep = 0;
            }
            else
            {
                // Mini-delay (1 tick = 500ms) between skills to help client processing.
                this._skillCooldownTicks = InterSkillDelayTicks;
                this._currentComboStep++;
            }
        }
    }

    private List<int> GetConfiguredComboSkillIds()
    {
        var ids = new List<int>();
        if (this._config is not null)
        {
            if (this._config.BasicSkillId > 0)
            {
                ids.Add(this._config.BasicSkillId);
            }

            if (this._config.ActivationSkill1Id > 0)
            {
                ids.Add(this._config.ActivationSkill1Id);
            }

            if (this._config.ActivationSkill2Id > 0)
            {
                ids.Add(this._config.ActivationSkill2Id);
            }
        }

        return ids;
    }

    private byte GetHuntingRange()
    {
        if (this._config is null || this._config.HuntingRange == 0)
        {
            return FallbackViewRange;
        }

        return (byte)Math.Min(this._config.HuntingRange, FallbackViewRange);
    }

    private byte GetEffectiveAttackRange()
    {
        if (this._config is null)
        {
            return FallbackAttackRange;
        }

        var skillIds = this._config.UseCombo
            ? this.GetConfiguredComboSkillIds()
            : (this._config.BasicSkillId > 0 ? [this._config.BasicSkillId] : []);

        if (skillIds.Count > 0)
        {
            var ranges = skillIds
                .Select(id => this._player.SkillList?.GetSkill((ushort)id)?.Skill?.Range ?? 0)
                .Where(r => r > 0)
                .ToList();

            if (ranges.Count > 0)
            {
                // In combo mode, we use the minimum range to ensure all skills can hit.
                return (byte)ranges.Min();
            }
        }

        return this.GetOffensiveSkills()
            .Select(s => (byte)s.Skill!.Range)
            .DefaultIfEmpty(FallbackAttackRange)
            .Max();
    }

    private IEnumerable<SkillEntry> GetOffensiveSkills()
    {
        return this._player.SkillList?.Skills
            .Where(s => s.Skill is not null
                        && s.Skill.SkillType != SkillType.PassiveBoost
                        && s.Skill.SkillType != SkillType.Buff
                        && s.Skill.SkillType != SkillType.Regeneration)
               ?? [];
    }

    private SkillEntry? GetAnyOffensiveSkill() => this.GetOffensiveSkills().FirstOrDefault();

    private SkillEntry? FindSkillByType(SkillType type)
        => this._player.SkillList?.Skills.FirstOrDefault(s => s.Skill?.SkillType == type);

    private SkillEntry? FindDrainLifeSkill()
        => this._player.SkillList?.Skills.FirstOrDefault(s =>
            s.Skill is { Number: DrainLifeBaseSkillId or DrainLifeStrengthenerSkillId or DrainLifeMasterySkillId });

    private async ValueTask<bool> WalkToAsync(Point target)
    {
        if (this._player.IsWalking || this._player.CurrentMap is not { } map)
        {
            return false;
        }

        var pathFinder = await this._player.GameContext.PathFinderPool.GetAsync().ConfigureAwait(false);
        try
        {
            pathFinder.ResetPathFinder();
            var path = pathFinder.FindPath(this._player.Position, target, map.Terrain.AIgrid, false);
            if (path is null || path.Count == 0)
            {
                return false;
            }

            // Convert path result to walking steps. Walker handles max 16 steps.
            var stepsCount = Math.Min(path.Count, 16);
            var steps = new WalkingStep[stepsCount];
            for (int i = 0; i < stepsCount; i++)
            {
                var node = path[i];
                var prevPos = i == 0 ? this._player.Position : steps[i - 1].To;
                steps[i] = new WalkingStep(prevPos, node.Point, prevPos.GetDirectionTo(node.Point));
            }

            await this._player.WalkToAsync(target, steps).ConfigureAwait(false);
            return true;
        }
        finally
        {
            this._player.GameContext.PathFinderPool.Return(pathFinder);
        }
    }
}