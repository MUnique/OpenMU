// <copyright file="PartyExperienceBonusAttributesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the party experience bonus attributes and their default values are available in the configuration.
/// </summary>
[TestFixture]
internal class PartyExperienceBonusAttributesTest
{
    private static readonly (AttributeDefinition Attribute, float Value)[] ExpectedValues =
    [
        (Stats.PartyExperienceMultiplier, 0.01f),
        (Stats.SetPartyExperienceBonus, 0f),
    ];

    /// <summary>
    /// Tests that a new database contains the attributes with their default values.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task NewDatabaseHasPartyExperienceBonusesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertDefaults(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the attributes and values to a database which was created before they existed,
    /// and that applying it twice doesn't add them twice.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task UpdateAddsPartyExperienceBonusesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        RemovePartyExperienceBonuses(gameConfiguration);

        var update = new AddPartyExperienceBonusAttributesPlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        AssertDefaults(gameConfiguration);

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        AssertDefaults(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update doesn't change values which have been customized by the server owner.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task UpdateKeepsCustomizedValuesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var definition = gameConfiguration.Attributes.Single(a => a.Id == Stats.SetPartyExperienceBonus.Id);
        gameConfiguration.GlobalBaseAttributeValues.Remove(gameConfiguration.GlobalBaseAttributeValues.Single(a => a.Definition?.Id == definition.Id));
        gameConfiguration.GlobalBaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.5f, definition));

        await new AddPartyExperienceBonusAttributesPlugInSeason6().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var values = gameConfiguration.GlobalBaseAttributeValues.Where(a => a.Definition?.Id == Stats.SetPartyExperienceBonus.Id).ToList();
        Assert.That(values, Has.Count.EqualTo(1));
        Assert.That(values[0].Value, Is.EqualTo(0.5f));
    }

    private static void AssertDefaults(GameConfiguration gameConfiguration)
    {
        foreach (var (attribute, value) in ExpectedValues)
        {
            Assert.That(gameConfiguration.Attributes.Count(a => a.Id == attribute.Id), Is.EqualTo(1), $"Attribute {attribute.Designation}");
            var values = gameConfiguration.GlobalBaseAttributeValues.Where(a => a.Definition?.Id == attribute.Id).ToList();
            Assert.That(values, Has.Count.EqualTo(1), $"Value of {attribute.Designation}");
            Assert.That(values[0].Value, Is.EqualTo(value), $"Value of {attribute.Designation}");
        }
    }

    private static void RemovePartyExperienceBonuses(GameConfiguration gameConfiguration)
    {
        foreach (var (attribute, _) in ExpectedValues)
        {
            gameConfiguration.GlobalBaseAttributeValues.Remove(gameConfiguration.GlobalBaseAttributeValues.Single(a => a.Definition?.Id == attribute.Id));
            gameConfiguration.Attributes.Remove(gameConfiguration.Attributes.Single(a => a.Id == attribute.Id));
        }
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
