// <copyright file="Jewelery.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initializer for jewelery (rings and pendants).
/// </summary>
internal class Jewelery : Version075.Items.Jewelery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Jewelery"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Jewelery(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Creates the items.
    /// </summary>
    protected override void CreateItems()
    {
        this.CreateRing(8, "Ring of Ice", 20, 50, Stats.IceResistance);
        this.CreateRing(9, "Ring of Poison", 17, 50, Stats.PoisonResistance);

        this.CreatePendant(12, "Pendant of Lighting", 21, 50, DamageType.Wizardry, Stats.LightningResistance);
        this.CreatePendant(13, "Pendant of Fire", 13, 50, DamageType.Physical, Stats.FireResistance);

        this.CreateTransformationRing(
            10,
            "Transformation Ring",
            0,
            200,
            20, // It's actually lvl 50 for the last 3
            CharacterTransformationSkin.BudgeDragon,
            CharacterTransformationSkin.Giant,
            CharacterTransformationSkin.SkeletonWarrior,
            CharacterTransformationSkin.PoisonBullFighter,
            CharacterTransformationSkin.ThunderLich,
            CharacterTransformationSkin.DeathCow);
    }

    /// <summary>
    /// Creates the jewelery.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="dropsFromMonsters">if set to <c>true</c> [drops from monsters].</param>
    /// <param name="name">The name.</param>
    /// <param name="level">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="excellentOptionDefinition">The excellent option definition.</param>
    /// <param name="resistanceAttribute">The resistance attribute.</param>
    /// <param name="withHealthOption">if set to <c>true</c> [with health option].</param>
    /// <returns>The created jewelery.</returns>
    protected ItemDefinition CreateJewelery(byte number, int slot, bool dropsFromMonsters, string name, byte level, byte durability, ItemOptionDefinition? excellentOptionDefinition, AttributeDefinition? resistanceAttribute, bool withHealthOption = true)
    {
        var item = this.CreateJewelery(number, slot, dropsFromMonsters, name, level, durability, resistanceAttribute, withHealthOption);

        if (excellentOptionDefinition != null)
        {
            item.PossibleItemOptions.Add(excellentOptionDefinition);
        }

        return item;
    }

    /// <summary>
    /// Creates a ring.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name.</param>
    /// <param name="level">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="resistanceAttribute">The resistance attribute.</param>
    /// <remarks>
    /// Rings always have defensive excellent options.
    /// </remarks>
    private void CreateRing(byte number, string name, byte level, byte durability, AttributeDefinition? resistanceAttribute)
    {
        this.CreateJewelery(number, 10, true, name, level, durability, this.GameConfiguration.ExcellentDefenseOptions(), resistanceAttribute);
    }

    /// <summary>
    /// Creates a pendant.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name.</param>
    /// <param name="level">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="excellentOptionDamageType">Type of the excellent option damage.</param>
    /// <param name="resistanceAttribute">The resistance attribute.</param>
    /// <remarks>
    /// Pendants always have offensive excellent options. If it's wizardry or physical depends on the specific item. I didn't find a pattern yet.
    /// </remarks>
    private void CreatePendant(byte number, string name, byte level, byte durability, DamageType excellentOptionDamageType, AttributeDefinition? resistanceAttribute)
    {
        var excellentOption = excellentOptionDamageType == DamageType.Physical
            ? this.GameConfiguration.ExcellentPhysicalAttackOptions()
            : this.GameConfiguration.ExcellentWizardryAttackOptions();

        this.CreateJewelery(number, 9, true, name, level, durability, excellentOption, resistanceAttribute);
    }
}