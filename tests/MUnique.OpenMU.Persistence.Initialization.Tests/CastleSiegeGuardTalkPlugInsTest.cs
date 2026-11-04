// <copyright file="CastleSiegeGuardTalkPlugInsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the NPC configuration of the Castle Siege guard talk plug-ins:
/// the guardsmen (224) open the registration window, the guard (220) is a passive NPC which opens the Land of Trials dialog.
/// </summary>
[TestFixture]
internal class CastleSiegeGuardTalkPlugInsTest
{
    private const short GuardsmanNumber = 224;
    private const short GuardNumber = 220;

    /// <summary>
    /// Tests that a new season 6 database configures both plug-ins with their NPCs.
    /// </summary>
    [Test]
    public async Task NewDatabaseConfiguresGuardsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertConfiguredNpcs(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update moves the Land of Trials dialog from the guardsman to the guard, turns the guard into a passive NPC,
    /// adds the registration plug-in to a database created without it, and can be applied twice.
    /// </summary>
    [Test]
    public async Task UpdateConfiguresGuardsOfExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        // Restore the state before the update: the Land of Trials dialog was opened by the guardsman,
        // and the registration plug-in didn't exist.
        var landOfTrials = GetPlugInConfiguration<CastleSiegeLandOfTrialsGuardTalkPlugIn>(gameConfiguration)!;
        landOfTrials.SetConfiguration(
            new NpcTalkPlugInConfiguration { Npc = GetMonster(gameConfiguration, GuardsmanNumber) },
            new ConfigureNpcTalkPlugInsPlugInBase.IdReferenceHandler());
        gameConfiguration.PlugInConfigurations.Remove(GetPlugInConfiguration<CastleSiegeRegistrationGuardsmanTalkPlugIn>(gameConfiguration)!);

        // Before, the guard was an attacking guard with its own intelligence and combat attributes.
        var guard = GetMonster(gameConfiguration, GuardNumber);
        guard.ObjectKind = NpcObjectKind.Guard;
        guard.IntelligenceTypeName = "MUnique.OpenMU.GameLogic.NPC.GuardIntelligence";
        guard.MoveRange = 3;
        guard.AttackRange = 2;
        guard.ViewRange = 8;
        guard.MoveDelay = TimeSpan.FromMilliseconds(400);
        guard.AttackDelay = TimeSpan.FromMilliseconds(1500);
        guard.RespawnDelay = TimeSpan.FromSeconds(3);
        var attribute = context.CreateNew<MonsterAttribute>();
        attribute.AttributeDefinition = gameConfiguration.Attributes.First();
        attribute.Value = 500;
        guard.Attributes.Add(attribute);

        var update = new ConfigureCastleSiegeGuardTalkPlugInsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertConfiguredNpcs(gameConfiguration);
        Assert.That(
            gameConfiguration.PlugInConfigurations.Count(c => c.TypeId == typeof(CastleSiegeRegistrationGuardsmanTalkPlugIn).GUID),
            Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the update only applies to season 6, which is the only version with Castle Siege.
    /// </summary>
    [Test]
    public void UpdateIsSeason6Only()
    {
        Assert.That(new ConfigureCastleSiegeGuardTalkPlugInsUpdatePlugIn().DataInitializationKey, Is.EqualTo(VersionSeasonSix.DataInitialization.Id));
    }

    private static void AssertConfiguredNpcs(GameConfiguration gameConfiguration)
    {
        Assert.Multiple(() =>
        {
            Assert.That(ConfiguredNpcId<CastleSiegeRegistrationGuardsmanTalkPlugIn>(gameConfiguration), Is.EqualTo(GetMonsterId(gameConfiguration, GuardsmanNumber)));
            Assert.That(ConfiguredNpcId<CastleSiegeLandOfTrialsGuardTalkPlugIn>(gameConfiguration), Is.EqualTo(GetMonsterId(gameConfiguration, GuardNumber)));
            Assert.That(GetPlugInConfiguration<CastleSiegeRegistrationGuardsmanTalkPlugIn>(gameConfiguration)!.IsActive, Is.True);
        });

        // The guard is a passive NPC which players talk to, like the guardsman.
        var guard = GetMonster(gameConfiguration, GuardNumber);
        Assert.Multiple(() =>
        {
            Assert.That(guard.ObjectKind, Is.EqualTo(NpcObjectKind.PassiveNpc));
            Assert.That(guard.IntelligenceTypeName, Is.Null);
            Assert.That(guard.Attributes, Is.Empty);
            Assert.That(guard.MoveRange, Is.Zero);
            Assert.That(guard.AttackRange, Is.Zero);
            Assert.That(guard.ViewRange, Is.Zero);
            Assert.That(guard.MoveDelay, Is.EqualTo(TimeSpan.Zero));
            Assert.That(guard.AttackDelay, Is.EqualTo(TimeSpan.Zero));
            Assert.That(guard.RespawnDelay, Is.EqualTo(TimeSpan.Zero));
        });
    }

    /// <summary>
    /// Gets the id of the configured NPC from the serialized configuration, which references the NPC by its id.
    /// </summary>
    private static Guid? ConfiguredNpcId<TPlugIn>(GameConfiguration gameConfiguration)
        where TPlugIn : NpcTalkPlugInBase
    {
        var json = GetPlugInConfiguration<TPlugIn>(gameConfiguration)?.CustomConfiguration;
        if (json is null)
        {
            return null;
        }

        using var document = System.Text.Json.JsonDocument.Parse(json);
        return document.RootElement.GetProperty(nameof(NpcTalkPlugInConfiguration.Npc)).GetProperty("$ref").GetGuid();
    }

    private static PlugInConfiguration? GetPlugInConfiguration<TPlugIn>(GameConfiguration gameConfiguration)
        => gameConfiguration.PlugInConfigurations.FirstOrDefault(c => c.TypeId == typeof(TPlugIn).GUID);

    private static MonsterDefinition GetMonster(GameConfiguration gameConfiguration, short number)
        => gameConfiguration.Monsters.Single(monster => monster.Number == number);

    private static Guid GetMonsterId(GameConfiguration gameConfiguration, short number)
        => ((MUnique.OpenMU.Persistence.IIdentifiable)GetMonster(gameConfiguration, number)).Id;

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
