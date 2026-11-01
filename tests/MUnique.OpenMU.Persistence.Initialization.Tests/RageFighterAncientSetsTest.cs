// <copyright file="RageFighterAncientSetsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the ancient sets of the rage fighter's Sacred items, Vega and Chamer, as the season 6 client defines them.
/// </summary>
[TestFixture]
internal class RageFighterAncientSetsTest
{
    /// <summary>
    /// Tests that a new season 6 database contains both sets with the client's items, discriminators and options.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsRageFighterSetsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertSets(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the sets to a database created without them, reusing the existing
    /// ancient bonus options, and that applying it twice doesn't add them twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsRageFighterSetsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var set in gameConfiguration.ItemSetGroups.Where(IsRageFighterSet).ToList())
        {
            gameConfiguration.ItemSetGroups.Remove(set);
            gameConfiguration.ItemOptions.Remove(set.Options!);
            foreach (var item in gameConfiguration.Items)
            {
                item.PossibleItemSetGroups.Remove(set);
            }
        }

        var optionCount = gameConfiguration.ItemOptions.Count;

        var update = new AddRageFighterAncientSetsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.ItemOptions, Has.Count.EqualTo(optionCount + 2), "only the two sets' option definitions are new");
        AssertSets(gameConfiguration);
    }

    private static bool IsRageFighterSet(ItemSetGroup set) => set.Name.ValueInNeutralLanguage is "Vega" or "Chamer";

    private static void AssertSets(GameConfiguration gameConfiguration)
    {
        var vega = gameConfiguration.ItemSetGroups.Single(set => set.Name.ValueInNeutralLanguage == "Vega");
        var chamer = gameConfiguration.ItemSetGroups.Single(set => set.Name.ValueInNeutralLanguage == "Chamer");

        Assert.Multiple(() =>
        {
            Assert.That(Members(vega), Is.EquivalentTo(new[] { (0, 32, 1), (7, 59, 1), (8, 59, 1), (9, 59, 1) }));
            Assert.That(Members(chamer), Is.EquivalentTo(new[] { (0, 32, 2), (8, 59, 2), (9, 59, 2), (11, 59, 1) }));
            Assert.That(vega.Options!.PossibleOptions.Select(o => o.PowerUpDefinition!.TargetAttribute), Is.EqualTo(new[] { Stats.AttackRatePvm, Stats.TotalVitality, Stats.MaximumPhysBaseDmg, Stats.ExcellentDamageChance, Stats.DoubleDamageChance, Stats.DefenseIgnoreChance }));
            Assert.That(chamer.Options!.PossibleOptions, Has.Count.EqualTo(7));
            Assert.That(vega.Items.Single(i => i.ItemDefinition!.Group == 7).BonusOption!.PowerUpDefinition!.TargetAttribute, Is.EqualTo(Stats.TotalAgility));
            Assert.That(gameConfiguration.Items.Single(i => i is { Group: 8, Number: 59 }).PossibleItemSetGroups, Does.Contain(vega).And.Contain(chamer));
        });
    }

    private static IEnumerable<(int Group, int Number, int Discriminator)> Members(ItemSetGroup set)
        => set.Items.Select(i => ((int)i.ItemDefinition!.Group, (int)i.ItemDefinition.Number, i.AncientSetDiscriminator));

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
