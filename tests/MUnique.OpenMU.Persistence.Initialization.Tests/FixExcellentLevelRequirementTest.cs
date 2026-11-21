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
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the excellent version of the 380 items and the Horn of Fenrir needs the same level as the normal one in
/// the season 6 data, and that the update gives this to existing databases.
/// </summary>
[TestFixture]
internal class FixExcellentLevelRequirementTest
{
    private const byte ArmorGroup = 8;
    private const short DragonKnightNumber = 29;

    /// <summary>
    /// The items whose excellent version needs the same level as the normal one, besides the lucky items.
    /// </summary>
    private static readonly (byte Group, short Number)[] ItemsWithoutRaise =
    {
        (0, 22), // Bone Blade
        (0, 23), // Explosion Blade
        (2, 14), // Soleil Scepter
        (4, 21), // Sylph Wind Bow
        (5, 12), // Grand Viper Staff
        (5, 19), // Storm Blitz Stick
        (7, 29), (7, 30), (7, 31), (7, 33), (7, 43), // Dragon Knight, Venom Mist, Sylphid Ray, Sunlight, Storm Blitz helms
        (8, 29), (8, 30), (8, 31), (8, 32), (8, 33), (8, 43), // the armors of these sets and of the Volcano set
        (9, 29), (9, 30), (9, 31), (9, 32), (9, 33), (9, 43), // pants
        (10, 29), (10, 30), (10, 31), (10, 32), (10, 33), (10, 43), // gloves
        (11, 29), (11, 30), (11, 31), (11, 32), (11, 33), (11, 43), // boots
        (13, 37), // Horn of Fenrir
    };

    /// <summary>
    /// Tests that in a new season 6 database exactly these items and the lucky items don't raise the level requirement
    /// of their excellent version, so an excellent Dragon Knight Armor needs level 380 like the normal one, also when
    /// excellent items need 20 levels more, as in the original game.
    /// </summary>
    [Test]
    public async Task NewDatabaseKeepsTheLevelOfThe380ItemsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        var withoutRaise = gameConfiguration.Items
            .Where(item => !item.ExcellentRaisesLevel && !IsLuckyItem(item))
            .Select(item => (item.Group, item.Number));
        Assert.That(withoutRaise, Is.EquivalentTo(ItemsWithoutRaise));
        RaiseExcellentLevelAsInTheOriginalGame(gameConfiguration);
        Assert.That(GetLevelRequirement(context, gameConfiguration, ArmorGroup, DragonKnightNumber, excellent: true), Is.EqualTo(380));
    }

    /// <summary>
    /// Tests that the update keeps the level of the excellent version on a database where it was raised,
    /// that it leaves the other items alone, and that applying it twice keeps the result.
    /// </summary>
    [Test]
    public async Task UpdateKeepsTheLevelOfThe380ItemsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var items = gameConfiguration.Items.Where(item => ItemsWithoutRaise.Contains((item.Group, item.Number))).ToList();
        Assert.That(items, Has.Count.EqualTo(ItemsWithoutRaise.Length));
        items.ForEach(item => item.ExcellentRaisesLevel = true);
        RaiseExcellentLevelAsInTheOriginalGame(gameConfiguration);
        Assert.That(GetLevelRequirement(context, gameConfiguration, ArmorGroup, DragonKnightNumber, excellent: true), Is.EqualTo(400));

        var update = new FixExcellentLevelRequirementPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(items, Has.None.Matches<ItemDefinition>(item => item.ExcellentRaisesLevel));
        Assert.That(GetLevelRequirement(context, gameConfiguration, ArmorGroup, DragonKnightNumber, excellent: true), Is.EqualTo(380));
        Assert.That(GetLevelRequirement(context, gameConfiguration, ArmorGroup, DragonKnightNumber, excellent: false), Is.EqualTo(380));
        Assert.That(
            gameConfiguration.Items.Where(item => !items.Contains(item) && !IsLuckyItem(item)),
            Has.All.Matches<ItemDefinition>(item => item.ExcellentRaisesLevel));
    }

    /// <summary>
    /// Lets an excellent item need 20 levels more, as in the original game and the client. The season 6 data adds no
    /// level, so <see cref="ItemDefinition.ExcellentRaisesLevel"/> has no effect without this.
    /// </summary>
    private static void RaiseExcellentLevelAsInTheOriginalGame(GameConfiguration gameConfiguration)
    {
        gameConfiguration.ItemStatProfiles.Single(profile => profile.Name == ItemStatProfile.DefaultName).ExcellentLevelAdd = 20;
    }

    private static bool IsLuckyItem(ItemDefinition item)
    {
        return item.Group is >= 7 and <= 11 && item.Number is >= 62 and <= 72;
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
