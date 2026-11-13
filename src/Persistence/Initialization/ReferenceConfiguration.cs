// <copyright file="ReferenceConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Creates the configuration of a data initialization in memory, e.g. to compare the installed configuration with its defaults.
/// </summary>
public static class ReferenceConfiguration
{
    /// <summary>
    /// Executes the data initialization in memory and returns its game configuration.
    /// This is mostly synchronous, CPU bound work which takes a while, so callers cache what they need of the result.
    /// </summary>
    /// <param name="plugInManager">The plug in manager, which knows the data initializations.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="initializationKey">The key of the data initialization, e.g. "season6".</param>
    /// <returns>The game configuration of the data initialization.</returns>
    public static async ValueTask<GameConfiguration> CreateAsync(PlugInManager plugInManager, ILoggerFactory loggerFactory, string initializationKey)
    {
        var plugIn = plugInManager.GetStrategyProvider<string, IDataInitializationPlugIn>()?[initializationKey]
                     ?? throw new InvalidOperationException($"The data initialization '{initializationKey}' is not available.");

        var inMemoryProvider = new InMemoryPersistenceContextProvider();
        var referenceInitialization = Activator.CreateInstance(plugIn.GetType(), inMemoryProvider, loggerFactory) as IDataInitializationPlugIn
                                      ?? throw new InvalidOperationException($"The data initialization '{initializationKey}' can't be executed in memory.");
        await referenceInitialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);

        using var referenceContext = inMemoryProvider.CreateNewContext();
        return (await referenceContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).FirstOrDefault()
               ?? throw new InvalidOperationException($"The data initialization '{initializationKey}' created no game configuration.");
    }
}
