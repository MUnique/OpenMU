// <copyright file="FixMurenRingOfFireBonusTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the Ring of Fire of the Muren ancient set has its energy bonus in the season 6 data,
/// and that the update gives it to existing databases.
/// </summary>
[TestFixture]
internal class FixMurenRingOfFireBonusTest
{
    private const byte RingOfFireGroup = 13;
    private const byte RingOfFireNumber = 21;
    private const byte StormCrowNumber = 15;

    /// <summary>
    /// Tests that a new season 6 database has the energy bonus for the Ring of Fire of both its sets,
    /// and still the vitality bonus for the Storm Crow pieces of the Muren set.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasEnergyBonusForMurenRingOfFireAsync()
    {
        var (_, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        Assert.That(GetBonusAttribute(gameConfiguration, "Muren", RingOfFireGroup, RingOfFireNumber), Is.EqualTo(Stats.TotalEnergy));
        Assert.That(GetBonusAttribute(gameConfiguration, "Anubis", RingOfFireGroup, RingOfFireNumber), Is.EqualTo(Stats.TotalEnergy));
        Assert.That(GetBonusAttribute(gameConfiguration, "Muren", (byte)ItemGroups.Armor, StormCrowNumber), Is.EqualTo(Stats.TotalVitality));
    }

    /// <summary>
    /// Tests that the update gives the energy bonus to the Ring of Fire of the Muren set on a database
    /// which was created with the vitality bonus, that it leaves the other pieces alone,
    /// and that applying it twice keeps the result.
    /// </summary>
    [Test]
    public async Task UpdateGivesEnergyBonusToMurenRingOfFireAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var muren = gameConfiguration.ItemSetGroups.Single(set => set.Name.ValueInNeutralLanguage == "Muren");
        var armor = GetItemOfSet(muren, (byte)ItemGroups.Armor, StormCrowNumber);
        GetItemOfSet(muren, RingOfFireGroup, RingOfFireNumber).BonusOption = armor.BonusOption;

        var update = new FixMurenRingOfFireBonusPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(GetBonusAttribute(gameConfiguration, "Muren", RingOfFireGroup, RingOfFireNumber), Is.EqualTo(Stats.TotalEnergy));
        Assert.That(armor.BonusOption?.PowerUpDefinition?.TargetAttribute, Is.EqualTo(Stats.TotalVitality));
    }

    private static AttributeDefinition? GetBonusAttribute(GameConfiguration gameConfiguration, string setName, byte group, byte number)
    {
        var set = gameConfiguration.ItemSetGroups.Single(s => s.Name.ValueInNeutralLanguage == setName);
        return GetItemOfSet(set, group, number).BonusOption?.PowerUpDefinition?.TargetAttribute;
    }

    private static ItemOfItemSet GetItemOfSet(ItemSetGroup set, byte group, byte number)
    {
        return set.Items.Single(item => item.ItemDefinition?.Group == group && item.ItemDefinition?.Number == number);
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
