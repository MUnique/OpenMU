// <copyright file="ItemRequirementCalculationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.BasicModel;
using IncreasableItemOption = MUnique.OpenMU.Persistence.BasicModel.IncreasableItemOption;
using ItemBasePowerUpDefinition = MUnique.OpenMU.Persistence.BasicModel.ItemBasePowerUpDefinition;
using ItemDefinition = MUnique.OpenMU.Persistence.BasicModel.ItemDefinition;
using ItemSlotType = MUnique.OpenMU.Persistence.BasicModel.ItemSlotType;

/// <summary>
/// Unit tests for <see cref="GameLogic.ItemExtensions.GetRequirement"/>.
/// </summary>
[TestFixture]
public class ItemRequirementCalculationTest
{
    /// <summary>
    /// Tests requirement calculation for the 'Vine Helm'.
    /// </summary>
    /// <param name="itemLevel">The item level.</param>
    /// <param name="requiredStrength">The required strength.</param>
    /// <param name="requiredAgility">The required agility.</param>
    [TestCase(0, 25, 30)]
    [TestCase(1, 28, 36)]
    [TestCase(2, 30, 41)]
    [TestCase(3, 33, 47)]
    [TestCase(4, 36, 52)]
    [TestCase(5, 38, 57)]
    [TestCase(6, 41, 63)]
    [TestCase(7, 44, 68)]
    [TestCase(8, 47, 74)]
    [TestCase(9, 49, 79)]
    [TestCase(10, 52, 84)]
    [TestCase(11, 55, 90)]
    [TestCase(12, 57, 95)]
    [TestCase(13, 60, 101)]
    [TestCase(14, 63, 106)]
    [TestCase(15, 65, 111)]
    public void VineHelm(byte itemLevel, int requiredStrength, int requiredAgility)
    {
        var item = new Item();
        item.Level = itemLevel;
        item.Definition = new ItemDefinition();
        item.Definition.DropLevel = 6;
        item.Definition.Group = 7;
        item.Definition.ItemSlot = new ItemSlotType();
        var strengthRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalStrengthRequirementValue, MinimumValue = 30 };
        var agilityRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalAgilityRequirementValue, MinimumValue = 60 };

        var strengthValue = item.GetRequirement(strengthRequirement, TestItemStatProfiles.ForItem(item.Definition!));
        var agilityValue = item.GetRequirement(agilityRequirement, TestItemStatProfiles.ForItem(item.Definition!));

        Assert.That(strengthValue.Item1, Is.EqualTo(Stats.TotalStrength));
        Assert.That(strengthValue.Item2, Is.EqualTo(requiredStrength));

        Assert.That(agilityValue.Item1, Is.EqualTo(Stats.TotalAgility));
        Assert.That(agilityValue.Item2, Is.EqualTo(requiredAgility));
    }

    /// <summary>
    /// Tests that a non-armor item adds 4 strength per item-option level.
    /// </summary>
    [Test]
    public void OptionAdds4Strength()
    {
        var item = new Item();
        item.Definition = new ItemDefinition();
        item.Definition.DropLevel = 6;
        item.Definition.Group = 0;
        item.Definition.ItemSlot = new ItemSlotType();
        item.Definition.ItemSlot.ItemSlots.Add(InventoryConstants.LeftHandSlot);
        item.ItemOptions.Add(new ItemOptionLink { Level = 2, ItemOption = new IncreasableItemOption { OptionType = ItemOptionTypes.Option } });
        var strengthRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalStrengthRequirementValue, MinimumValue = 30 };
        var agilityRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalAgilityRequirementValue, MinimumValue = 60 };

        var strengthValue = item.GetRequirement(strengthRequirement, TestItemStatProfiles.ForItem(item.Definition!));
        var agilityValue = item.GetRequirement(agilityRequirement, TestItemStatProfiles.ForItem(item.Definition!));

        Assert.That(strengthValue.Item1, Is.EqualTo(Stats.TotalStrength));
        Assert.That(strengthValue.Item2, Is.EqualTo(33));

        Assert.That(agilityValue.Item1, Is.EqualTo(Stats.TotalAgility));
        Assert.That(agilityValue.Item2, Is.EqualTo(30));
    }

    /// <summary>
    /// Tests that an armor item adds 5 strength per item-option level, whether it's equipped or not.
    /// </summary>
    /// <param name="currentSlot">The current item slot.</param> 
    [TestCase(2)]
    [TestCase(12)]
    public void ArmorOptionAdds5Strength(byte currentSlot)
    {
        var item = new Item();
        item.Definition = new ItemDefinition();
        item.Definition.DropLevel = 6;
        item.Definition.Group = 7;
        item.Definition.ItemSlot = new ItemSlotType();
        item.Definition.ItemSlot.ItemSlots.Add(InventoryConstants.HelmSlot);
        item.ItemSlot = currentSlot;
        item.ItemOptions.Add(new ItemOptionLink { Level = 2, ItemOption = new IncreasableItemOption { OptionType = ItemOptionTypes.Option } });
        var strengthRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalStrengthRequirementValue, MinimumValue = 30 };

        var strengthValue = item.GetRequirement(strengthRequirement, TestItemStatProfiles.ForItem(item.Definition!));

        Assert.That(strengthValue.Item1, Is.EqualTo(Stats.TotalStrength));
        Assert.That(strengthValue.Item2, Is.EqualTo(35));
    }

    /// <summary>
    /// Tests that a requirement can be made conditional on the item's level.
    /// </summary>
    /// <param name="itemLevel">The item level.</param>
    /// <param name="expectedRequirement">The expected requirement.</param>
    [TestCase(2, 0)]
    [TestCase(3, 50)]
    public void RequirementStartsAtConfiguredItemLevel(byte itemLevel, int expectedRequirement)
    {
        var item = new Item { Level = itemLevel };
        var requirement = new Persistence.BasicModel.AttributeRequirement
        {
            Attribute = Stats.Level,
            MinimumValue = 50,
            MinimumItemLevel = 3,
        };

        var result = item.GetRequirement(requirement, TestItemStatProfiles.ForItem(item.Definition!));

        Assert.That(result.Attr, Is.EqualTo(Stats.Level));
        Assert.That(result.Value, Is.EqualTo(expectedRequirement));
    }

    /// <summary>
    /// Tests requirement calculation for the 'Sunlight Armor'.
    /// </summary>
    /// <param name="itemLevel">The item level.</param>
    /// <param name="requiredStrength">The required strength.</param>
    /// <param name="requiredAgility">The required agility.</param>
    [TestCase(0, 293, 90)]
    [TestCase(1, 299, 92)]
    [TestCase(2, 304, 93)]
    [TestCase(3, 310, 94)]
    [TestCase(4, 315, 96)]
    [TestCase(5, 321, 97)]
    [TestCase(6, 326, 99)]
    [TestCase(7, 332, 100)]
    [TestCase(8, 338, 102)]
    [TestCase(9, 343, 103)]
    [TestCase(10, 349, 104)]
    [TestCase(11, 354, 106)]
    [TestCase(12, 360, 107)]
    [TestCase(13, 365, 109)]
    [TestCase(14, 371, 110)]
    [TestCase(15, 377, 112)]
    public void SunlightArmor(byte itemLevel, int requiredStrength, int requiredAgility)
    {
        var item = new Item();
        item.Level = itemLevel;
        item.Definition = new ItemDefinition();
        item.Definition.DropLevel = 147;
        item.Definition.Group = 8;
        item.Definition.ItemSlot = new ItemSlotType();
        var strengthRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalStrengthRequirementValue, MinimumValue = 62 };
        var agilityRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalAgilityRequirementValue, MinimumValue = 16 };

        var strengthValue = item.GetRequirement(strengthRequirement, TestItemStatProfiles.ForItem(item.Definition!));
        var agilityValue = item.GetRequirement(agilityRequirement, TestItemStatProfiles.ForItem(item.Definition!));

        Assert.That(strengthValue.Item1, Is.EqualTo(Stats.TotalStrength));
        Assert.That(strengthValue.Item2, Is.EqualTo(requiredStrength));

        Assert.That(agilityValue.Item1, Is.EqualTo(Stats.TotalAgility));
        Assert.That(agilityValue.Item2, Is.EqualTo(requiredAgility));
    }

    /// <summary>
    /// Tests the requirement calculation of the 'Book of Neil'.
    /// Energy requirement calculation of summoner books are different from other items, so a unit test makes sense here.
    /// </summary>
    /// <param name="itemLevel">The item level.</param>
    /// <param name="requiredEnergy">The required energy.</param>
    /// <param name="requiredAgility">The required agility.</param>
    [TestCase(0, 317, 64)]
    [TestCase(1, 322, 66)]
    [TestCase(2, 327, 68)]
    [TestCase(3, 332, 71)]
    [TestCase(4, 337, 73)]
    [TestCase(5, 342, 75)]
    [TestCase(6, 347, 77)]
    [TestCase(7, 352, 80)]
    [TestCase(8, 357, 82)]
    [TestCase(9, 362, 84)]
    [TestCase(10, 367, 86)]
    [TestCase(11, 372, 89)]
    [TestCase(12, 377, 91)]
    [TestCase(13, 382, 93)]
    [TestCase(14, 387, 95)]
    [TestCase(15, 392, 98)]
    public void BookOfNeil(byte itemLevel, int requiredEnergy, int requiredAgility)
    {
        var item = new Item();
        item.Level = itemLevel;
        item.Definition = new ItemDefinition();
        item.Definition.DropLevel = 59;
        item.Definition.ItemSlot = new ItemSlotType();
        item.Definition.BasePowerUpAttributes.Add(new ItemBasePowerUpDefinition { TargetAttribute = Stats.BookRise, BaseValue = 23 });
        var energyRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalEnergyRequirementValue, MinimumValue = 168 };
        var agilityRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalAgilityRequirementValue, MinimumValue = 25 };

        var energyValue = item.GetRequirement(energyRequirement, TestItemStatProfiles.ForItem(item.Definition!));
        var agilityValue = item.GetRequirement(agilityRequirement, TestItemStatProfiles.ForItem(item.Definition!));

        Assert.That(energyValue.Item1, Is.EqualTo(Stats.TotalEnergy));
        Assert.That(energyValue.Item2, Is.EqualTo(requiredEnergy));

        Assert.That(agilityValue.Item1, Is.EqualTo(Stats.TotalAgility));
        Assert.That(agilityValue.Item2, Is.EqualTo(requiredAgility));
    }

    /// <summary>
    /// Tests that an energy requirement of 0 of a book stays 0, as for other items (see MUnique/OpenMU#1301).
    /// </summary>
    [Test]
    public void BookEnergyRequirementOf0Stays0()
    {
        var item = CreateBook(itemLevel: 9);
        var energyRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalEnergyRequirementValue, MinimumValue = 0 };

        var energyValue = item.GetRequirement(energyRequirement, TestItemStatProfiles.ForItem(item.Definition!));

        Assert.That(energyValue.Item1, Is.EqualTo(Stats.TotalEnergy));
        Assert.That(energyValue.Item2, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that an energy requirement reduction, e.g. of a harmony option, lowers the energy requirement of a book,
    /// as for other items (see MUnique/OpenMU#1301).
    /// </summary>
    [Test]
    public void BookEnergyRequirementIsReduced()
    {
        var item = CreateBook(itemLevel: 0);
        item.ItemOptions.Add(new ItemOptionLink
        {
            ItemOption = new IncreasableItemOption
            {
                OptionType = ItemOptionTypes.HarmonyOption,
                PowerUpDefinition = new PowerUpDefinition
                {
                    TargetAttribute = Stats.RequiredEnergyReduction,
                    Boost = new PowerUpDefinitionValue { ConstantValue = { Value = 10 } },
                },
            },
        });
        var energyRequirement = new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.TotalEnergyRequirementValue, MinimumValue = 168 };

        var energyValue = item.GetRequirement(energyRequirement, TestItemStatProfiles.ForItem(item.Definition!));

        // 317 of the 'Book of Neil' at level 0, minus the reduction of 10
        Assert.That(energyValue.Item1, Is.EqualTo(Stats.TotalEnergy));
        Assert.That(energyValue.Item2, Is.EqualTo(307));
    }

    private static Item CreateBook(byte itemLevel)
    {
        var item = new Item();
        item.Level = itemLevel;
        item.Definition = new ItemDefinition();
        item.Definition.DropLevel = 59;
        item.Definition.ItemSlot = new ItemSlotType();
        item.Definition.BasePowerUpAttributes.Add(new ItemBasePowerUpDefinition { TargetAttribute = Stats.BookRise, BaseValue = 23 });
        return item;
    }
}
