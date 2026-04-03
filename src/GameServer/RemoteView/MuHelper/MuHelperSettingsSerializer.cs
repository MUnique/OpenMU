// <copyright file="MuHelperSettingsSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MuHelper;

using System.Text;
using MUnique.OpenMU.GameLogic.MuHelper;

/// <summary>
/// Deserializer for <see cref="MuHelperSettings"/> from the raw 257-byte blob
/// stored in <see cref="DataModel.Entities.Character.MuHelperConfiguration"/>.
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
public static class MuHelperSettingsSerializer
{
    private const int MinBlobLength = 69;

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

    private const int PickJewelFlag = 1 << 3;
    private const int PickSetItemFlag = 1 << 4;
    private const int PickExcellentFlag = 1 << 5;
    private const int PickZenFlag = 1 << 6;
    private const int PickExtraItemFlag = 1 << 7;

    private const int HuntingRangeMask = 0x0F;
    private const int ObtainRangeShift = 4;
    private const int ObtainRangeMask = 0x0F;

    private const int HpPotionNibbleMask = 0x0F;
    private const int HpHealNibbleShift = 4;
    private const int HpHealNibbleMask = 0x0F;
    private const int HpPartyNibbleMask = 0x0F;
    private const int HpThresholdMultiplier = 10;

    private const int AutoPotionFlag = 1 << 0;
    private const int AutoHealFlag = 1 << 1;
    private const int DrainLifeFlag = 1 << 2;
    private const int LongRangeCounterAttackFlag = 1 << 3;
    private const int ReturnToOriginalFlag = 1 << 4;
    private const int ComboFlag = 1 << 5;
    private const int PartyFlag = 1 << 6;
    private const int PreferPartyHealFlag = 1 << 7;

    private const int BuffDurationPartyFlag = 1 << 0;
    private const int UseDarkSpiritsFlag = 1 << 1;
    private const int BuffDurationFlag = 1 << 2;
    private const int Skill1DelayFlag = 1 << 3;
    private const int Skill1ConditionFlag = 1 << 4;
    private const int Skill1PreConditionFlag = 1 << 5;
    private const int Skill1SubConditionShift = 6;
    private const int Skill1SubConditionMask = 0x03;

    private const int Skill2DelayFlag = 1 << 0;
    private const int Skill2ConditionFlag = 1 << 1;
    private const int Skill2PreConditionFlag = 1 << 2;
    private const int Skill2SubConditionShift = 3;
    private const int Skill2SubConditionMask = 0x03;
    private const int RepairItemFlag = 1 << 5;
    private const int PickAllItemsFlag = 1 << 6;
    private const int PickSelectItemsFlag = 1 << 7;

    /// <summary>
    /// Deserializes a <see cref="MuHelperSettings"/> from the raw blob.
    /// Returns <see langword="null"/> when the blob is null, empty, or too short.
    /// </summary>
    /// <param name="blob">The raw Mu Helper configuration.</param>
    public static IMuHelperSettings? TryDeserialize(byte[]? blob)
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

        return new MuHelperSettings
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
        => blob[offset] | (blob[offset + 1] << 8);
}
