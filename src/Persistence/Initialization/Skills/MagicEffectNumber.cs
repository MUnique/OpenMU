// <copyright file="MagicEffectNumber.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// This contains all magic effect numbers.
/// </summary>
/// <remarks>Might not be complete. Negative numbers are for internal usage and their effects are not exposed to the game client.</remarks>
internal enum MagicEffectNumber : short
{
    #region Artificial effects which don't end up as an actual magic effect, but regenerate something

    /// <summary>
    /// The shield recover skill effect number.
    /// </summary>
    /// <remarks>
    /// Internal.
    /// </remarks>
    ShieldRecover = -3,

    /// <summary>
    /// The heal skill effect number.
    /// </summary>
    /// <remarks>
    /// Internal.
    /// </remarks>
    Heal = -2,

    #endregion

    /// <summary>
    /// Undefined effect number.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The damage buff effect.
    /// </summary>
    GreaterDamage = 1,

    /// <summary>
    /// The defense buff effect.
    /// </summary>
    GreaterDefense = 2,

    /// <summary>
    /// The elf soldier buff effect.
    /// </summary>
    ElfSoldierBuff = 3,

    /// <summary>
    /// The soul barrier effect.
    /// </summary>
    SoulBarrier = 4,

    /// <summary>
    /// The critical damage increase effect.
    /// </summary>
    CriticalDamageIncrease = 5,

    /// <summary>
    /// The infinite arrow effect.
    /// </summary>
    InfiniteArrow = 6,

    /// <summary>
    /// The ability recover speed increase effect.
    /// </summary>
    AbilityRecoverSpeedIncrease = 7,

    /// <summary>
    /// The greater fortitude effect.
    /// </summary>
    GreaterFortitude = 8,

    /// <summary>
    /// The elite mana potion effect.
    /// </summary>
    EliteManaPotion = 9,

    /// <summary>
    /// The potion of bless effect.
    /// </summary>
    PotionOfBless = 10,

    /// <summary>
    /// The potion of soul effect.
    /// </summary>
    PotionOfSoul = 11,

    /// <summary>
    /// The gate open close status effect.
    /// </summary>
    GateOpenCloseStatus = 0x0D,

    /// <summary>
    /// The watchtower effect.
    /// </summary>
    Watchtower = 0x0E,

    /// <summary>
    /// The transparency effect.
    /// </summary>
    Transparency = 0x12,

    /// <summary>
    /// The game master effect.
    /// </summary>
    GameMaster = 0x1C,

    /// <summary>
    /// The seal ascension effect.
    /// </summary>
    SealAscension = 0x1D,

    /// <summary>
    /// The seal wealth effect.
    /// </summary>
    SealWealth = 0x1E,

    /// <summary>
    /// The seal sustenance effect.
    /// </summary>
    SealSustenance = 0x1F,

    /// <summary>
    /// The spell of quickness effect.
    /// </summary>
    SpellOfQuickness = 0x20,

    /// <summary>
    /// The spell of protection effect.
    /// </summary>
    SpellOfProtection = 0x22,

    /// <summary>
    /// The jack olantern blessing effect.
    /// </summary>
    JackOlanternBlessing = 0x23,

    /// <summary>
    /// The jack olantern wrath effect.
    /// </summary>
    JackOlanternWrath = 0x24,

    /// <summary>
    /// The jack olantern cry effect.
    /// </summary>
    JackOlanternCry = 0x25,

    /// <summary>
    /// The jack olantern food effect.
    /// </summary>
    JackOlanternFood = 0x26,

    /// <summary>
    /// The jack olantern drink effect.
    /// </summary>
    JackOlanternDrink = 0x27,

    /// <summary>
    /// The scroll of quickness effect.
    /// </summary>
    ScrollOfQuickness = 0x2C,

    /// <summary>
    /// The scroll of defense effect.
    /// </summary>
    ScrollOfDefense = 0x2D,

    /// <summary>
    /// The scroll of wrath effect.
    /// </summary>
    ScrollOfWrath = 0x2E,

    /// <summary>
    /// The scroll of wizardry effect.
    /// </summary>
    ScrollOfWizardry = 0x2F,

    /// <summary>
    /// The scroll of health effect.
    /// </summary>
    ScrollOfHealth = 0x30,

    /// <summary>
    /// The scroll of mana effect.
    /// </summary>
    ScrollOfMana = 0x31,

    /// <summary>
    /// The poisoned effect.
    /// </summary>
    Poisoned = 0x37,

    /// <summary>
    /// The iced effect.
    /// </summary>
    Iced = 0x38,

    /// <summary>
    /// The freeze effect, caused by the ice arrow skill.
    /// </summary>
    Freeze = 0x39,

    /// <summary>
    /// The defense reduction effect.
    /// </summary>
    DefenseReduction = 0x3A,

    /// <summary>
    /// The stunned effect.
    /// </summary>
    Stunned = 0x3D,

    /// <summary>
    /// The spell of restriction effect.
    /// </summary>
    SpellOfRestriction = 0x41,

    /// <summary>
    /// The reflection effect.
    /// </summary>
    Reflection = 0x47,

    /// <summary>
    /// The sleep effect.
    /// </summary>
    Sleep = 0x48,

    /// <summary>
    /// The blind effect.
    /// </summary>
    Blind = 0x49,

    /// <summary>
    /// The cherry blossom wine effect (+ 700 Mana).
    /// </summary>
    CherryBlossomWine = 0x4E,

    /// <summary>
    /// The cherry blossom rice cake effect (+ 700 Health).
    /// </summary>
    CherryBlossomRiceCake = 0x4F,

    /// <summary>
    /// The cherry blossom flower petal effect (+ 40 dmg).
    /// </summary>
    CherryBlossomFlowerPetal = 0x50,

    /// <summary>
    /// The wiz enhance effect.
    /// </summary>
    WizEnhance = 0x52,

    /// <summary>
    /// The ignore defense effect of the rage fighter.
    /// </summary>
    IgnoreDefense = 129,

    /// <summary>
    /// The increase health effect of the rage fighter.
    /// </summary>
    IncreaseHealth = 130,

    /// <summary>
    /// The increase block effect of the rage fighter.
    /// </summary>
    IncreaseBlock = 131,

    /// <summary>
    /// The wiz enhance strengthener effect.
    /// </summary>
    WizEnhance2 = 138,

    /// <summary>
    /// The wiz enhance mastery effect.
    /// </summary>
    WizEnhance3 = 139,

    #region Artificial effects which are not sent to the client, starting at 200.

    /// <summary>
    /// The shield skill effect number.
    /// </summary>
    /// <remarks>
    /// Internal.
    /// </remarks>
    ShieldSkill = 200,

    /// <summary>
    /// The alcohol effect number.
    /// </summary>
    Alcohol = 201,

    #endregion

}