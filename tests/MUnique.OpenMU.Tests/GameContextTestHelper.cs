// <copyright file="GameContextTestHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Helper functions to create test game contexts.
/// </summary>
public static class GameContextTestHelper
{
    /// <summary>
    /// Creates a game context.
    /// </summary>
    /// <param name="additionalPlugInConfigurations">Additional plugin configurations which should be applied, e.g. to deactivate specific plugins.</param>
    /// <returns>The game context with MuHelperFeaturePlugIn configured.</returns>
    public static IGameContext CreateGameContext(IEnumerable<PlugInConfiguration>? additionalPlugInConfigurations = null)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var context = contextProvider.CreateNewContext();
        var gameConfig = context.CreateNew<MUnique.OpenMU.Persistence.BasicModel.GameConfiguration>();
        var mapDef = context.CreateNew<MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition>();
        mapDef.Number = 0;
        mapDef.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfig.Maps.Add(mapDef);
        gameConfig.MaximumPartySize = 5;
        gameConfig.RecoveryInterval = int.MaxValue;
        gameConfig.MaximumInventoryMoney = int.MaxValue;
        gameConfig.ItemDropDuration = TimeSpan.FromMinutes(1);

        var mapInitializer = new MapInitializer(gameConfig, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var plugInConfigurations = new List<PlugInConfiguration>
        {
            new ()
            {
                TypeId = new Guid("E90A72C3-0459-4323-B6D3-171F88D35542"), // MuHelperFeaturePlugIn
                IsActive = true,
            },
        };

        if (additionalPlugInConfigurations is { })
        {
            plugInConfigurations.AddRange(additionalPlugInConfigurations);
        }

        var plugInManager = new PlugInManager(plugInConfigurations, new NullLoggerFactory(), null, null);
        var gameContext = new GameContext(gameConfig, contextProvider, mapInitializer, new NullLoggerFactory(), plugInManager, NullDropGenerator.Instance, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;

        return gameContext;
    }
}
