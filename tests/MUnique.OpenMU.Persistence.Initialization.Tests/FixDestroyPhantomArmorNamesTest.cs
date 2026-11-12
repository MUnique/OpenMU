// <copyright file="FixDestroyPhantomArmorNamesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the armor sets 47 (Phantom) and 48 (Destroy) have their names in the season 6 data,
/// and that the update gives them to existing databases.
/// </summary>
[TestFixture]
internal class FixDestroyPhantomArmorNamesTest
{
    private const byte PhantomNumber = 47;
    private const byte DestroyNumber = 48;

    private static readonly string[] PieceNames = ["Armor", "Pants", "Gloves", "Boots"];

    private static readonly byte[] PieceGroups = [(byte)ItemGroups.Armor, (byte)ItemGroups.Pants, (byte)ItemGroups.Gloves, (byte)ItemGroups.Boots];

    /// <summary>
    /// Tests that a new season 6 database has the names of the client for the pieces and the set bonuses
    /// of both sets, linked to their sources.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasPhantomAndDestroyNamesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertNames(gameConfiguration);
        Assert.That(GetPiece(gameConfiguration, (byte)ItemGroups.Armor, PhantomNumber).Name.SourceKey, Is.EqualTo("ItemNames/PhantomArmor"));
        Assert.That(GetPiece(gameConfiguration, (byte)ItemGroups.Armor, DestroyNumber).Name.SourceKey, Is.EqualTo("ItemNames/DestroyArmor"));
        Assert.That(GetSetBonuses(gameConfiguration, DestroyNumber).Select(set => set.Name.SourceKey), Has.All.StartWith("ArmorSetNames/DestroyArmor"));
    }

    /// <summary>
    /// Tests that the update renames the pieces and set bonuses of a database which was created with the
    /// swapped names, that it keeps a name which was changed in the configuration,
    /// and that applying it twice keeps the result.
    /// </summary>
    [Test]
    public async Task UpdateRenamesPhantomAndDestroyAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        SetPreviousNames(gameConfiguration);
        var customizedBoots = GetPiece(gameConfiguration, (byte)ItemGroups.Boots, DestroyNumber);
        customizedBoots.Name = new LocalizedString("My Boots");

        var update = new FixDestroyPhantomArmorNamesPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(customizedBoots.Name.ValueInNeutralLanguage, Is.EqualTo("My Boots"));
        customizedBoots.Name = LocalizedString.FromResource(() => Properties.ItemNames.DestroyBoots);
        AssertNames(gameConfiguration);
        Assert.That(GetPiece(gameConfiguration, (byte)ItemGroups.Armor, PhantomNumber).Name.SourceKey, Is.EqualTo("ItemNames/PhantomArmor"));
    }

    private static void AssertNames(GameConfiguration gameConfiguration)
    {
        for (var i = 0; i < PieceGroups.Length; i++)
        {
            Assert.That(GetPiece(gameConfiguration, PieceGroups[i], PhantomNumber).Name.ValueInNeutralLanguage, Is.EqualTo("Phantom " + PieceNames[i]));
            Assert.That(GetPiece(gameConfiguration, PieceGroups[i], DestroyNumber).Name.ValueInNeutralLanguage, Is.EqualTo("Destroy " + PieceNames[i]));
        }

        Assert.That(GetSetBonuses(gameConfiguration, PhantomNumber).Select(set => set.Name.ValueInNeutralLanguage), Is.EquivalentTo(GetSetBonusNames("Phantom")));
        Assert.That(GetSetBonuses(gameConfiguration, DestroyNumber).Select(set => set.Name.ValueInNeutralLanguage), Is.EquivalentTo(GetSetBonusNames("Destroy")));
        Assert.That(gameConfiguration.Items.Select(item => item.Name.ValueInNeutralLanguage), Has.None.Contains("Destory"));
        Assert.That(gameConfiguration.ItemSetGroups.Select(set => set.Name.ValueInNeutralLanguage), Has.None.Contains("Destory"));
    }

    private static void SetPreviousNames(GameConfiguration gameConfiguration)
    {
        string[] previousPhantomNames = ["Destory Armor", "Destory Pants", "Destroy Gloves", "Destory Boots"];
        for (var i = 0; i < PieceGroups.Length; i++)
        {
            GetPiece(gameConfiguration, PieceGroups[i], PhantomNumber).Name = new LocalizedString(previousPhantomNames[i]);
            GetPiece(gameConfiguration, PieceGroups[i], DestroyNumber).Name = new LocalizedString("Phantom " + PieceNames[i]);
        }

        SetPreviousSetBonusNames(gameConfiguration, PhantomNumber, "Destory");
        SetPreviousSetBonusNames(gameConfiguration, DestroyNumber, "Phantom");
    }

    private static void SetPreviousSetBonusNames(GameConfiguration gameConfiguration, byte number, string previousSetName)
    {
        foreach (var set in GetSetBonuses(gameConfiguration, number))
        {
            set.Name = new LocalizedString(set.SetLevel > 0
                ? $"{previousSetName} Defense Bonus (Level {set.SetLevel})"
                : previousSetName + " Defense Rate Bonus");
        }
    }

    private static IEnumerable<string> GetSetBonusNames(string setName)
    {
        yield return setName + " Defense Rate Bonus";
        for (var level = 10; level <= 15; level++)
        {
            yield return $"{setName} Defense Bonus (Level {level})";
        }
    }

    private static ItemDefinition GetPiece(GameConfiguration gameConfiguration, byte group, byte number)
    {
        return gameConfiguration.Items.Single(item => item.Group == group && item.Number == number);
    }

    private static List<ItemSetGroup> GetSetBonuses(GameConfiguration gameConfiguration, byte number)
    {
        return gameConfiguration.ItemSetGroups
            .Where(set => set.Items.Count > 0 && set.Items.All(item => item.ItemDefinition?.Number == number && item.ItemDefinition.Group is >= (byte)ItemGroups.Helm and <= (byte)ItemGroups.Boots))
            .ToList();
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
