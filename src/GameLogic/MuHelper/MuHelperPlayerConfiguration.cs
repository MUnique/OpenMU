// <copyright file="MuHelperConfig.cs" company="MUnique">
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
        if (blob is null || blob.Length < 69)
        {
            return null;
        }

        byte b1 = blob[1];
        bool jewel = (b1 & (1 << 3)) != 0;
        bool setItem = (b1 & (1 << 4)) != 0;
        bool excellent = (b1 & (1 << 5)) != 0;
        bool zen = (b1 & (1 << 6)) != 0;
        bool extraItem = (b1 & (1 << 7)) != 0;

        byte b2 = blob[2];
        int huntingRange = b2 & 0x0F;
        int obtainRange = (b2 >> 4) & 0x0F;

        int distanceMin = BlobReadWord(blob, 3);

        int basicSkill1 = BlobReadWord(blob, 5);
        int activationSkill1 = BlobReadWord(blob, 7);
        int delayMinSkill1 = BlobReadWord(blob, 9);
        int activationSkill2 = BlobReadWord(blob, 11);
        int delayMinSkill2 = BlobReadWord(blob, 13);
        int castingBuffMin = BlobReadWord(blob, 15);
        int buffSkill0 = BlobReadWord(blob, 17);
        int buffSkill1 = BlobReadWord(blob, 19);
        int buffSkill2 = BlobReadWord(blob, 21);

        byte b23 = blob[23];
        int hpPotion = (b23 & 0x0F) * 10;
        int hpHeal = ((b23 >> 4) & 0x0F) * 10;

        byte b24 = blob[24];
        int hpParty = (b24 & 0x0F) * 10;
        // HPStatusDrainLife uses the same threshold as HPStatusAutoHeal per the serialize code
        byte b25 = blob[25];
        bool autoPotion = (b25 & (1 << 0)) != 0;
        bool autoHeal = (b25 & (1 << 1)) != 0;
        bool drainLife = (b25 & (1 << 2)) != 0;
        bool longRangeCounterAttack = (b25 & (1 << 3)) != 0;
        bool returnToOriginal = (b25 & (1 << 4)) != 0;
        bool combo = (b25 & (1 << 5)) != 0;
        bool party = (b25 & (1 << 6)) != 0;
        bool prefPartyHeal = (b25 & (1 << 7)) != 0;

        byte b26 = blob[26];
        bool buffDurationParty = (b26 & (1 << 0)) != 0;
        bool useDarkSpirits = (b26 & (1 << 1)) != 0;
        bool buffDuration = (b26 & (1 << 2)) != 0;
        bool skill1Delay = (b26 & (1 << 3)) != 0;
        bool skill1Con = (b26 & (1 << 4)) != 0;
        bool skill1PreCon = (b26 & (1 << 5)) != 0; // 0=nearby, 1=attacking
        int skill1SubCon = (b26 >> 6) & 0x03;

        byte b27 = blob[27];
        bool skill2Delay = (b27 & (1 << 0)) != 0;
        bool skill2Con = (b27 & (1 << 1)) != 0;
        bool skill2PreCon = (b27 & (1 << 2)) != 0;
        int skill2SubCon = (b27 >> 3) & 0x03;
        bool repairItem = (b27 & (1 << 5)) != 0;
        bool pickAll = (b27 & (1 << 6)) != 0;
        bool pickSelect = (b27 & (1 << 7)) != 0;

        int petAttack = blob[28];

        var extraNames = new List<string>();
        if (blob.Length >= 245)
        {
            for (int i = 0; i < 12; i++)
            {
                int start = 65 + (i * 15);
                int len = 0;
                while (len < 15 && blob[start + len] != 0)
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
            BasicSkillId = basicSkill1,
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