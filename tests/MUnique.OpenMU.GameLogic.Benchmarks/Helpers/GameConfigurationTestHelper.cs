// <copyright file="GameConfigurationTestHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Benchmarks.Helpers;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Helper for creating test game configurations.
/// </summary>
public static class GameConfigurationTestHelper
{
    /// <summary>
    /// Creates a mock game configuration for drop generation testing.
    /// </summary>
    /// <returns>A mock game configuration.</returns>
    public static GameConfiguration Create()
    {
        var gameConfiguration = new Mock<GameConfiguration>();
        gameConfiguration.SetupAllProperties();
        gameConfiguration.Object.ExcellentItemDropLevelDelta = 50;
        gameConfiguration.Object.MaximumItemOptionLevelDrop = 3;

        var items = CreateItems();
        gameConfiguration.Setup(c => c.Items).Returns(items);
        return gameConfiguration.Object;
    }

    private static IList<ItemDefinition> CreateItems()
    {
        var items = new List<ItemDefinition>();
        var random = new Random(42);

        for (byte dropLevel = 0; dropLevel <= 200; dropLevel++)
        {
            int itemsAtThisLevel = random.Next(5, 20);
            for (int i = 0; i < itemsAtThisLevel; i++)
            {
                var item = new Mock<ItemDefinition>();
                item.SetupAllProperties();
                item.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
                item.Setup(d => d.PossibleItemOptions).Returns(CreateItemOptions(dropLevel));
                item.Setup(d => d.Requirements).Returns(new List<AttributeRequirement>());
                item.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
                item.Setup(d => d.DropItems).Returns(new List<ItemDropItemGroup>());
                item.Setup(d => d.QualifiedCharacters).Returns(new List<CharacterClass>());

                item.Object.DropsFromMonsters = true;
                item.Object.DropLevel = dropLevel;
                item.Object.Width = (byte)(i % 4 + 1);
                item.Object.Height = (byte)(i % 4 + 1);
                item.Object.MaximumItemLevel = 13;
                item.Object.MaximumSockets = dropLevel > 100 ? 5 : 0;
                item.Object.Group = (byte)(dropLevel % 16);
                item.Object.Number = (short)(i % 256);

                items.Add(item.Object);
            }
        }

        return items;
    }

    private static IList<ItemOptionDefinition> CreateItemOptions(byte dropLevel)
    {
        var options = new List<ItemOptionDefinition>();

        if (dropLevel > 30)
        {
            var excellentOption = new Mock<ItemOptionDefinition>();
            excellentOption.SetupAllProperties();
            excellentOption.Setup(o => o.PossibleOptions).Returns(CreateExcellentOptions());
            excellentOption.Object.AddsRandomly = true;
            excellentOption.Object.AddChance = 100;
            excellentOption.Object.MaximumOptionsPerItem = 6;
            options.Add(excellentOption.Object);
        }

        if (dropLevel > 50)
        {
            var luckOption = new Mock<ItemOptionDefinition>();
            luckOption.SetupAllProperties();
            luckOption.Setup(o => o.PossibleOptions).Returns(new List<IncreasableItemOption>());
            luckOption.Object.AddsRandomly = true;
            luckOption.Object.AddChance = 100;
            luckOption.Object.MaximumOptionsPerItem = 1;
            options.Add(luckOption.Object);
        }

        return options;
    }

    private static IList<IncreasableItemOption> CreateExcellentOptions()
    {
        var options = new List<IncreasableItemOption>();
        for (int i = 0; i < 6; i++)
        {
            var option = new Mock<IncreasableItemOption>();
            option.SetupAllProperties();
            option.Setup(o => o.LevelDependentOptions).Returns(new List<ItemOptionOfLevel>());
            options.Add(option.Object);
        }

        return options;
    }
}