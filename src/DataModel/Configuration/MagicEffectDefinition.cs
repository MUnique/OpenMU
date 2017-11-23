// <copyright file="MagicEffectDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using MUnique.OpenMU.DataModel.Attributes;

    /// <summary>
    /// Magic Effect Definition. It can be an effect from an consumed item, or by a buff.
    /// </summary>
    public class MagicEffectDefinition
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <remarks>
        /// This number is a reference for the game client.
        /// </remarks>
        public byte Number { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sub type.
        /// Same sub type = cant stack. adding a magic effect with the same sub type will cause the existing magic effect to disappear.
        /// </summary>
        public byte SubType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the effect change is sent to observers.
        /// </summary>
        /// <remarks>
        /// Some effects are not externally visible, but only to the player himself.
        /// </remarks>
        public bool InformObservers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the effect gets stopped by a death of the player.
        /// </summary>
        public bool StopByDeath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the duration of the effect should be sent.
        /// </summary>
        public bool SendDuration { get; set; }

        /// <summary>
        /// Gets or sets the powerUp definition which is used to create the actual power up element.
        /// </summary>
        public virtual PowerUpDefinitionWithDuration PowerUpDefinition { get; set; }
    }

    /* IDs:
     * public enum Effects
    {
        //Not Complete!
        DmgBuff = 1,
        DefBuff = 2,
        NPCBuff = 3,
        SoulBarrier = 4,
        CritDmgDL = 5,
        InfiniteArrow = 6,
        AGrecoverSpeedInc = 7,
        HPFortitude = 8,
        EliteManaPotion = 9,
        PotionOfBless = 10,
        PotionOfSoul = 11,
        GateOpenCloseStatus = 0x0D,
        Watchtower = 0x0E,
        Transperency = 0x12,
        GM = 0x1C,
        SealAscension = 0x1D,
        SealWealth = 0x1E,
        SealSustenance = 0x1F,
        SpellOfQuickness = 0x20,
        SpellOfProtection = 0x22,
        JackOlanternBlessing = 0x23,
        JackOlanternWrath = 0x24,
        JackOlanternCry = 0x25,
        JackOlanternFood = 0x26,
        JackOlanternDrink = 0x27,
        ScrollOfQuickness = 0x2C,
        ScrollOfDefense = 0x2D,
        //....
        Poisoned = 0x37,
        Iced = 0x38,
        IcedArrowed = 0x39,
        DefenseReduction = 0x3A,
        Stunned = 0x3D,
        SpellOfRestriction = 0x41,
        Reflection = 0x47,
        Sleep = 0x48,
        Blind = 0x49,
        WizEnhance = 0x52
    }*/
}
