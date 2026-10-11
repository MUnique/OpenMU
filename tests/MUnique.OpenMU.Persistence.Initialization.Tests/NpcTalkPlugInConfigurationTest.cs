// <copyright file="NpcTalkPlugInConfigurationTest.cs" company="MUnique">
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
/// Tests that the NPCs of the <see cref="NpcTalkPlugInBase"/> plugins are configured.
/// </summary>
[TestFixture]
internal class NpcTalkPlugInConfigurationTest
{
    private static readonly Type[] NpcTalkPlugInTypes = typeof(NpcTalkPlugInBase).Assembly.GetTypes()
        .Where(type => type.IsSubclassOf(typeof(NpcTalkPlugInBase)) && !type.IsAbstract)
        .ToArray();

    /// <summary>
    /// Tests that a new database configures the default NPCs of the plugins, if they exist.
    /// </summary>
    /// <param name="version">The version of the data.</param>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("097k")]
    [TestCase("season6")]
    public async Task NewDatabaseConfiguresDefaultNpcsAsync(string version)
    {
        var (contextProvider, context, gameConfiguration) = await CreateConfigurationAsync(version).ConfigureAwait(false);
        using var _ = context;

        await AssertConfiguredNpcsAsync(contextProvider, gameConfiguration, version == "season6").ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that the update configures the default NPCs of the plugins of a database which was created before they were configurable,
    /// and that applying it twice doesn't change the result.
    /// </summary>
    /// <param name="version">The version of the data.</param>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("season6")]
    public async Task UpdateConfiguresDefaultNpcsAsync(string version)
    {
        var (contextProvider, context, gameConfiguration) = await CreateConfigurationAsync(version).ConfigureAwait(false);
        using var _ = context;
        foreach (var plugInType in NpcTalkPlugInTypes)
        {
            gameConfiguration.PlugInConfigurations.Single(c => c.TypeId == plugInType.GUID).CustomConfiguration = null;
        }

        UpdatePlugInBase update = version switch
        {
            "075" => new ConfigureNpcTalkPlugInsPlugIn075(),
            "095d" => new ConfigureNpcTalkPlugInsPlugIn095D(),
            _ => new ConfigureNpcTalkPlugInsPlugInSeason6(),
        };
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        await AssertConfiguredNpcsAsync(contextProvider, gameConfiguration, version == "season6").ConfigureAwait(false);
    }

    private static async Task AssertConfiguredNpcsAsync(IPersistenceContextProvider contextProvider, GameConfiguration gameConfiguration, bool allNpcsExist)
    {
        var dataSource = new GameConfigurationDataSource(new NullLogger<GameConfigurationDataSource>(), contextProvider);
        await dataSource.GetOwnerAsync(gameConfiguration.GetId()).ConfigureAwait(false);
        var referenceHandler = new ByDataSourceReferenceHandler(dataSource);

        Assert.That(NpcTalkPlugInTypes, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var plugInType in NpcTalkPlugInTypes)
            {
                var plugIn = (NpcTalkPlugInBase)Activator.CreateInstance(plugInType)!;
                var expectedNpc = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == plugIn.DefaultNpcNumber);
                var configuration = gameConfiguration.PlugInConfigurations.Single(c => c.TypeId == plugInType.GUID)
                    .GetConfiguration<NpcTalkPlugInConfiguration>(referenceHandler);
                Assert.That(configuration, Is.Not.Null, plugInType.Name);
                if (allNpcsExist)
                {
                    Assert.That(expectedNpc, Is.Not.Null, plugInType.Name);
                }

                Assert.That(configuration?.Npc?.Number, Is.EqualTo(expectedNpc?.Number), plugInType.Name);
            }
        });
    }

    private static async Task<(IPersistenceContextProvider ContextProvider, IContext Context, GameConfiguration GameConfiguration)> CreateConfigurationAsync(string version)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        DataInitializationBase dataInitialization = version switch
        {
            "075" => new Version075.DataInitialization(contextProvider, new NullLoggerFactory()),
            "095d" => new Version095d.DataInitialization(contextProvider, new NullLoggerFactory()),
            "097k" => new Version097k.DataInitialization(contextProvider, new NullLoggerFactory()),
            _ => new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory()),
        };
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (contextProvider, context, gameConfiguration);
    }
}
