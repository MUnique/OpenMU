// <copyright file="IMuHelperSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MuHelper;

/// <summary>
/// The Mu Helper player settings.
/// </summary>
public interface IMuHelperSettings
{
    /// <summary>Gets the always-active basic attack skill ID (0 = no skill, use normal attack).</summary>
    int BasicSkillId { get; }

    /// <summary>Gets the first conditional skill ID.</summary>
    int ActivationSkill1Id { get; }

    /// <summary>Gets the second conditional skill ID.</summary>
    int ActivationSkill2Id { get; }

    /// <summary>Gets the timer interval (seconds) for ActivationSkill1 when Skill1Delay is set.</summary>
    int DelayMinSkill1 { get; }

    /// <summary>Gets the timer interval (seconds) for ActivationSkill2 when Skill2Delay is set.</summary>
    int DelayMinSkill2 { get; }

    /// <summary>Gets a value indicating whether to use timer for the skill 1.</summary>
    bool Skill1UseTimer { get; }

    /// <summary>Gets a value indicating whether to use condition for the skill 1.</summary>
    bool Skill1UseCondition { get; }

    /// <summary>Gets a value indicating whether to use the precondition for Skill1 (false = nearby, true = attacking).</summary>
    bool Skill1ConditionAttacking { get; }

    /// <summary>Gets the mob count threshold for Skill1 condition: 0=2+, 1=3+, 2=4+, 3=5+.</summary>
    int Skill1SubCondition { get; }

    /// <summary>Gets a value indicating whether to use timer for the skill 2.</summary>
    bool Skill2UseTimer { get; }

    /// <summary>Gets a value indicating whether to use condition for the skill 2.</summary>
    bool Skill2UseCondition { get; }

    /// <summary>Gets a value indicating whether to use the precondition for Skill2 (false = nearby, true = attacking).</summary>
    bool Skill2ConditionAttacking { get; }

    /// <summary>Gets the mob count threshold for Skill2 condition.</summary>
    int Skill2SubCondition { get; }

    /// <summary>Gets a value indicating whether to use combo mode.</summary>
    bool UseCombo { get; }

    /// <summary>Gets the hunting range nibble (0-15); multiply to get tile distance.</summary>
    int HuntingRange { get; }

    /// <summary>Gets the max seconds away from original position before regrouping.</summary>
    int MaxSecondsAway { get; }

    /// <summary>Gets a value indicating whether to counter-attack enemies that attack from long range.</summary>
    bool LongRangeCounterAttack { get; }

    /// <summary>Gets a value indicating whether to return to original spawn position when away too long.</summary>
    bool ReturnToOriginalPosition { get; }

    /// <summary>Gets the Buff 0 skill id.</summary>
    int BuffSkill0Id { get; }

    /// <summary>Gets the Buff 1 skill id.</summary>
    int BuffSkill1Id { get; }

    /// <summary>Gets the Buff 2 skill id.</summary>
    int BuffSkill2Id { get; }

    /// <summary>Gets a value indicating whether apply buffs based on duration (i.e. when the buff expires).</summary>
    bool BuffOnDuration { get; }

    /// <summary>Gets a value indicating whether apply buff duration logic to party members too.</summary>
    bool BuffDurationForParty { get; }

    /// <summary>Gets the buff cast interval in seconds (0 = disabled).</summary>
    int BuffCastIntervalSeconds { get; }

    /// <summary>Gets a value indicating whether to use auto-heal.</summary>
    bool AutoHeal { get; }

    /// <summary>Gets the self-heal threshold (% HP, e.g. 30 means heal when below 30%).</summary>
    int HealThresholdPercent { get; }

    /// <summary>Gets a value indicating whether to use drain life.</summary>
    bool UseDrainLife { get; }

    /// <summary>Gets a value indicating whether to use healing potion.</summary>
    bool UseHealPotion { get; }

    /// <summary>Gets the potion use threshold (% HP).</summary>
    int PotionThresholdPercent { get; }

    /// <summary>Gets a value indicating whether to support party.</summary>
    bool SupportParty { get; }

    /// <summary>Gets a value indicating whether to auto heal party.</summary>
    bool AutoHealParty { get; }

    /// <summary>Gets the party member HP threshold (%) below which healing is applied.</summary>
    int HealPartyThresholdPercent { get; }

    /// <summary>Gets a value indicating whether to use dark raven.</summary>
    bool UseDarkRaven { get; }

    /// <summary>Gets the dark raven mode 0 = cease, 1 = auto-attack, 2 = attack with owner.</summary>
    int DarkRavenMode { get; }

    /// <summary>Gets the obtain range.</summary>
    int ObtainRange { get; }

    /// <summary>Gets a value indicating whether pickup all items.</summary>
    bool PickAllItems { get; }

    /// <summary>Gets a value indicating whether pickup selected items.</summary>
    bool PickSelectItems { get; }

    /// <summary>Gets a value indicating whether pickup jewels.</summary>
    bool PickJewel { get; }

    /// <summary>Gets a value indicating whether pickup zen.</summary>
    bool PickZen { get; }

    /// <summary>Gets a value indicating whether pickup ancient items.</summary>
    bool PickAncient { get; }

    /// <summary>Gets a value indicating whether pickup excellent items.</summary>
    bool PickExcellent { get; }

    /// <summary>Gets a value indicating whether pickup extra items.</summary>
    bool PickExtraItems { get; }

    /// <summary>Gets the extra item names. Up to 12 item name substrings; pick any dropped item whose name contains one of these.</summary>
    IReadOnlyList<string> ExtraItemNames { get; }

    /// <summary>Gets a value indicating whether to repair items.</summary>
    bool RepairItem { get; }
}
