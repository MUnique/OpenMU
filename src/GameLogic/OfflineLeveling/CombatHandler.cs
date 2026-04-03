// <copyright file="CombatHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Handles combat logic including target selection, attacks, combo attacks, and skill usage.
/// </summary>
public sealed class CombatHandler
{
    private const byte DefaultRange = 1;
    private const int ComboFinisherDelayTicks = 3;
    private const int InterSkillDelayTicks = 1;

    private const short DrainLifeBaseSkillId = 214;
    private const short DrainLifeStrengthenerSkillId = 458;
    private const short DrainLifeMasterySkillId = 462;

    private readonly OfflineLevelingPlayer _player;
    private readonly IMuHelperSettings? _config;
    private readonly MovementHandler _movementHandler;
    private readonly Point _originPosition;

    private IAttackable? _currentTarget;
    private int _nearbyMonsterCount;
    private int _currentComboStep;
    private int _skillCooldownTicks;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatHandler"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    /// <param name="config">The MU helper settings.</param>
    /// <param name="movementHandler">The movement handler.</param>
    /// <param name="originPosition">The original position to hunt around.</param>
    public CombatHandler(OfflineLevelingPlayer player, IMuHelperSettings? config, MovementHandler movementHandler, Point originPosition)
    {
        this._player = player;
        this._config = config;
        this._movementHandler = movementHandler;
        this._originPosition = originPosition;
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
            var skill = this.SelectAttackSkill();
            await this.ExecuteAttackAsync(this._currentTarget, skill, false).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Executes an attack on the specified target.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="skillEntry">The skill entry.</param>
    /// <param name="isCombo">If set to <c>true</c>, it's a combo attack.</param>
    public async ValueTask ExecuteAttackAsync(IAttackable target, SkillEntry? skillEntry, bool isCombo)
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

        this._currentTarget ??= this.FindNearestMonster();
        this._nearbyMonsterCount = this.CountMonstersNearby();
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
        var strategy = this._player.GameContext.PlugInManager.GetStrategy<short, ITargetedSkillPlugin>((short)skill.Number)
            ?? new TargetedSkillDefaultPlugin();
        await strategy.PerformSkillAsync(this._player, target, (ushort)skill.Number).ConfigureAwait(false);
    }

    private IAttackable? FindNearestMonster()
    {
        if (this._player.CurrentMap is not { } map)
        {
            return null;
        }

        return map.GetAttackablesInRange(this._originPosition, this.HuntingRange)
            .OfType<Monster>()
            .Where(this.IsMonsterAttackable)
            .MinBy(m => m.GetDistanceTo(this._player));
    }

    private int CountMonstersNearby()
    {
        if (this._player.CurrentMap is not { } map)
        {
            return 0;
        }

        return map.GetAttackablesInRange(this._originPosition, this.HuntingRange)
            .OfType<Monster>()
            .Count(this.IsMonsterAttackable);
    }

    private SkillEntry? SelectAttackSkill()
    {
        if (this._config is null)
        {
            return this.GetAnyOffensiveSkill();
        }

        var s1 = this.EvaluateConditionalSkill(
            this._config.ActivationSkill1Id,
            this._config.Skill1UseTimer,
            this._config.DelayMinSkill1,
            this._config.Skill1UseCondition,
            this._config.Skill1SubCondition);
        if (s1 is not null)
        {
            return s1;
        }

        var s2 = this.EvaluateConditionalSkill(
            this._config.ActivationSkill2Id,
            this._config.Skill2UseTimer,
            this._config.DelayMinSkill2,
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

    private SkillEntry? EvaluateConditionalSkill(int skillId, bool useTimer, int timerInterval, bool useCond, int subCond)
    {
        if (skillId <= 0)
        {
            return null;
        }

        if (useTimer && timerInterval > 0)
        {
            var secondsElapsed = (int)(DateTime.UtcNow - this._player.StartTimestamp).TotalSeconds;
            if (secondsElapsed > 0 && secondsElapsed % timerInterval == 0)
            {
                return this._player.SkillList?.GetSkill((ushort)skillId);
            }
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
        if (this._currentTarget is null)
        {
            this._currentComboStep = 0;
            return;
        }

        var ids = this.GetConfiguredComboSkillIds();
        if (ids.Count == 0)
        {
            await this.ExecuteAttackAsync(this._currentTarget, this.GetAnyOffensiveSkill(), false).ConfigureAwait(false);
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

        return this.GetOffensiveSkills()
            .Select(s => (byte)s.Skill!.Range)
            .DefaultIfEmpty(DefaultRange)
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

    private SkillEntry? GetAnyOffensiveSkill()
    {
        return this.GetOffensiveSkills().FirstOrDefault();
    }

    private SkillEntry? FindDrainLifeSkill()
    {
        return this._player.SkillList?.Skills.FirstOrDefault(s =>
            s.Skill is { Number: DrainLifeBaseSkillId or DrainLifeStrengthenerSkillId or DrainLifeMasterySkillId });
    }
}
