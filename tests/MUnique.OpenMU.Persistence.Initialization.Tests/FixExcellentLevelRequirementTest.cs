// <copyright file="FixExcellentLevelRequirementTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the excellent version of the weapons, shields and armor pieces and of the Horn of Fenrir needs the same
/// level as the normal one in the season 6 data, and that the update gives this to existing databases.
/// </summary>
[TestFixture]
internal class FixExcellentLevelRequirementTest
{
    /// <summary>
    /// Items whose excellent version needs level 380 like the normal one: the Dragon Knight Armor, which the client
    /// also shows at 380, and the Phoenix Soul Armor and the socket weapon Flameberge, which it shows at 400.
    /// </summary>
    private static readonly (byte Group, short Number)[] ItemsOfLevel380 = [(8, 29), (8, 73), (0, 26)];

    /// <summary>
    /// Tests that in a new season 6 database exactly the weapons, shields and armor pieces with a level requirement,
    /// the Horn of Fenrir and the lucky items don't raise the level requirement of their excellent version, so the
    /// excellent version of the items of level 380 needs 380, also when excellent items need 20 levels more, as in the
    /// original game.
    /// </summary>
    [Test]
    public async Task NewDatabaseKeepsTheLevelOfWeaponsAndArmorAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        var withoutRaise = gameConfiguration.Items.Where(item => !item.ExcellentRaisesLevel && !LuckyItems.IsLuckyItem(item)).ToList();
        Assert.That(withoutRaise, Has.All.Matches<ItemDefinition>(item => item.Group <= 11 || (item.Group == 13 && item.Number == 37)));
        Assert.That(withoutRaise.Select(item => (item.Group, item.Number)), Does.Contain(((byte)13, (short)37)).And.SupersetOf(ItemsOfLevel380));
        Assert.That(
            gameConfiguration.Items.Where(item => item.Group <= 11 && HasLevelRequirement(item)),
            Has.None.Matches<ItemDefinition>(item => item.ExcellentRaisesLevel));
        Assert.That(
            gameConfiguration.Items.Where(item => item.Group == 12 && HasLevelRequirement(item)),
            Has.Some.Matches<ItemDefinition>(item => item.ExcellentRaisesLevel));
        RaiseExcellentLevelAsInTheOriginalGame(gameConfiguration);
        Assert.That(ItemsOfLevel380.Select(item => GetLevelRequirement(context, gameConfiguration, item.Group, item.Number, excellent: true)), Has.All.EqualTo(380));
    }

    /// <summary>
    /// Tests that the update keeps the level of the excellent version on a database where it was raised,
    /// that it leaves the other items alone, and that applying it twice keeps the result.
    /// </summary>
    [Test]
    public async Task UpdateKeepsTheLevelOfWeaponsAndArmorAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var items = gameConfiguration.Items.Where(item => !item.ExcellentRaisesLevel && !LuckyItems.IsLuckyItem(item)).ToList();
        items.ForEach(item => item.ExcellentRaisesLevel = true);
        var others = gameConfiguration.Items.Except(items).ToDictionary(item => item, item => item.ExcellentRaisesLevel);
        RaiseExcellentLevelAsInTheOriginalGame(gameConfiguration);
        Assert.That(ItemsOfLevel380.Select(item => GetLevelRequirement(context, gameConfiguration, item.Group, item.Number, excellent: true)), Has.All.EqualTo(400));

        var update = new FixExcellentLevelRequirementPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(items, Has.None.Matches<ItemDefinition>(item => item.ExcellentRaisesLevel));
        Assert.That(ItemsOfLevel380.Select(item => GetLevelRequirement(context, gameConfiguration, item.Group, item.Number, excellent: true)), Has.All.EqualTo(380));
        Assert.That(ItemsOfLevel380.Select(item => GetLevelRequirement(context, gameConfiguration, item.Group, item.Number, excellent: false)), Has.All.EqualTo(380));
        Assert.That(others.Where(pair => pair.Key.ExcellentRaisesLevel != pair.Value), Is.Empty);
    }

    /// <summary>
    /// Lets an excellent item need 20 levels more, as in the original game and the client. The season 6 data adds no
    /// level, so <see cref="ItemDefinition.ExcellentRaisesLevel"/> has no effect without this.
    /// </summary>
    private static void RaiseExcellentLevelAsInTheOriginalGame(GameConfiguration gameConfiguration)
    {
        gameConfiguration.ItemStatProfiles.Single(profile => profile.Name == ItemStatProfile.DefaultName).ExcellentLevelAdd = 20;
    }

    private static bool HasLevelRequirement(ItemDefinition item)
    {
        return item.Requirements.Any(requirement => requirement.Attribute == Stats.Level && requirement.MinimumValue > 0);
    }

    private static int GetLevelRequirement(IContext context, GameConfiguration gameConfiguration, byte group, short number, bool excellent)
    {
        var item = context.CreateNew<Item>();
        item.Definition = gameConfiguration.Items.Single(definition => definition.Group == group && definition.Number == number);
        if (excellent)
        {
            var link = context.CreateNew<ItemOptionLink>();
            link.ItemOption = gameConfiguration.ItemOptions
                .SelectMany(definition => definition.PossibleOptions)
                .First(option => option.OptionType == ItemOptionTypes.Excellent);
            item.ItemOptions.Add(link);
        }

        return item.GetRequirements(gameConfiguration).Single(requirement => requirement.Attr == Stats.Level).Value;
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
