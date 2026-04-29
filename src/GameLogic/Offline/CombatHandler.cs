// <copyright file="CombatHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Handles combat logic including target selection, attacks, combo attacks, and skill usage.
/// </summary>
public sealed class CombatHandler
{
    private const byte DefaultRange = 1;
    private const int ComboFinisherDelayTicks = 3;
    private const int InterSkillDelayTicks = 1;
    private const int MinComboSkillCount = 3;

    private static readonly TargetedSkillDefaultPlugin DefaultPlugin = new();

    private const short DrainLifeBaseSkillId = 214;
    private const short DrainLifeStrengthenerSkillId = 458;
    private const short DrainLifeMasterySkillId = 462;

    private readonly OfflinePlayer _player;
    private readonly IMuHelperSettings? _config;
    private readonly MovementHandler _movementHandler;
    private readonly BuffHandler _buffHandler;
    private readonly Point _originPosition;
    private readonly ConditionalSkillSlot[] _conditionalSkillSlots;

    private IAttackable? _currentTarget;
    private int _nearbyMonsterCount;
    private int _currentComboStep;
    private int _skillCooldownTicks;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatHandler"/> class.
    /// </summary>
    /// <param name="player">The offline player.</param>
    /// <param name="config">The MU helper settings.</param>
    /// <param name="movementHandler">The movement handler.</param>
    /// <param name="buffHandler">The buff handler.</param>
    /// <param name="originPosition">The original position to hunt around.</param>
    public CombatHandler(OfflinePlayer player, IMuHelperSettings? config, MovementHandler movementHandler, BuffHandler buffHandler, Point originPosition)
    {
        this._player = player;
        this._config = config;
        this._movementHandler = movementHandler;
        this._buffHandler = buffHandler;
        this._originPosition = originPosition;
        this._conditionalSkillSlots = config is null ? [] :
        [
            new ConditionalSkillSlot(config.ActivationSkill1Id, config.Skill1UseTimer, config.DelayMinSkill1, config.Skill1UseCondition, config.Skill1ConditionAttacking, config.Skill1SubCondition),
            new ConditionalSkillSlot(config.ActivationSkill2Id, config.Skill2UseTimer, config.DelayMinSkill2, config.Skill2UseCondition, config.Skill2ConditionAttacking, config.Skill2SubCondition),
        ];
    }

    /// <summary>
    /// Gets the remaining skill cooldown ticks.
    /// </summary>
    public int SkillCooldownTicks => this._skillCooldownTicks;

    /// <summary>
    /// Gets the hunting range in tiles.
    /// </summary>
    public byte HuntingRange => CalculateHuntingRange(this._config);

    /// <summary>
    /// Calculates the hunting range in tiles from the specified configuration.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>The hunting range.</returns>
    public static byte CalculateHuntingRange(IMuHelperSettings? config)
    {
        if (config is null)
        {
            return DefaultRange;
        }

        return (byte)Math.Max(DefaultRange, config.HuntingRange);
    }

    /// <summary>
    /// Decrements the skill cooldown counter by one tick.
    /// </summary>
    public void DecrementCooldown()
    {
        if (this._skillCooldownTicks > 0)
        {
            this._skillCooldownTicks--;
        }
    }

    /// <summary>
    /// Performs combat attacks on targets.
    /// </summary>
    /// <returns>A value task representing the asynchronous operation.</returns>
    public async ValueTask PerformAttackAsync()
    {
        this.RefreshTarget();

        if (this._currentTarget is null)
        {
            return;
        }

        byte attackRange = this.GetEffectiveAttackRange();
        if (!this.IsTargetInAttackRange(this._currentTarget, attackRange))
        {
            await this._movementHandler.MoveCloserToTargetAsync(this._currentTarget, attackRange).ConfigureAwait(false);
            return;
        }

        if (this._config?.UseCombo == true)
        {
            await this.ExecuteComboAttackAsync().ConfigureAwait(false);
        }
        else
        {
            await this.ExecuteAttackAsync(this._currentTarget).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Performs health recovery through Drain Life attacks if configured.
    /// </summary>
    public async ValueTask PerformDrainLifeRecoveryAsync()
    {
        if (this._config is null || this._player.Attributes is null || !this._config.UseDrainLife)
        {
            return;
        }

        double maxHp = this._player.Attributes[Stats.MaximumHealth];
        if (maxHp <= 0)
        {
            return;
        }

        double hp = this._player.Attributes[Stats.CurrentHealth];
        int hpPercent = (int)(hp * 100.0 / maxHp);
        if (hpPercent <= this._config.HealThresholdPercent)
        {
            var drainSkill = this.FindDrainLifeSkill();
            if (drainSkill is not null)
            {
                this.RefreshTarget();
                if (this._currentTarget is not null)
                {
                    await this.ExecuteAttackAsync(this._currentTarget, drainSkill, false).ConfigureAwait(false);
                }
            }
        }
    }

    private async ValueTask ExecuteAttackAsync(IAttackable target)
    {
        var skill = this.SelectAttackSkill();
        if (skill == null)
        {
            // Do not attack if there are buffs configured to handle buff-only classes.
            var buffs = this._buffHandler.ConfiguredBuffIds;
            if (buffs.Any(id => id > 0))
            {
                return;
            }
        }

        await this.ExecuteAttackAsync(target, skill, false).ConfigureAwait(false);
    }

    private async ValueTask ExecuteAttackAsync(IAttackable target, SkillEntry? skillEntry, bool isCombo)
    {
        this._player.Rotation = this._player.GetDirectionTo(target);

        if (skillEntry?.Skill is not { } skill)
        {
            await this.ExecutePhysicalAttackAsync(target).ConfigureAwait(false);
            return;
        }

        if (skill.SkillType is SkillType.AreaSkillAutomaticHits or SkillType.AreaSkillExplicitTarget or SkillType.AreaSkillExplicitHits)
        {
            await this.ExecuteAreaSkillAttackAsync(target, skillEntry, isCombo).ConfigureAwait(false);
        }
        else
        {
            await this.ExecuteTargetedSkillAttackAsync(target, skill).ConfigureAwait(false);
        }
    }

    private void RefreshTarget()
    {
        if (this._currentTarget is { } t && !this.IsTargetStillValid(t))
        {
            this._currentTarget = null;
        }

        if (this._currentTarget is null)
        {
            var monsters = this.GetAttackableMonstersInHuntingRange().ToList();
            this._currentTarget = monsters.MinBy(m => m.GetDistanceTo(this._player));
            this._nearbyMonsterCount = monsters.Count;
        }
        else
        {
            this._nearbyMonsterCount = this.GetAttackableMonstersInHuntingRange().Count();
        }
    }

    private IEnumerable<Monster> GetAttackableMonstersInHuntingRange()
    {
        if (this._player.CurrentMap is not { } map)
        {
            return [];
        }

        return map.GetAttackablesInRange(this._originPosition, this.HuntingRange)
            .OfType<Monster>()
            .Where(this.IsMonsterAttackable);
    }

    private bool IsTargetInAttackRange(IAttackable target, byte range)
    {
        return target.IsInRange(this._player.Position, range);
    }

    private bool IsTargetStillValid(IAttackable target)
    {
        return target.IsAlive
               && !target.IsAtSafezone()
               && !target.IsTeleporting
               && target.IsInRange(this._originPosition, this.HuntingRange);
    }

    private bool IsMonsterAttackable(Monster monster)
    {
        return monster.IsAlive
               && !monster.IsAtSafezone()
               && monster.Definition.ObjectKind == NpcObjectKind.Monster;
    }

    private async ValueTask ExecutePhysicalAttackAsync(IAttackable target)
    {
        await target.AttackByAsync(this._player, null, false).ConfigureAwait(false);

        await this._player.ForEachWorldObserverAsync<IShowAnimationPlugIn>(
            p => p.ShowAnimationAsync(this._player, 120, target, this._player.Rotation),
            includeThis: true).ConfigureAwait(false);
    }

    private async ValueTask ExecuteAreaSkillAttackAsync(IAttackable target, SkillEntry skillEntry, bool isCombo)
    {
        var skill = skillEntry.Skill!;

        if (isCombo)
        {
            await this._player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(
                p => p.ShowComboAnimationAsync(this._player, target),
                includeThis: true).ConfigureAwait(false);
        }

        var rotationByte = (byte)(this._player.Position.GetAngleDegreeTo(target.Position) / 360.0 * 255.0);
        await this._player.ForEachWorldObserverAsync<IShowAreaSkillAnimationPlugIn>(
            p => p.ShowAreaSkillAnimationAsync(this._player, skill, target.Position, rotationByte),
            includeThis: true).ConfigureAwait(false);

        var monstersInRange = this._player.CurrentMap?
            .GetAttackablesInRange(target.Position, skill.Range)
            .OfType<Monster>()
            .Where(this.IsMonsterAttackable)
            ?? [];

        foreach (var monster in monstersInRange)
        {
            await monster.AttackByAsync(this._player, skillEntry, isCombo).ConfigureAwait(false);
        }
    }

    private async ValueTask ExecuteTargetedSkillAttackAsync(IAttackable target, Skill skill)
    {
        var strategy = this._player.GameContext.PlugInManager.GetStrategy<short, ITargetedSkillPlugin>(skill.Number)
            ?? DefaultPlugin;
        await strategy.PerformSkillAsync(this._player, target, (ushort)skill.Number).ConfigureAwait(false);
    }

    private SkillEntry? SelectAttackSkill()
    {
        if (this._config is null)
        {
            return null;
        }

        // If no skills are configured at all, don't attack.
        if (this._config.BasicSkillId == 0
            && this._config.ActivationSkill1Id == 0
            && this._config.ActivationSkill2Id == 0)
        {
            return null;
        }

        foreach (var slot in this._conditionalSkillSlots)
        {
            var skill = this.EvaluateConditionalSkill(slot);
            if (skill is not null && this.HasEnoughResources(skill))
            {
                return skill;
            }
        }

        if (this._config.BasicSkillId > 0)
        {
            var basicSkill = this._player.SkillList?.GetSkill((ushort)this._config.BasicSkillId);
            if (basicSkill is not null && this.HasEnoughResources(basicSkill))
            {
                return basicSkill;
            }
        }

        return null;
    }

    /// <summary>
    /// Evaluates whether the skill in the given slot should fire this tick.
    /// </summary>
    private SkillEntry? EvaluateConditionalSkill(ConditionalSkillSlot slot)
    {
        if (slot.SkillId <= 0)
        {
            return null;
        }

        if (slot.UseTimer && !slot.UseCondition)
        {
            if (slot.TimerIntervalSeconds <= 0)
            {
                return null;
            }

            var secondsSinceLastUse = (DateTime.UtcNow - slot.LastUseTime).TotalSeconds;
            if (secondsSinceLastUse >= slot.TimerIntervalSeconds)
            {
                slot.LastUseTime = DateTime.UtcNow;
                return this._player.SkillList?.GetSkill((ushort)slot.SkillId);
            }

            return null;
        }

        if (slot.UseCondition && !slot.UseTimer)
        {
            int threshold = slot.SubCondition switch
            {
                0 => 2,
                1 => 3,
                2 => 4,
                3 => 5,
                _ => int.MaxValue,
            };

            int monsterCount = slot.ConditionAttacking
                ? this.CountMonstersAttackingPlayer()
                : this._nearbyMonsterCount;

            if (monsterCount >= threshold)
            {
                return this._player.SkillList?.GetSkill((ushort)slot.SkillId);
            }
        }

        return null;
    }

    private int CountMonstersAttackingPlayer()
    {
        return this.GetAttackableMonstersInHuntingRange()
            .Count(m => m.IsTargetingPlayer(this._player));
    }

    /// <summary>
    /// Checks that the player has enough mana AND ability (AG) to cast the skill.
    /// Both resources are consumed by skills via <see cref="Skill.ConsumeRequirements"/>.
    /// </summary>
    private bool HasEnoughResources(SkillEntry? skillEntry)
    {
        if (skillEntry?.Skill is not { } skill || this._player.Attributes is null)
        {
            return true;
        }

        foreach (var requirement in skill.ConsumeRequirements)
        {
            int required = this._player.GetRequiredValue(requirement);
            if (this._player.Attributes[requirement.Attribute] < required)
            {
                return false;
            }
        }

        return true;
    }

    private async ValueTask ExecuteComboAttackAsync()
    {
        if (this._currentTarget is null)
        {
            this._currentComboStep = 0;
            return;
        }

        var ids = this.GetConfiguredComboSkillIds();
        if (ids.Count < MinComboSkillCount)
        {
            await this.ExecuteAttackAsync(this._currentTarget).ConfigureAwait(false);
            return;
        }

        var skillId = (ushort)ids[this._currentComboStep % ids.Count];
        var skillEntry = this._player.SkillList?.GetSkill(skillId);

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

            await this.ExecuteAttackAsync(this._currentTarget, skillEntry, isActuallyCombo).ConfigureAwait(false);

            if (isActuallyCombo)
            {
                this._skillCooldownTicks = ComboFinisherDelayTicks;
                this._currentComboStep = 0;
            }
            else
            {
                this._skillCooldownTicks = InterSkillDelayTicks;
                this._currentComboStep++;
            }
        }
        else
        {
            await this.ExecuteAttackAsync(this._currentTarget, skillEntry, false).ConfigureAwait(false);

            var stateAfter = comboState?.CurrentState;
            if (stateBefore != comboState?.InitialState && stateAfter == comboState?.InitialState)
            {
                this._skillCooldownTicks = ComboFinisherDelayTicks;
                this._currentComboStep = 0;
            }
            else
            {
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

    private byte GetEffectiveAttackRange()
    {
        if (this._config is null)
        {
            return DefaultRange;
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
                return (byte)ranges.Min();
            }
        }

        return DefaultRange;
    }

    private SkillEntry? FindDrainLifeSkill()
    {
        return this._player.SkillList?.Skills.FirstOrDefault(s =>
            s.Skill is { Number: DrainLifeBaseSkillId or DrainLifeStrengthenerSkillId or DrainLifeMasterySkillId });
    }

    /// <summary>
    /// Holds the configuration for one conditional skill slot, along with its mutable last-use timestamp.
    /// </summary>
    private sealed class ConditionalSkillSlot(int skillId, bool useTimer, int timerIntervalSeconds, bool useCondition, bool conditionAttacking, int subCondition)
    {
        public int SkillId { get; } = skillId;
        public bool UseTimer { get; } = useTimer;
        public int TimerIntervalSeconds { get; } = timerIntervalSeconds;
        public bool UseCondition { get; } = useCondition;
        public bool ConditionAttacking { get; } = conditionAttacking;
        public int SubCondition { get; } = subCondition;
        public DateTime LastUseTime { get; set; }
    }
}