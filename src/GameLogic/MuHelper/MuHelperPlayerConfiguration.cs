// <copyright file="MuHelperPlayerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MuHelper;

/// <summary>
/// Deserialized representation of the 257-byte <c>PRECEIVE_MUHELPER_DATA</c> blob that
/// the client stores in <see cref="DataModel.Entities.Character.MuHelperConfiguration"/>.
///
/// Byte layout (all little-endian, #pragma pack(1)):
/// <code>
///  0      DataStartMarker
///  1      [unused:3][JewelOrGem:1][SetItem:1][ExcellentItem:1][Zen:1][AddExtraItem:1]
///  2      [HuntingRange:4][ObtainRange:4]
///  3-4    DistanceMin   (WORD – maxSecondsAway encoded as seconds)
///  5-6    BasicSkill1   (WORD – always-on attack skill ID, 0 = none)
///  7-8    ActivationSkill1
///  9-10   DelayMinSkill1 (WORD – timer interval in seconds for ActivationSkill1)
///  11-12  ActivationSkill2
///  13-14  DelayMinSkill2
///  15-16  CastingBuffMin (WORD – buff recast interval in seconds)
///  17-18  BuffSkill0NumberID
///  19-20  BuffSkill1NumberID
///  21-22  BuffSkill2NumberID
///  23     [HPStatusAutoPotion:4][HPStatusAutoHeal:4]   (value * 10 = threshold %)
///  24     [HPStatusOfPartyMembers:4][HPStatusDrainLife:4]
///  25     [AutoPotion:1][AutoHeal:1][DrainLife:1][LongDistanceAttack:1]
///         [OriginalPosition:1][Combo:1][Party:1][PreferenceOfPartyHeal:1]
///  26     [BuffDurationForAllPartyMembers:1][UseDarkSpirits:1][BuffDuration:1]
///         [Skill1Delay:1][Skill1Con:1][Skill1PreCon:1][Skill1SubCon:2]
///  27     [Skill2Delay:1][Skill2Con:1][Skill2PreCon:1][Skill2SubCon:2]
///         [RepairItem:1][PickAllNearItems:1][PickSelectedItems:1]
///  28     PetAttack (BYTE: 0=cease, 1=auto, 2=together)
///  29-64  _UnusedPadding[36]
///  65-244 ExtraItems[12][15]  (null-terminated ASCII item name filters)
///  245-256 (trailing padding, ignored)
/// </code>
/// </summary>
public sealed class MuHelperPlayerConfiguration
{
    // Minimum blob length to safely read all structured fields
    private const int MinBlobLength = 69;

    // Byte offsets
    private const int PickupFlagsOffset = 1;
    private const int RangeFlagsOffset = 2;
    private const int DistanceMinOffset = 3;
    private const int BasicSkillOffset = 5;
    private const int ActivationSkill1Offset = 7;
    private const int DelayMinSkill1Offset = 9;
    private const int ActivationSkill2Offset = 11;
    private const int DelayMinSkill2Offset = 13;
    private const int CastingBuffMinOffset = 15;
    private const int BuffSkill0Offset = 17;
    private const int BuffSkill1Offset = 19;
    private const int BuffSkill2Offset = 21;
    private const int HpThresholdFlagsOffset = 23;
    private const int PartyDrainThresholdOffset = 24;
    private const int BehaviorFlagsOffset = 25;
    private const int Skill1FlagsOffset = 26;
    private const int Skill2FlagsOffset = 27;
    private const int PetAttackOffset = 28;
    private const int ExtraItemsOffset = 65;
    private const int ExtraItemsEndOffset = 245;
    private const int ExtraItemSlotCount = 12;
    private const int ExtraItemSlotLength = 15;

    // Byte 1 – pickup flags
    private const int PickJewelFlag = 1 << 3;
    private const int PickSetItemFlag = 1 << 4;
    private const int PickExcellentFlag = 1 << 5;
    private const int PickZenFlag = 1 << 6;
    private const int PickExtraItemFlag = 1 << 7;

    // Byte 2 – range nibbles
    private const int HuntingRangeMask = 0x0F;
    private const int ObtainRangeShift = 4;
    private const int ObtainRangeMask = 0x0F;

    // Bytes 23/24 – HP threshold nibbles (value * 10 = %)
    private const int HpPotionNibbleMask = 0x0F;
    private const int HpHealNibbleShift = 4;
    private const int HpHealNibbleMask = 0x0F;
    private const int HpPartyNibbleMask = 0x0F;
    private const int HpThresholdMultiplier = 10;

    // Byte 25 – behaviour flags
    private const int AutoPotionFlag = 1 << 0;
    private const int AutoHealFlag = 1 << 1;
    private const int DrainLifeFlag = 1 << 2;
    private const int LongRangeCounterAttackFlag = 1 << 3;
    private const int ReturnToOriginalFlag = 1 << 4;
    private const int ComboFlag = 1 << 5;
    private const int PartyFlag = 1 << 6;
    private const int PreferPartyHealFlag = 1 << 7;

    // Byte 26 – buff / skill-1 flags
    private const int BuffDurationPartyFlag = 1 << 0;
    private const int UseDarkSpiritsFlag = 1 << 1;
    private const int BuffDurationFlag = 1 << 2;
    private const int Skill1DelayFlag = 1 << 3;
    private const int Skill1ConditionFlag = 1 << 4;
    private const int Skill1PreConditionFlag = 1 << 5;
    private const int Skill1SubConditionShift = 6;
    private const int Skill1SubConditionMask = 0x03;

    // Byte 27 – skill-2 / item flags
    private const int Skill2DelayFlag = 1 << 0;
    private const int Skill2ConditionFlag = 1 << 1;
    private const int Skill2PreConditionFlag = 1 << 2;
    private const int Skill2SubConditionShift = 3;
    private const int Skill2SubConditionMask = 0x03;
    private const int RepairItemFlag = 1 << 5;
    private const int PickAllItemsFlag = 1 << 6;
    private const int PickSelectItemsFlag = 1 << 7;

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

    /// <summary>Gets a value indicating whether to use timer for the skill 1.</summary>
    public bool Skill1UseTimer { get; init; }

    /// <summary>Gets a value indicating whether to use condition for the skill 1.</summary>
    public bool Skill1UseCondition { get; init; }

    /// <summary>Gets a value indicating whether to use the precondition for Skill1 (false = nearby, true = attacking).</summary>
    public bool Skill1ConditionAttacking { get; init; }

    /// <summary>
    /// Gets the mob count threshold for Skill1 condition: 0=2+, 1=3+, 2=4+, 3=5+.
    /// </summary>
    public int Skill1SubCondition { get; init; }

    /// <summary>Gets a value indicating whether to use timer for the skill 2.</summary>
    public bool Skill2UseTimer { get; init; }

    /// <summary>Gets a value indicating whether to use condition for the skill 2.</summary>
    public bool Skill2UseCondition { get; init; }

    /// <summary>Gets a value indicating whether to use the precondition for Skill2 (false = nearby, true = attacking).</summary>
    public bool Skill2ConditionAttacking { get; init; }

    /// <summary>Gets the mob count threshold for Skill2 condition.</summary>
    public int Skill2SubCondition { get; init; }

    /// <summary>Gets a value indicating whether to use combo mode.</summary>
    public bool UseCombo { get; init; }

    /// <summary>Gets the hunting range nibble (0-15); multiply to get tile distance.</summary>
    public int HuntingRange { get; init; }

    /// <summary>Gets the max seconds away from original position before regrouping.</summary>
    public int MaxSecondsAway { get; init; }

    /// <summary>Gets a value indicating whether to counter-attack enemies that attack from long range.</summary>
    public bool LongRangeCounterAttack { get; init; }

    /// <summary>Gets a value indicating whether to return to original spawn position when away too long.</summary>
    public bool ReturnToOriginalPosition { get; init; }

    /// <summary>Gets the Buff 0 skill id.</summary>
    public int BuffSkill0Id { get; init; }

    /// <summary>Gets the Buff 1 skill id.</summary>
    public int BuffSkill1Id { get; init; }

    /// <summary>Gets the Buff 2 skill id.</summary>
    public int BuffSkill2Id { get; init; }

    /// <summary>Gets a value indicating whether apply buffs based on duration (i.e. when the buff expires).</summary>
    public bool BuffOnDuration { get; init; }

    /// <summary>Gets a value indicating whether apply buff duration logic to party members too.</summary>
    public bool BuffDurationForParty { get; init; }

    /// <summary>Gets the buff cast interval in seconds (0 = disabled).</summary>
    public int BuffCastIntervalSeconds { get; init; }

    /// <summary>Gets a value indicating whether to use auto-heal.</summary>
    public bool AutoHeal { get; init; }

    /// <summary>Gets the self-heal threshold (% HP, e.g. 30 means heal when below 30%).</summary>
    public int HealThresholdPercent { get; init; }

    /// <summary>Gets a value indicating whether to use drain life.</summary>
    public bool UseDrainLife { get; init; }

    /// <summary>Gets a value indicating whether to use healing potion.</summary>
    public bool UseHealPotion { get; init; }

    /// <summary>Gets the potion use threshold (% HP).</summary>
    public int PotionThresholdPercent { get; init; }

    /// <summary>Gets a value indicating whether to support party.</summary>
    public bool SupportParty { get; init; }

    /// <summary>Gets a value indicating whether to auto heal party.</summary>
    public bool AutoHealParty { get; init; }

    /// <summary>Gets the party member HP threshold (%) below which healing is applied.</summary>
    public int HealPartyThresholdPercent { get; init; }

    /// <summary>Gets a value indicating whether to use dark raven.</summary>
    public bool UseDarkRaven { get; init; }

    /// <summary>Gets the dark raven mode 0 = cease, 1 = auto-attack, 2 = attack with owner.</summary>
    public int DarkRavenMode { get; init; }

    /// <summary>Gets the obtain range.</summary>
    public int ObtainRange { get; init; }

    /// <summary>Gets a value indicating whether pickup all items.</summary>
    public bool PickAllItems { get; init; }

    /// <summary>Gets a value indicating whether pickup selected items.</summary>
    public bool PickSelectItems { get; init; }

    /// <summary>Gets a value indicating whether pickup jewels.</summary>
    public bool PickJewel { get; init; }

    /// <summary>Gets a value indicating whether pickup zen.</summary>
    public bool PickZen { get; init; }

    /// <summary>Gets a value indicating whether pickup ancient items.</summary>
    public bool PickAncient { get; init; }

    /// <summary>Gets a value indicating whether pickup excellent items.</summary>
    public bool PickExcellent { get; init; }

    /// <summary>Gets a value indicating whether pickup extra items.</summary>
    public bool PickExtraItems { get; init; }

    /// <summary>Gets the extra item names. Up to 12 item name substrings; pick any dropped item whose name contains one of these.</summary>
    public IReadOnlyList<string> ExtraItemNames { get; init; } = [];

    /// <summary>Gets a value indicating whether to repair items.</summary>
    public bool RepairItem { get; init; }

    /// <summary>
    /// Deserializes a <see cref="MuHelperPlayerConfiguration"/> from the raw blob stored in
    /// <see cref="DataModel.Entities.Character.MuHelperConfiguration"/>.
    /// Returns <see langword="null"/> when the blob is null, empty, or too short.
    /// </summary>
    /// <param name="blob">The raw Mu Helper configuration.</param>
    public static MuHelperPlayerConfiguration? TryDeserialize(byte[]? blob)
    {
        if (blob is null || blob.Length < MinBlobLength)
        {
            return null;
        }

        byte pickupFlags = blob[PickupFlagsOffset];
        bool jewel = (pickupFlags & PickJewelFlag) != 0;
        bool setItem = (pickupFlags & PickSetItemFlag) != 0;
        bool excellent = (pickupFlags & PickExcellentFlag) != 0;
        bool zen = (pickupFlags & PickZenFlag) != 0;
        bool extraItem = (pickupFlags & PickExtraItemFlag) != 0;

        byte rangeFlags = blob[RangeFlagsOffset];
        int huntingRange = rangeFlags & HuntingRangeMask;
        int obtainRange = (rangeFlags >> ObtainRangeShift) & ObtainRangeMask;

        int distanceMin = BlobReadWord(blob, DistanceMinOffset);
        int basicSkill = BlobReadWord(blob, BasicSkillOffset);
        int activationSkill1 = BlobReadWord(blob, ActivationSkill1Offset);
        int delayMinSkill1 = BlobReadWord(blob, DelayMinSkill1Offset);
        int activationSkill2 = BlobReadWord(blob, ActivationSkill2Offset);
        int delayMinSkill2 = BlobReadWord(blob, DelayMinSkill2Offset);
        int castingBuffMin = BlobReadWord(blob, CastingBuffMinOffset);
        int buffSkill0 = BlobReadWord(blob, BuffSkill0Offset);
        int buffSkill1 = BlobReadWord(blob, BuffSkill1Offset);
        int buffSkill2 = BlobReadWord(blob, BuffSkill2Offset);

        byte hpThresholdFlags = blob[HpThresholdFlagsOffset];
        int hpPotion = (hpThresholdFlags & HpPotionNibbleMask) * HpThresholdMultiplier;
        int hpHeal = ((hpThresholdFlags >> HpHealNibbleShift) & HpHealNibbleMask) * HpThresholdMultiplier;

        byte partyDrainFlags = blob[PartyDrainThresholdOffset];
        int hpParty = (partyDrainFlags & HpPartyNibbleMask) * HpThresholdMultiplier;

        byte behaviorFlags = blob[BehaviorFlagsOffset];
        bool autoPotion = (behaviorFlags & AutoPotionFlag) != 0;
        bool autoHeal = (behaviorFlags & AutoHealFlag) != 0;
        bool drainLife = (behaviorFlags & DrainLifeFlag) != 0;
        bool longRangeCounterAttack = (behaviorFlags & LongRangeCounterAttackFlag) != 0;
        bool returnToOriginal = (behaviorFlags & ReturnToOriginalFlag) != 0;
        bool combo = (behaviorFlags & ComboFlag) != 0;
        bool party = (behaviorFlags & PartyFlag) != 0;
        bool prefPartyHeal = (behaviorFlags & PreferPartyHealFlag) != 0;

        byte skill1Flags = blob[Skill1FlagsOffset];
        bool buffDurationParty = (skill1Flags & BuffDurationPartyFlag) != 0;
        bool useDarkSpirits = (skill1Flags & UseDarkSpiritsFlag) != 0;
        bool buffDuration = (skill1Flags & BuffDurationFlag) != 0;
        bool skill1Delay = (skill1Flags & Skill1DelayFlag) != 0;
        bool skill1Con = (skill1Flags & Skill1ConditionFlag) != 0;
        bool skill1PreCon = (skill1Flags & Skill1PreConditionFlag) != 0;
        int skill1SubCon = (skill1Flags >> Skill1SubConditionShift) & Skill1SubConditionMask;

        byte skill2Flags = blob[Skill2FlagsOffset];
        bool skill2Delay = (skill2Flags & Skill2DelayFlag) != 0;
        bool skill2Con = (skill2Flags & Skill2ConditionFlag) != 0;
        bool skill2PreCon = (skill2Flags & Skill2PreConditionFlag) != 0;
        int skill2SubCon = (skill2Flags >> Skill2SubConditionShift) & Skill2SubConditionMask;
        bool repairItem = (skill2Flags & RepairItemFlag) != 0;
        bool pickAll = (skill2Flags & PickAllItemsFlag) != 0;
        bool pickSelect = (skill2Flags & PickSelectItemsFlag) != 0;

        int petAttack = blob[PetAttackOffset];

        var extraNames = new List<string>();
        if (blob.Length >= ExtraItemsEndOffset)
        {
            for (int i = 0; i < ExtraItemSlotCount; i++)
            {
                int start = ExtraItemsOffset + (i * ExtraItemSlotLength);
                int len = 0;
                while (len < ExtraItemSlotLength && blob[start + len] != 0)
                {
                    len++;
                }

                if (len > 0)
                {
                    extraNames.Add(Encoding.ASCII.GetString(blob, start, len));
                }
            }
        }

        return new MuHelperPlayerConfiguration
        {
            BasicSkillId = basicSkill,
            ActivationSkill1Id = activationSkill1,
            ActivationSkill2Id = activationSkill2,
            DelayMinSkill1 = delayMinSkill1,
            DelayMinSkill2 = delayMinSkill2,
            Skill1UseTimer = skill1Delay,
            Skill1UseCondition = skill1Con,
            Skill1ConditionAttacking = skill1PreCon,
            Skill1SubCondition = skill1SubCon,
            Skill2UseTimer = skill2Delay,
            Skill2UseCondition = skill2Con,
            Skill2ConditionAttacking = skill2PreCon,
            Skill2SubCondition = skill2SubCon,
            UseCombo = combo,
            HuntingRange = huntingRange,
            MaxSecondsAway = distanceMin,
            LongRangeCounterAttack = longRangeCounterAttack,
            ReturnToOriginalPosition = returnToOriginal,
            BuffSkill0Id = buffSkill0,
            BuffSkill1Id = buffSkill1,
            BuffSkill2Id = buffSkill2,
            BuffOnDuration = buffDuration,
            BuffDurationForParty = buffDurationParty,
            BuffCastIntervalSeconds = castingBuffMin,
            AutoHeal = autoHeal,
            HealThresholdPercent = hpHeal,
            UseDrainLife = drainLife,
            UseHealPotion = autoPotion,
            PotionThresholdPercent = hpPotion,
            SupportParty = party,
            AutoHealParty = prefPartyHeal,
            HealPartyThresholdPercent = hpParty,
            UseDarkRaven = useDarkSpirits,
            DarkRavenMode = petAttack,
            ObtainRange = obtainRange,
            PickAllItems = pickAll,
            PickSelectItems = pickSelect,
            PickJewel = jewel,
            PickZen = zen,
            PickAncient = setItem,
            PickExcellent = excellent,
            PickExtraItems = extraItem,
            ExtraItemNames = extraNames,
            RepairItem = repairItem,
        };
    }

    private static int BlobReadWord(byte[] blob, int offset)
        => blob[offset] | (blob[offset + 1] << 8); // little-endian
}