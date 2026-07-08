// <copyright file="MuHelperSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MuHelper;

using MUnique.OpenMU.GameLogic.MuHelper;

/// <summary>
/// Deserialized representation of the 257-byte <c>PRECEIVE_MUHELPER_DATA</c> blob.
/// </summary>
public sealed class MuHelperSettings : IMuHelperSettings
{
    /// <summary>Gets the always-active basic attack skill ID (0 = no skill, use normal attack).</summary>
    public int BasicSkillId { get; init; }

    /// <summary>Gets the first conditional skill ID.</summary>
    public int ActivationSkill1Id { get; init; }

    /// <summary>Gets the second conditional skill ID.</summary>
    public int ActivationSkill2Id { get; init; }

    /// <summary>Gets the timer interval (seconds) for ActivationSkill1 when Skill1Delay is set.</summary>
    public int DelayMinSkill1 { get; init; }

    /// <summary>Gets the timer interval (seconds) for ActivationSkill2 when Skill2Delay is set.</summary>
    public int DelayMinSkill2 { get; init; }

    /// <summary>Gets a value indicating whether to use a timer for skill 1.</summary>
    public bool Skill1UseTimer { get; init; }

    /// <summary>Gets a value indicating whether to use a condition for skill 1.</summary>
    public bool Skill1UseCondition { get; init; }

    /// <summary>Gets a value indicating whether to use the precondition for Skill1 (false = nearby, true = attacking).</summary>
    public bool Skill1ConditionAttacking { get; init; }

    /// <summary>Gets the mob count threshold for Skill1 condition: 0=2+, 1=3+, 2=4+, 3=5+.</summary>
    public int Skill1SubCondition { get; init; }

    /// <summary>Gets a value indicating whether to use a timer for skill 2.</summary>
    public bool Skill2UseTimer { get; init; }

    /// <summary>Gets a value indicating whether to use a condition for skill 2.</summary>
    public bool Skill2UseCondition { get; init; }

    /// <summary>Gets a value indicating whether to use the precondition for Skill2 (false = nearby, true = attacking).</summary>
    public bool Skill2ConditionAttacking { get; init; }

    /// <summary>Gets the mob count threshold for Skill2 condition.</summary>
    public int Skill2SubCondition { get; init; }

    /// <summary>Gets a value indicating whether to use combo mode.</summary>
    public bool UseCombo { get; init; }

    /// <summary>Gets the hunting range nibble (0-15); multiply to get tile distance.</summary>
    public int HuntingRange { get; init; }

    /// <summary>Gets the max seconds away from the original position before regrouping.</summary>
    public int MaxSecondsAway { get; init; }

    /// <summary>Gets a value indicating whether to counter-attack enemies that attack from long range.</summary>
    public bool LongRangeCounterAttack { get; init; }

    /// <summary>Gets a value indicating whether to return to the original spawn position when away too long.</summary>
    public bool ReturnToOriginalPosition { get; init; }

    /// <summary>Gets the Buff 0 skill id.</summary>
    public int BuffSkill0Id { get; init; }

    /// <summary>Gets the Buff 1 skill id.</summary>
    public int BuffSkill1Id { get; init; }

    /// <summary>Gets the Buff 2 skill id.</summary>
    public int BuffSkill2Id { get; init; }

    /// <summary>Gets a value indicating whether to apply buffs based on duration (i.e. when the buff expires).</summary>
    public bool BuffOnDuration { get; init; }

    /// <summary>Gets a value indicating whether to apply buff duration logic to party members too.</summary>
    public bool BuffDurationForParty { get; init; }

    /// <summary>Gets the buff cast interval in seconds (0 = disabled).</summary>
    public int BuffCastIntervalSeconds { get; init; }

    /// <summary>Gets a value indicating whether to use auto-heal.</summary>
    public bool AutoHeal { get; init; }

    /// <summary>Gets the self-heal threshold (% HP, e.g. 30 means heal when below 30%).</summary>
    public int HealThresholdPercent { get; init; }

    /// <summary>Gets a value indicating whether to use drain life.</summary>
    public bool UseDrainLife { get; init; }

    /// <summary>Gets a value indicating whether to use a healing potion.</summary>
    public bool UseHealPotion { get; init; }

    /// <summary>Gets the potion use threshold (% HP).</summary>
    public int PotionThresholdPercent { get; init; }

    /// <summary>Gets a value indicating whether to support a party.</summary>
    public bool SupportParty { get; init; }

    /// <summary>Gets a value indicating whether to auto heal party.</summary>
    public bool AutoHealParty { get; init; }

    /// <summary>Gets the party member HP threshold (%) below which healing is applied.</summary>
    public int HealPartyThresholdPercent { get; init; }

    /// <summary>Gets a value indicating whether to use dark raven.</summary>
    public bool UseDarkRaven { get; init; }

    /// <summary>Gets the dark raven mode 0 = cease, 1 = auto-attack, 2 = attack with an owner.</summary>
    public int DarkRavenMode { get; init; }

    /// <summary>Gets the range to obtain.</summary>
    public int ObtainRange { get; init; }

    /// <summary>Gets a value indicating whether to pick up all items.</summary>
    public bool PickAllItems { get; init; }

    /// <summary>Gets a value indicating whether to pick up selected items.</summary>
    public bool PickSelectItems { get; init; }

    /// <summary>Gets a value indicating whether to pick up jewels.</summary>
    public bool PickJewel { get; init; }

    /// <summary>Gets a value indicating whether to pick up zen.</summary>
    public bool PickZen { get; init; }

    /// <summary>Gets a value indicating whether to pick up ancient items.</summary>
    public bool PickAncient { get; init; }

    /// <summary>Gets a value indicating whether to pick up excellent items.</summary>
    public bool PickExcellent { get; init; }

    /// <summary>Gets a value indicating whether to pick up extra items.</summary>
    public bool PickExtraItems { get; init; }

    /// <summary>Gets the extra item names. Up to 12 item name substrings; pick any dropped item whose name contains one of these.</summary>
    public IReadOnlyList<string> ExtraItemNames { get; init; } = [];

    /// <summary>Gets a value indicating whether to repair items.</summary>
    public bool RepairItem { get; init; }

    /// <summary>Gets a value indicating whether to automatically defend against players attacking the character.</summary>
    public bool UseSelfDefense { get; init; }

    /// <summary>Gets a value indicating whether to automatically accept requests from friends.</summary>
    public bool AutoAcceptFriend { get; init; }

    /// <summary>Gets a value indicating whether to automatically accept requests from guild.</summary>
    public bool AutoAcceptGuild { get; init; }

    /// <summary>Gets a value indicating whether to use basic attack as fallback when the configured skill cannot be used.</summary>
    public bool FallbackBasicAttack { get; init; }

    /// <inheritdoc />
    /// <remarks>Human MU Helper sessions drive their skills through the explicit client configuration, so this is always <c>false</c>.</remarks>
    public bool AutoSelectBestSkill => false;

    /// <inheritdoc />
    /// <remarks>Human MU Helper sessions configure their buff slots explicitly, so this is always <c>false</c>.</remarks>
    public bool AutoSelectBuffs => false;

    /// <inheritdoc />
    /// <remarks>The client-side MU Helper has no mana potion setting; this only exists for server-side bots.</remarks>
    public bool UseManaPotion => false;

    /// <inheritdoc />
    /// <remarks>A human picked their hunting spot deliberately, so their offline session fights whatever is there.</remarks>
    public bool OnlyHuntSafeMonsters => false;

    /// <inheritdoc />
    /// <remarks>Equipment progression is a server-side bot behavior only.</remarks>
    public bool PickUpgradeItems => false;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[MuHelperSettings: BasicSkill={this.BasicSkillId}, Act1={this.ActivationSkill1Id}, Act2={this.ActivationSkill2Id}, Buffs={this.BuffSkill0Id}/{this.BuffSkill1Id}/{this.BuffSkill2Id}, HP={this.PotionThresholdPercent}%, Party={this.SupportParty}, PickAll={this.PickAllItems}, PickZen={this.PickZen}]";
    }
}
