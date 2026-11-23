// <copyright file="IllusionTempleNpcTalkPlugInsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the NPC configuration of the Illusion Temple talk plug-ins: the stone statue (380) and the
/// item storages of the allied (383) and the illusion forces (384).
/// </summary>
[TestFixture]
internal class IllusionTempleNpcTalkPlugInsTest
{
    private const short StoneStatueNumber = 380;
    private const short AllianceItemStorageNumber = 383;
    private const short IllusionItemStorageNumber = 384;
    private const short MirageNumber = 385;

    /// <summary>
    /// Tests that a new season 6 database configures the three plug-ins with their NPCs.
    /// </summary>
    [Test]
    public async Task NewDatabaseConfiguresNpcsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertConfiguredNpcs(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update sets the NPCs which an existing database has left empty,
    /// and that applying it twice doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateConfiguresNpcsOfExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        // The state which the Illusion Temple update leaves behind: the configurations exist, but without NPC.
        SetNpc<IllusionTempleStoneStatueTalkPlugIn>(gameConfiguration, null);
        SetNpc<IllusionTempleAllianceStorageTalkPlugIn>(gameConfiguration, null);
        SetNpc<IllusionTempleIllusionStorageTalkPlugIn>(gameConfiguration, null);
        Assert.That(ConfiguredNpcId<IllusionTempleStoneStatueTalkPlugIn>(gameConfiguration), Is.Null);

        var update = new ConfigureIllusionTempleNpcTalkPlugInsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var configurationsAfterFirstUpdate = gameConfiguration.PlugInConfigurations.Select(c => (c.TypeId, c.CustomConfiguration)).ToList();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertConfiguredNpcs(gameConfiguration);
        Assert.That(gameConfiguration.PlugInConfigurations.Select(c => (c.TypeId, c.CustomConfiguration)), Is.EqualTo(configurationsAfterFirstUpdate));
    }

    /// <summary>
    /// Tests that the update creates the configuration of a plug-in which has none, and doesn't overwrite an NPC which was set already.
    /// </summary>
    [Test]
    public async Task UpdateKeepsConfiguredNpcsAndAddsMissingConfigurationAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        SetNpc<IllusionTempleStoneStatueTalkPlugIn>(gameConfiguration, GetMonster(gameConfiguration, MirageNumber));
        gameConfiguration.PlugInConfigurations.Remove(GetPlugInConfiguration<IllusionTempleIllusionStorageTalkPlugIn>(gameConfiguration)!);

        await new ConfigureIllusionTempleNpcTalkPlugInsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(ConfiguredNpcId<IllusionTempleStoneStatueTalkPlugIn>(gameConfiguration), Is.EqualTo(GetMonsterId(gameConfiguration, MirageNumber)));
            Assert.That(ConfiguredNpcId<IllusionTempleAllianceStorageTalkPlugIn>(gameConfiguration), Is.EqualTo(GetMonsterId(gameConfiguration, AllianceItemStorageNumber)));
            Assert.That(ConfiguredNpcId<IllusionTempleIllusionStorageTalkPlugIn>(gameConfiguration), Is.EqualTo(GetMonsterId(gameConfiguration, IllusionItemStorageNumber)));
            Assert.That(GetPlugInConfiguration<IllusionTempleIllusionStorageTalkPlugIn>(gameConfiguration)!.IsActive, Is.True);
        });
    }

    /// <summary>
    /// Tests that the update only applies to season 6, which is the only version with the Illusion Temple.
    /// </summary>
    [Test]
    public void UpdateIsSeason6Only()
    {
        Assert.That(new ConfigureIllusionTempleNpcTalkPlugInsUpdatePlugIn().DataInitializationKey, Is.EqualTo(VersionSeasonSix.DataInitialization.Id));
    }

    private static void AssertConfiguredNpcs(GameConfiguration gameConfiguration)
    {
        Assert.Multiple(() =>
        {
            Assert.That(ConfiguredNpcId<IllusionTempleStoneStatueTalkPlugIn>(gameConfiguration), Is.EqualTo(GetMonsterId(gameConfiguration, StoneStatueNumber)));
            Assert.That(ConfiguredNpcId<IllusionTempleAllianceStorageTalkPlugIn>(gameConfiguration), Is.EqualTo(GetMonsterId(gameConfiguration, AllianceItemStorageNumber)));
            Assert.That(ConfiguredNpcId<IllusionTempleIllusionStorageTalkPlugIn>(gameConfiguration), Is.EqualTo(GetMonsterId(gameConfiguration, IllusionItemStorageNumber)));
        });
    }

    private static void SetNpc<TPlugIn>(GameConfiguration gameConfiguration, MonsterDefinition? npc)
        where TPlugIn : NpcTalkPlugInBase
    {
        GetPlugInConfiguration<TPlugIn>(gameConfiguration)!.SetConfiguration(
            new NpcTalkPlugInConfiguration { Npc = npc },
            new ConfigureNpcTalkPlugInsPlugInBase.IdReferenceHandler());
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
        var npc = document.RootElement.GetProperty(nameof(NpcTalkPlugInConfiguration.Npc));
        return npc.ValueKind == System.Text.Json.JsonValueKind.Null ? null : npc.GetProperty("$ref").GetGuid();
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
