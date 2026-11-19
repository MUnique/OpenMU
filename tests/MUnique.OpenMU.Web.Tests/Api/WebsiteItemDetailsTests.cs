// <copyright file="WebsiteItemDetailsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Web.AdminPanel.Operations.Website;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="WebsiteItemDetails"/>, which builds the picture name and the structured options of an item for a website.
/// </summary>
[TestFixture]
public class WebsiteItemDetailsTests
{
    /// <summary>
    /// An excellent +13 sword with luck, a +16 option, an excellent option and the skill gets its picture with the
    /// glow of +13 and its options ordered by type.
    /// </summary>
    [Test]
    public void ExcellentItemIsDescribed()
    {
        var item = CreateItem(slot: 0, level: 13);
        AddOption(item, ItemOptionTypes.Luck, "Critical Damage Chance", 5, 0);
        AddOption(item, ItemOptionTypes.Option, "Physical Damage", 16, 4);
        AddOption(item, ItemOptionTypes.Excellent, "Excellent Damage Chance", 0.1f, 0);
        item.HasSkill = true;

        Assert.That(WebsiteItemDetails.GetPictureName(item), Is.EqualTo("item_0_16_13_e.png"));
        Assert.That(WebsiteItemDetails.GetOptions(item).Select(option => option.Type), Is.EqualTo(new[] { ItemOptionTypes.Excellent, ItemOptionTypes.Luck, ItemOptionTypes.Option }.Select(type => type.Name.ValueInNeutralLanguage)));
    }

    /// <summary>
    /// Wings and pets keep the base picture, whatever their level is.
    /// </summary>
    [Test]
    public void WingsKeepTheBasePicture()
    {
        Assert.That(WebsiteItemDetails.GetPictureName(CreateItem(slot: 7, level: 11)), Is.EqualTo("item_0_16_0.png"));
        Assert.That(WebsiteItemDetails.GetPictureName(CreateItem(slot: 0, level: 4)), Is.EqualTo("item_0_16_3.png"));
    }

    /// <summary>
    /// The structured options contain type, number, level, attribute, value and aggregate, ordered by type and number.
    /// </summary>
    [Test]
    public void OptionsAreStructured()
    {
        var item = CreateItem(slot: 0, level: 9);
        AddOption(item, ItemOptionTypes.Option, "Physical Damage", 16, 4);
        AddOption(item, ItemOptionTypes.Excellent, "Excellent Damage Chance", 0.1f, 0);
        item.HasSkill = true;

        var options = WebsiteItemDetails.GetOptions(item);

        Assert.That(options, Has.Count.EqualTo(2));
        Assert.That(options[0].Type, Is.EqualTo(ItemOptionTypes.Excellent.Name.ValueInNeutralLanguage));
        Assert.That(options[0].Attribute, Is.EqualTo("Excellent Damage Chance"));
        Assert.That(options[0].Value, Is.EqualTo(0.1f));
        Assert.That(options[0].Level, Is.EqualTo(0));
        Assert.That(options[1].Type, Is.EqualTo(ItemOptionTypes.Option.Name.ValueInNeutralLanguage));
        Assert.That(options[1].Level, Is.EqualTo(4));
        Assert.That(options[1].Attribute, Is.EqualTo("Physical Damage"));
        Assert.That(options[1].Value, Is.EqualTo(16f));
        Assert.That(options[1].Aggregate, Is.EqualTo("AddRaw"));
        Assert.That(options[1].RelatedAttribute, Is.Null);
        Assert.That(options[1].Number, Is.EqualTo(0));
        Assert.That(WebsiteItemDetails.GetAncientSetName(item), Is.Null);
    }

    /// <summary>
    /// An option with <see cref="LevelType.ItemLevel"/> takes the value of the item level, not of the link level.
    /// </summary>
    [Test]
    public void ItemLevelOptionIsResolvedByItemLevel()
    {
        var item = CreateItem(slot: 7, level: 13);
        var option = new BasicModel.IncreasableItemOption { OptionType = ItemOptionTypes.Wing, LevelType = LevelType.ItemLevel };
        foreach (var (level, value) in new[] { (1, 55f), (13, 115f) })
        {
            var powerUp = new BasicModel.PowerUpDefinition
            {
                TargetAttribute = new AttributeDefinition(Guid.NewGuid(), "Maximum Health", string.Empty),
                Boost = new BasicModel.PowerUpDefinitionValue(),
            };
            powerUp.Boost.ConstantValue.Value = value;
            option.LevelDependentOptions.Add(new BasicModel.ItemOptionOfLevel { Level = level, PowerUpDefinition = powerUp });
        }

        item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = option, Level = 1 });

        var result = WebsiteItemDetails.GetOptions(item);

        Assert.That(result[0].Value, Is.EqualTo(115f));
        Assert.That(result[0].Level, Is.EqualTo(1));
    }

    private static BasicModel.Item CreateItem(byte slot, byte level)
    {
        var definition = new BasicModel.ItemDefinition { Group = 0, Number = 16, Name = "Sword" };
        return new BasicModel.Item { Definition = definition, ItemSlot = slot, Level = level };
    }

    private static void AddOption(BasicModel.Item item, ItemOptionType type, string target, float value, int level)
    {
        var powerUp = new BasicModel.PowerUpDefinition
        {
            TargetAttribute = new AttributeDefinition(Guid.NewGuid(), target, string.Empty),
            Boost = new BasicModel.PowerUpDefinitionValue(),
        };
        powerUp.Boost.ConstantValue.Value = value;
        var option = new BasicModel.IncreasableItemOption { OptionType = type };
        if (level > 0)
        {
            option.LevelDependentOptions.Add(new BasicModel.ItemOptionOfLevel { Level = level, PowerUpDefinition = powerUp });
        }
        else
        {
            option.PowerUpDefinition = powerUp;
        }

        item.ItemOptions.Add(new BasicModel.ItemOptionLink { ItemOption = option, Level = level });
    }
}
