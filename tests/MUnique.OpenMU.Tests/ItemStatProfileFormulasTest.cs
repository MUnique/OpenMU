// <copyright file="ItemStatProfileFormulasTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.ItemFormulas;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests the item formulas with values of the item stat profiles which the built-in profiles don't use; the comparison
/// test of the initialization tests proves the built-in values against today's code.
/// </summary>
[TestFixture]
public class ItemStatProfileFormulasTest
{
    private const byte DropLevel = 40;

    /// <summary>
    /// Tests that the level requirement grows with the profile's level growth per item level.
    /// </summary>
    [Test]
    public void LevelRequirementGrowsWithTheItemLevel()
    {
        var configuration = TestItemStatProfiles.Create();
        GetDefault(configuration).LevelPerItemLevel = 4;
        var item = CreateArmor(level: 3);

        Assert.That(item.GetRequirement(Requirement(Stats.Level, 50), configuration).Value, Is.EqualTo(62));
    }

    /// <summary>
    /// Tests that a trainable pet counts as level 0 for the level growth.
    /// </summary>
    [Test]
    public void TrainablePetCountsAsLevel0()
    {
        var configuration = TestItemStatProfiles.Create();
        GetDefault(configuration).LevelPerItemLevel = 4;
        var item = CreateArmor(level: 3);
        item.Definition!.PetExperienceFormula = "level * 100";

        Assert.That(item.GetRequirement(Requirement(Stats.Level, 50), configuration).Value, Is.EqualTo(50));
    }

    /// <summary>
    /// Tests that an excellent item needs the profile's excellent level add more, unless its definition says it doesn't.
    /// </summary>
    /// <param name="excellentRaisesLevel">The value of <see cref="ItemDefinition.ExcellentRaisesLevel"/>.</param>
    /// <param name="expectedLevel">The expected level requirement.</param>
    [TestCase(true, 70)]
    [TestCase(false, 50)]
    public void ExcellentItemNeedsMoreLevel(bool excellentRaisesLevel, int expectedLevel)
    {
        var configuration = TestItemStatProfiles.Create();
        GetDefault(configuration).ExcellentLevelAdd = 20;
        var item = MakeExcellent(CreateArmor());
        item.Definition!.ExcellentRaisesLevel = excellentRaisesLevel;

        Assert.That(item.GetRequirement(Requirement(Stats.Level, 50), configuration).Value, Is.EqualTo(expectedLevel));
    }

    /// <summary>
    /// Tests that a formula which keeps the base value doesn't let the requirement grow.
    /// </summary>
    [Test]
    public void KeepBaseKeepsTheStoredValue()
    {
        var configuration = TestItemStatProfiles.Create();
        GetFormula(GetDefault(configuration), Stats.TotalAgilityRequirementValue).KeepBase = true;
        var item = CreateArmor(level: 9);

        Assert.That(item.GetRequirement(Requirement(Stats.TotalAgilityRequirementValue, 60), configuration).Value, Is.EqualTo(60));
    }

    /// <summary>
    /// Tests that a formula which keeps the base value only keeps the growth: the option strength and a requirement
    /// reduction still apply, as in the game client.
    /// </summary>
    [Test]
    public void KeepBaseStillAddsTheOptionAndTheReduction()
    {
        var configuration = TestItemStatProfiles.Create();
        GetFormula(GetDefault(configuration), Stats.TotalStrengthRequirementValue).KeepBase = true;
        var item = CreateArmor(level: 9);
        item.ItemOptions.Add(new BasicModel.ItemOptionLink { Level = 2, ItemOption = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.Option } });
        item.ItemOptions.Add(new BasicModel.ItemOptionLink
        {
            ItemOption = new BasicModel.IncreasableItemOption
            {
                OptionType = ItemOptionTypes.HarmonyOption,
                PowerUpDefinition = new BasicModel.PowerUpDefinition
                {
                    TargetAttribute = Stats.RequiredStrengthReduction,
                    Boost = new BasicModel.PowerUpDefinitionValue { ConstantValue = { Value = 10 } },
                },
            },
        });

        // the base 60, plus 2 option levels * 4 of the default, minus the reduction of 10
        Assert.That(item.GetRequirement(Requirement(Stats.TotalStrengthRequirementValue, 60), configuration).Value, Is.EqualTo(58));
    }

    /// <summary>
    /// Tests that a formula with the level requirement basis takes the item's level requirement instead of the drop level.
    /// </summary>
    [Test]
    public void LevelRequirementBasisTakesTheLevelRequirement()
    {
        var configuration = TestItemStatProfiles.Create();
        GetFormula(GetDefault(configuration), Stats.TotalAgilityRequirementValue).LevelRequirementBasis = true;
        var item = CreateArmor(level: 9);
        item.Definition!.Requirements.Add(Requirement(Stats.Level, 100));

        // 20 + 60 * 100 * 3 / 100
        Assert.That(item.GetRequirement(Requirement(Stats.TotalAgilityRequirementValue, 60), configuration).Value, Is.EqualTo(200));
    }

    /// <summary>
    /// Tests that the requirements of an excellent item follow the configured excellent drop level bonus.
    /// </summary>
    /// <param name="bonus">The excellent drop level bonus.</param>
    /// <param name="expected">The expected agility requirement.</param>
    [TestCase((byte)25, 137)]
    [TestCase((byte)40, 164)]
    public void RequirementsFollowTheExcellentDropLevelBonus(byte bonus, int expected)
    {
        var configuration = TestItemStatProfiles.Create();
        configuration.ExcellentDropLevelBonus = bonus;
        var item = MakeExcellent(CreateArmor());

        // 20 + 60 * (40 + bonus) * 3 / 100
        Assert.That(item.GetRequirement(Requirement(Stats.TotalAgilityRequirementValue, 60), configuration).Value, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that the requirements of an ancient item follow the configured ancient drop level bonus, also when it's
    /// excellent too.
    /// </summary>
    /// <param name="bonus">The ancient drop level bonus.</param>
    /// <param name="alsoExcellent">If set to <c>true</c>, the item is excellent too.</param>
    /// <param name="expected">The expected agility requirement.</param>
    [TestCase((byte)30, false, 146)]
    [TestCase((byte)50, false, 182)]
    [TestCase((byte)50, true, 182)]
    public void RequirementsFollowTheAncientDropLevelBonus(byte bonus, bool alsoExcellent, int expected)
    {
        var configuration = TestItemStatProfiles.Create();
        configuration.AncientDropLevelBonus = bonus;
        var item = MakeAncient(CreateArmor());
        if (alsoExcellent)
        {
            MakeExcellent(item);
        }

        // 20 + 60 * (40 + bonus) * 3 / 100; the excellent bonus of 25 would give 137
        Assert.That(item.GetRequirement(Requirement(Stats.TotalAgilityRequirementValue, 60), configuration).Value, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that a requirement formula and a part with large factors, which an owner can configure, are limited
    /// instead of wrapping to a negative value.
    /// </summary>
    [Test]
    public void LargeFactorsAreLimitedInsteadOfWrapping()
    {
        var configuration = TestItemStatProfiles.Create();
        GetFormula(GetDefault(configuration), Stats.TotalAgilityRequirementValue).Percent = int.MaxValue;
        var item = CreateArmor(level: 9);
        var part = new ItemStatTermPart(ItemStatTermState.Values, int.MaxValue, 0, 0, false);

        Assert.Multiple(() =>
        {
            Assert.That(item.GetRequirement(Requirement(Stats.TotalAgilityRequirementValue, 60), configuration).Value, Is.EqualTo(int.MaxValue));
            Assert.That(part.Compute(300, DropLevel), Is.EqualTo(int.MaxValue));
        });
    }

    /// <summary>
    /// Tests that an excellent part which is limited to <see cref="int.MaxValue"/> and the base value are added up
    /// without wrapping before the ancient part is computed from them.
    /// </summary>
    [Test]
    public void LimitedExcellentPartDoesNotWrapTheAncientPart()
    {
        var configuration = TestItemStatProfiles.Create();
        GetTerms(GetDefault(configuration), Stats.DefenseBase).ExcellentAdd = int.MaxValue;
        var armor = MakeAncient(MakeExcellent(CreateItem(group: 7, slot: InventoryConstants.HelmSlot, (Stats.DefenseBase, 30))));

        // The excellent part (30 * 12 / 40 + int.MaxValue + 40 / 5) is limited to int.MaxValue; the ancient part of
        // the default is 2 + (30 + int.MaxValue) * 3 / (40 + 30) + 70 / 30. The attribute system adds them as float.
        var ancient = 2 + ((30L + int.MaxValue) * 3 / 70) + 2;
        Assert.That(GetPowerUps(armor, configuration)[Stats.DefenseBase], Is.EqualTo(30d + int.MaxValue + ancient).Within(256));
    }

    /// <summary>
    /// Tests that a formula with the level requirement basis takes the stored level requirement of the item, without its
    /// growth per item level and the excellent addition, as in the game client.
    /// </summary>
    [Test]
    public void LevelRequirementBasisTakesTheStoredLevelRequirement()
    {
        var configuration = TestItemStatProfiles.Create();
        var defaultProfile = GetDefault(configuration);
        defaultProfile.LevelPerItemLevel = 4;
        defaultProfile.ExcellentLevelAdd = 20;
        GetFormula(defaultProfile, Stats.TotalAgilityRequirementValue).LevelRequirementBasis = true;
        var item = MakeExcellent(CreateArmor(level: 5));
        var levelRequirement = Requirement(Stats.Level, 100);
        item.Definition!.Requirements.Add(levelRequirement);

        Assert.Multiple(() =>
        {
            // the level requirement grows: 100 + 5 * 4 + 20
            Assert.That(item.GetRequirement(levelRequirement, configuration).Value, Is.EqualTo(140));

            // the formula takes the stored 100: 20 + 60 * 100 * 3 / 100
            Assert.That(item.GetRequirement(Requirement(Stats.TotalAgilityRequirementValue, 60), configuration).Value, Is.EqualTo(200));
        });
    }

    /// <summary>
    /// Tests that the item's profile is laid over the default: its own formula applies, the others come from the default.
    /// </summary>
    [Test]
    public void ProfileFormulaReplacesTheOneOfTheDefault()
    {
        var configuration = TestItemStatProfiles.Create();
        var profile = new BasicModel.ItemStatProfile { Name = "own" };
        profile.RequirementFormulas.Add(new BasicModel.ItemRequirementFormula { Attribute = Stats.TotalAgilityRequirementValue, Add = 10, LevelWeight = 0, Percent = 1 });
        configuration.ItemStatProfiles.Add(profile);
        var item = CreateArmor(level: 9);
        item.Definition!.StatProfile = profile;

        Assert.Multiple(() =>
        {
            // own: 10 + 60 * 40 * 1 / 100; default: 20 + 60 * (40 + 27) * 3 / 100
            Assert.That(item.GetRequirement(Requirement(Stats.TotalAgilityRequirementValue, 60), configuration).Value, Is.EqualTo(34));
            Assert.That(item.GetRequirement(Requirement(Stats.TotalStrengthRequirementValue, 60), configuration).Value, Is.EqualTo(140));
        });
    }

    /// <summary>
    /// Tests that an item without a profile has the formulas of the default.
    /// </summary>
    [Test]
    public void ItemWithoutProfileTakesTheDefault()
    {
        var configuration = TestItemStatProfiles.Create();
        var item = CreateArmor(level: 9);
        Assert.That(item.Definition!.StatProfile, Is.Null);

        // the default's formula: 20 + 60 * (40 + 27) * 3 / 100
        Assert.That(item.GetRequirement(Requirement(Stats.TotalStrengthRequirementValue, 60), configuration).Value, Is.EqualTo(140));
    }

    /// <summary>
    /// Tests that a fixed excellent bonus replaces the excellent part of the damage.
    /// </summary>
    [Test]
    public void FixedExcellentBonusReplacesTheExcellentPartOfTheDamage()
    {
        var configuration = TestItemStatProfiles.Create();
        var weapon = MakeExcellent(CreateItem(group: 0, slot: InventoryConstants.LeftHandSlot, (Stats.MinimumPhysBaseDmgByWeapon, 50), (Stats.MaximumPhysBaseDmgByWeapon, 60)));
        weapon.Definition!.ExcellentBonus = 15;

        var attributes = GetPowerUps(weapon, configuration);

        Assert.Multiple(() =>
        {
            Assert.That(attributes[Stats.MinimumPhysBaseDmgByWeapon], Is.EqualTo(50 + 15));
            Assert.That(attributes[Stats.MaximumPhysBaseDmgByWeapon], Is.EqualTo(60 + 15));
        });
    }

    /// <summary>
    /// Tests that an ancient part after the level bonus adds the level bonus of the item's power-up to the value.
    /// </summary>
    [Test]
    public void AncientPartAfterLevelBonusAddsTheLevelBonus()
    {
        var configuration = TestItemStatProfiles.Create();
        var shield = MakeAncient(CreateItem(group: 6, slot: InventoryConstants.RightHandSlot, (Stats.DefenseShield, 10)));
        shield.Level = 5;
        var table = new BasicModel.ItemLevelBonusTable();
        table.BonusPerLevel.Add(new BasicModel.LevelBonus { Level = 5, AdditionalValue = 7 });
        shield.Definition!.BasePowerUpAttributes.Single(p => p.TargetAttribute == Stats.DefenseShield).BonusPerLevelTable = table;
        TestItemStatProfiles.ForItem(shield.Definition, configuration);

        var attributes = GetPowerUps(shield, configuration);

        // base 10 + level bonus 7, and the ancient part of the shield profile: 2 + (10 + 7) * 20 / (40 + 30)
        Assert.That(attributes[Stats.DefenseShield], Is.EqualTo(10 + 7 + 2 + (17 * 20 / 70)));
    }

    /// <summary>
    /// Tests that the ancient part follows the configured ancient drop level bonus.
    /// </summary>
    /// <param name="bonus">The ancient drop level bonus.</param>
    /// <param name="expected">The expected shield defense.</param>
    [TestCase((byte)30, 35 + 2 + (35 * 20 / 70))]
    [TestCase((byte)60, 35 + 2 + (35 * 20 / 100))]
    public void AncientPartFollowsTheAncientDropLevelBonus(byte bonus, int expected)
    {
        var configuration = TestItemStatProfiles.Create();
        configuration.AncientDropLevelBonus = bonus;
        var shield = MakeAncient(CreateItem(group: 6, slot: InventoryConstants.RightHandSlot, (Stats.DefenseShield, 35)));
        TestItemStatProfiles.ForItem(shield.Definition, configuration);

        // base 35, and the ancient part of the shield profile: 2 + 35 * 20 / (40 + bonus)
        Assert.That(GetPowerUps(shield, configuration)[Stats.DefenseShield], Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that a part with the state <see cref="ItemStatTermState.None"/> adds nothing.
    /// </summary>
    [Test]
    public void PartWithoutValuesAddsNothing()
    {
        var configuration = TestItemStatProfiles.Create();
        GetTerms(GetDefault(configuration), Stats.DefenseBase).ExcellentState = ItemStatTermState.None;
        var armor = MakeExcellent(CreateItem(group: 7, slot: InventoryConstants.HelmSlot, (Stats.DefenseBase, 30)));

        Assert.That(GetPowerUps(armor, configuration)[Stats.DefenseBase], Is.EqualTo(30));
    }

    /// <summary>
    /// Tests that a drop level divisor of 0 leaves the drop level part out.
    /// </summary>
    [Test]
    public void DivisorOf0LeavesTheDropLevelOut()
    {
        var configuration = TestItemStatProfiles.Create();
        GetTerms(GetDefault(configuration), Stats.DefenseBase).ExcellentDropLevelDivisor = 0;
        var armor = MakeExcellent(CreateItem(group: 7, slot: InventoryConstants.HelmSlot, (Stats.DefenseBase, 30)));

        // 30 + 30 * 12 / 40 + 4, without the 40 / 5 of the built-in default
        Assert.That(GetPowerUps(armor, configuration)[Stats.DefenseBase], Is.EqualTo(30 + 9 + 4));
    }

    /// <summary>
    /// Tests that an ancient part with a drop level of 0 leaves out the value instead of dividing by 0, as in the game client.
    /// </summary>
    [Test]
    public void AncientDropLevelOf0LeavesOutTheValue()
    {
        var configuration = TestItemStatProfiles.Create();
        configuration.AncientDropLevelBonus = 0;
        var armor = MakeAncient(CreateItem(group: 7, slot: InventoryConstants.HelmSlot, (Stats.DefenseBase, 30)));
        armor.Definition!.DropLevel = 0;

        // the ancient part of the default: 2 + 0 / 30, without the value * 3 / 0
        Assert.That(GetPowerUps(armor, configuration)[Stats.DefenseBase], Is.EqualTo(30 + 2));
    }

    /// <summary>
    /// Tests that an excellent item with a drop level of 0 gets no excellent part, as in the game client.
    /// </summary>
    [Test]
    public void ExcellentItemWithDropLevel0GetsNoExcellentPart()
    {
        var configuration = TestItemStatProfiles.Create();
        var armor = MakeExcellent(CreateItem(group: 7, slot: InventoryConstants.HelmSlot, (Stats.DefenseBase, 30)));
        armor.Definition!.DropLevel = 0;

        Assert.That(GetPowerUps(armor, configuration)[Stats.DefenseBase], Is.EqualTo(30));
    }

    /// <summary>
    /// Tests that terms of an attribute which the item formulas don't compute today are kept, but not used.
    /// </summary>
    [Test]
    public void TermsOfOtherAttributesAreNotUsed()
    {
        var configuration = TestItemStatProfiles.Create();
        GetDefault(configuration).StatTerms.Add(new BasicModel.ItemStatTerms
        {
            Attribute = Stats.MaximumHealth,
            ExcellentState = ItemStatTermState.Values,
            ExcellentTimes = 25,
            ExcellentAdd = 5,
        });
        var armor = MakeExcellent(CreateItem(group: 7, slot: InventoryConstants.HelmSlot, (Stats.DefenseBase, 30), (Stats.MaximumHealth, 100)));

        Assert.That(GetPowerUps(armor, configuration)[Stats.MaximumHealth], Is.EqualTo(100));
    }

    private static ItemStatProfile GetDefault(GameConfiguration configuration)
    {
        return configuration.ItemStatProfiles.Single(profile => profile.Name == ItemStatProfile.DefaultName);
    }

    private static ItemRequirementFormula GetFormula(ItemStatProfile profile, AttributeDefinition attribute)
    {
        return profile.RequirementFormulas.Single(formula => formula.Attribute == attribute);
    }

    private static ItemStatTerms GetTerms(ItemStatProfile profile, AttributeDefinition attribute)
    {
        return profile.StatTerms.Single(terms => terms.Attribute == attribute);
    }

    private static AttributeRequirement Requirement(AttributeDefinition attribute, int value)
    {
        return new BasicModel.AttributeRequirement { Attribute = attribute, MinimumValue = value };
    }

    private static Item CreateArmor(byte level = 0)
    {
        var item = CreateItem(group: 7, slot: InventoryConstants.HelmSlot);
        item.Level = level;
        return item;
    }

    private static Item CreateItem(byte group, byte slot, params (AttributeDefinition Attribute, int Value)[] powerUps)
    {
        var slotType = new BasicModel.ItemSlotType();
        slotType.ItemSlots.Add(slot);
        var definition = new BasicModel.ItemDefinition { Group = group, DropLevel = DropLevel, Durability = 100, ItemSlot = slotType };
        foreach (var (attribute, value) in powerUps)
        {
            definition.BasePowerUpAttributes.Add(new BasicModel.ItemBasePowerUpDefinition { TargetAttribute = attribute, BaseValue = value, AggregateType = AggregateType.AddRaw });
        }

        return new BasicModel.Item { Definition = definition, ItemSlot = slot, Durability = 255 };
    }

    private static Item MakeExcellent(Item item)
    {
        item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.Excellent } });
        return item;
    }

    private static Item MakeAncient(Item item)
    {
        item.ItemSetGroups.Add(new BasicModel.ItemOfItemSet { AncientSetDiscriminator = 1 });
        return item;
    }

    private static AttributeSystem GetPowerUps(Item item, GameConfiguration configuration)
    {
        var attributes = new AttributeSystem(Enumerable.Empty<IAttribute>(), Enumerable.Empty<IAttribute>(), Enumerable.Empty<AttributeRelationship>());
        _ = new ItemPowerUpFactory(NullLogger<ItemPowerUpFactory>.Instance, configuration).GetPowerUps(item, attributes).ToList();
        return attributes;
    }
}
