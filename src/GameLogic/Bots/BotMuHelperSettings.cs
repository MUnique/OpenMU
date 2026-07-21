// <copyright file="BotMuHelperSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.GameLogic.MuHelper;

/// <summary>
/// Default MU Helper settings used to drive a bot's combat AI.
/// A bot never sends a client-side MU Helper configuration, so without this the player would
/// fall back to a hunting range of a single tile (see <see cref="Offline.CombatHandler"/>).
/// These defaults make the bot hunt nearby monsters, pick up the valuable drops and use
/// potions, while staying close to its spawn origin.
/// </summary>
internal sealed class BotMuHelperSettings : IMuHelperSettings
{
    /// <inheritdoc />
    public int BasicSkillId => 0;

    /// <inheritdoc />
    public int ActivationSkill1Id => 0;

    /// <inheritdoc />
    public int ActivationSkill2Id => 0;

    /// <inheritdoc />
    public int DelayMinSkill1 => 0;

    /// <inheritdoc />
    public int DelayMinSkill2 => 0;

    /// <inheritdoc />
    public bool Skill1UseTimer => false;

    /// <inheritdoc />
    public bool Skill1UseCondition => false;

    /// <inheritdoc />
    public bool Skill1ConditionAttacking => false;

    /// <inheritdoc />
    public int Skill1SubCondition => 0;

    /// <inheritdoc />
    public bool Skill2UseTimer => false;

    /// <inheritdoc />
    public bool Skill2UseCondition => false;

    /// <inheritdoc />
    public bool Skill2ConditionAttacking => false;

    /// <inheritdoc />
    public int Skill2SubCondition => 0;

    /// <inheritdoc />
    public bool UseCombo => false;

    /// <inheritdoc />
    public int HuntingRange => 6;

    /// <inheritdoc />
    public int MaxSecondsAway => 30;

    /// <inheritdoc />
    public bool LongRangeCounterAttack => false;

    /// <inheritdoc />
    /// <remarks>
    /// Disabled for bots: the <see cref="BotNavigator"/> is the sole driver of travel between hunting
    /// grounds, so the offline movement handler must not try to walk the bot back to its origin in parallel.
    /// </remarks>
    public bool ReturnToOriginalPosition => false;

    /// <inheritdoc />
    public int BuffSkill0Id => 0;

    /// <inheritdoc />
    public int BuffSkill1Id => 0;

    /// <inheritdoc />
    public int BuffSkill2Id => 0;

    /// <inheritdoc />
    public bool BuffOnDuration => false;

    /// <inheritdoc />
    public bool BuffDurationForParty => false;

    /// <inheritdoc />
    public int BuffCastIntervalSeconds => 0;

    // With the class heal skill learned (e.g. elf Heal), the HealingHandler casts it below the threshold
    // before falling back to potions - the same order a real player follows.

    /// <inheritdoc />
    public bool AutoHeal => true;

    /// <inheritdoc />
    public int HealThresholdPercent => 60;

    /// <inheritdoc />
    public bool UseDrainLife => false;

    /// <inheritdoc />
    public bool UseHealPotion => true;

    /// <inheritdoc />
    public int PotionThresholdPercent => 60;

    /// <inheritdoc />
    // Bots hunt in small parties (see BotManager.FormParties): the elf heals the group, buffs are
    // shared, and the party experience bonus applies - like a real group of players.
    public bool SupportParty => true;

    /// <inheritdoc />
    public bool AutoHealParty => true;

    /// <inheritdoc />
    public int HealPartyThresholdPercent => 60;

    /// <inheritdoc />
    public bool UseDarkRaven => false;

    /// <inheritdoc />
    public int DarkRavenMode => 0;

    /// <inheritdoc />
    public int ObtainRange => 6;

    /// <inheritdoc />
    public bool PickAllItems => false;

    /// <inheritdoc />
    // Must be true: the pickup handler bails out early unless PickAllItems or PickSelectItems is set,
    // so with this off the selective PickZen/PickJewel/PickAncient flags below never take effect.
    public bool PickSelectItems => true;

    /// <inheritdoc />
    public bool PickJewel => true;

    /// <inheritdoc />
    public bool PickZen => true;

    /// <inheritdoc />
    public bool PickAncient => true;

    /// <inheritdoc />
    public bool PickExcellent => true;

    /// <inheritdoc />
    public bool PickExtraItems => false;

    /// <inheritdoc />
    public IReadOnlyList<string> ExtraItemNames => Array.Empty<string>();

    /// <inheritdoc />
    /// <remarks>
    /// Disabled on purpose: offline auto-repair has no NPC discount and drains Zen at an
    /// increased rate. Bots should not burn their balance on repairs during the proof of concept.
    /// </remarks>
    public bool RepairItem => false;

    /// <inheritdoc />
    /// <remarks>Bots fight back when a player attacks them (see <see cref="BotSelfDefensePlugIn"/>).</remarks>
    public bool UseSelfDefense => true;

    /// <inheritdoc />
    public bool AutoAcceptFriend => false;

    /// <inheritdoc />
    public bool AutoAcceptGuild => false;

    /// <inheritdoc />
    /// <remarks>
    /// Bots accept party invitations from any player, like a friendly stranger would - within the
    /// safeguards applied by <see cref="BotPartyHandler"/> (level gap, not while busy, limited time).
    /// </remarks>
    public bool AutoAcceptAnyone => true;

    /// <inheritdoc />
    public bool FallbackBasicAttack => true;

    /// <inheritdoc />
    /// <remarks>
    /// Enabled for bots: they have no client-side skill configuration, so the combat AI auto-selects the
    /// strongest learned attack skill the character can currently afford. Combined with the level-gated
    /// skills granted at generation (see <see cref="BotGenerator"/>), this makes bots cast class- and
    /// level-appropriate magic/skills instead of only swinging their weapon.
    /// </remarks>
    public bool AutoSelectBestSkill => true;

    /// <inheritdoc />
    /// <remarks>Bots keep their class's learned buffs up automatically (e.g. elf Greater Defense/Greater Damage).</remarks>
    public bool AutoSelectBuffs => true;

    /// <inheritdoc />
    /// <remarks>Casters drink mana potions, so they keep casting instead of degrading to weak melee.</remarks>
    public bool UseManaPotion => true;

    /// <inheritdoc />
    /// <remarks>
    /// Bots only engage monsters they can handle (the navigator's safe-monster cap). Without this, a bot
    /// travelling through hostile territory picks fights with monsters far above its level and dies.
    /// </remarks>
    public bool OnlyHuntSafeMonsters => true;

    /// <inheritdoc />
    /// <remarks>Bots evaluate dropped gear and pick up upgrades for their own class (see <see cref="BotEquipmentHandler"/>).</remarks>
    public bool PickUpgradeItems => true;
}
