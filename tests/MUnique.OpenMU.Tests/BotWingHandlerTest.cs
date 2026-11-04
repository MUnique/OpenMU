// <copyright file="BotWingHandlerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests the wing milestone policy of <see cref="BotWingHandler"/> - which wings a bot has earned
/// at which level; the creation and equipping go through the regular persistence context and
/// <see cref="MUnique.OpenMU.GameLogic.PlayerActions.Items.MoveItemAction"/>.
/// </summary>
[TestFixture]
public class BotWingHandlerTest
{
    private const short FirstTierWingNumber = 2;
    private const short SecondTierWingNumber = 5;
    private const short ThirdTierWingNumber = 36;

    /// <summary>The Cape of Lord lives in group 13, unlike all other wings (group 12).</summary>
    private const short CapeNumber = 30;
    private const byte CapeGroup = 13;

    /// <summary>
    /// Below the first milestone no wings are due.
    /// </summary>
    [Test]
    public async ValueTask PlansNothingBelowFirstMilestoneAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddWingDefinition(player, FirstTierWingNumber, Stats.PhysicalBaseDmg);
        player.Attributes![Stats.Level] = 179;

        var plan = BotWingHandler.PlanNextGrant(player);

        Assert.That(plan, Is.Null);
    }

    /// <summary>
    /// At the milestones the earned tier is granted with the agreed item level and option level.
    /// </summary>
    /// <param name="level">The character level.</param>
    /// <param name="expectedNumber">The expected wing number.</param>
    /// <param name="expectedItemLevel">The expected item level of the grant.</param>
    /// <param name="expectedOptionLevel">The expected level of the wing option.</param>
    [TestCase(180, FirstTierWingNumber, 0, 3)]
    [TestCase(280, SecondTierWingNumber, 9, 4)]
    [TestCase(400, ThirdTierWingNumber, 15, 4)]
    public async ValueTask GrantsEarnedTierAtMilestoneAsync(int level, short expectedNumber, byte expectedItemLevel, int expectedOptionLevel)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddWingDefinition(player, FirstTierWingNumber, Stats.PhysicalBaseDmg);
        AddWingDefinition(player, SecondTierWingNumber, Stats.PhysicalBaseDmg);
        AddWingDefinition(player, ThirdTierWingNumber, Stats.PhysicalBaseDmg);
        player.Attributes![Stats.Level] = level;

        var plan = BotWingHandler.PlanNextGrant(player);

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.Value.Definition.Number, Is.EqualTo(expectedNumber));
        Assert.That(plan.Value.ItemLevel, Is.EqualTo(expectedItemLevel));
        Assert.That(plan.Value.OptionLevel, Is.EqualTo(expectedOptionLevel));
    }

    /// <summary>
    /// A bot re-levelling through the lower milestones after a reset keeps its better wings.
    /// </summary>
    [Test]
    public async ValueTask NeverDowngradesWornWingsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddWingDefinition(player, FirstTierWingNumber, Stats.PhysicalBaseDmg);
        var secondTier = AddWingDefinition(player, SecondTierWingNumber, Stats.PhysicalBaseDmg);
        await WearWingsAsync(player, secondTier, 9).ConfigureAwait(false);
        player.Attributes![Stats.Level] = 200;

        var plan = BotWingHandler.PlanNextGrant(player);

        Assert.That(plan, Is.Null);
    }

    /// <summary>
    /// Wearing the earned tier already: nothing to do.
    /// </summary>
    [Test]
    public async ValueTask PlansNothingWhenEarnedTierIsWornAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var secondTier = AddWingDefinition(player, SecondTierWingNumber, Stats.PhysicalBaseDmg);
        await WearWingsAsync(player, secondTier, 9).ConfigureAwait(false);
        player.Attributes![Stats.Level] = 300;

        var plan = BotWingHandler.PlanNextGrant(player);

        Assert.That(plan, Is.Null);
    }

    /// <summary>
    /// A bot which did not evolve into its master class yet is not qualified for the third tier
    /// wings and falls back to the best qualified tier.
    /// </summary>
    [Test]
    public async ValueTask FallsBackWhenThirdTierIsNotQualifiedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddWingDefinition(player, SecondTierWingNumber, Stats.PhysicalBaseDmg);
        var thirdTier = AddWingDefinition(player, ThirdTierWingNumber, Stats.PhysicalBaseDmg);
        thirdTier.QualifiedCharacters.Clear();
        player.Attributes![Stats.Level] = 400;

        var plan = BotWingHandler.PlanNextGrant(player);

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.Value.Definition.Number, Is.EqualTo(SecondTierWingNumber));
        Assert.That(plan.Value.Tier, Is.EqualTo(2));
    }

    /// <summary>
    /// The capes are the only pre-master wing of their classes: granted at the first milestone at +0
    /// and re-granted as a fresh +9 cape at the second one.
    /// </summary>
    /// <param name="wornCapeLevel">The item level of the worn cape.</param>
    /// <param name="expectsGrant">Whether a new cape is expected.</param>
    [TestCase(0, true)]
    [TestCase(9, false)]
    public async ValueTask RegrantsCapeAtSecondMilestoneAsync(byte wornCapeLevel, bool expectsGrant)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var cape = AddWingDefinition(player, CapeNumber, Stats.PhysicalBaseDmg, CapeGroup);
        await WearWingsAsync(player, cape, wornCapeLevel).ConfigureAwait(false);
        player.Attributes![Stats.Level] = 280;

        var plan = BotWingHandler.PlanNextGrant(player);

        Assert.That(plan.HasValue, Is.EqualTo(expectsGrant));
        if (expectsGrant)
        {
            Assert.That(plan!.Value.Definition, Is.SameAs(cape));
            Assert.That(plan.Value.ItemLevel, Is.EqualTo(9));
        }
    }

    /// <summary>
    /// When a class qualifies for more than one pair (the Magic Gladiator may wear both Wings of
    /// Heaven and Satan), the pair whose option matches the fighting style wins.
    /// </summary>
    /// <param name="baseEnergy">The bot's base energy (base strength is 28).</param>
    /// <param name="expectedNumber">The expected wing number.</param>
    [TestCase(200, 1)]
    [TestCase(0, 2)]
    public async ValueTask PrefersWingsMatchingFightingStyleAsync(int baseEnergy, short expectedNumber)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddWingDefinition(player, 1, Stats.WizardryBaseDmg);
        AddWingDefinition(player, 2, Stats.PhysicalBaseDmg);
        player.Attributes![Stats.BaseEnergy] = baseEnergy;
        player.Attributes[Stats.Level] = 180;

        var plan = BotWingHandler.PlanNextGrant(player);

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.Value.Definition.Number, Is.EqualTo(expectedNumber));
    }

    private static ItemDefinition AddWingDefinition(Player player, short number, AttributeDefinition optionTarget, byte group = 12)
    {
        var definitionMock = new Mock<ItemDefinition>();
        definitionMock.SetupAllProperties();
        definitionMock.Setup(d => d.QualifiedCharacters).Returns(new List<CharacterClass>());
        definitionMock.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        var slotType = new Mock<ItemSlotType>();
        slotType.Setup(s => s.ItemSlots).Returns(new List<int> { InventoryConstants.WingsSlot });
        definitionMock.Setup(d => d.ItemSlot).Returns(slotType.Object);

        var definition = definitionMock.Object;
        definition.Group = group;
        definition.Number = number;
        definition.Width = 5;
        definition.Height = 3;
        definition.Durability = 200;
        definition.MaximumItemLevel = 15;
        definition.QualifiedCharacters.Add(player.SelectedCharacter!.CharacterClass!);

        var optionDefinitionMock = new Mock<ItemOptionDefinition>();
        optionDefinitionMock.SetupAllProperties();
        optionDefinitionMock.Setup(o => o.PossibleOptions).Returns(new List<IncreasableItemOption>());
        var optionMock = new Mock<IncreasableItemOption>();
        optionMock.SetupAllProperties();
        var powerUpMock = new Mock<PowerUpDefinition>();
        powerUpMock.SetupAllProperties();
        powerUpMock.Object.TargetAttribute = optionTarget;
        optionMock.Object.OptionType = ItemOptionTypes.Option;
        optionMock.Object.PowerUpDefinition = powerUpMock.Object;
        optionDefinitionMock.Object.PossibleOptions.Add(optionMock.Object);
        definition.PossibleItemOptions.Add(optionDefinitionMock.Object);

        player.GameContext.Configuration.Items.Add(definition);
        return definition;
    }

    private static async ValueTask<Item> WearWingsAsync(Player player, ItemDefinition definition, byte level)
    {
        var itemMock = new Mock<Item>();
        itemMock.SetupAllProperties();
        itemMock.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        itemMock.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        var item = itemMock.Object;
        item.Definition = definition;
        item.Level = level;
        item.Durability = definition.Durability;

        await player.Inventory!.AddItemAsync(InventoryConstants.WingsSlot, item).ConfigureAwait(false);
        return item;
    }
}
