// <copyright file="DatabaseInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Measurements;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

/// <summary>
/// Re-creates the database with the season 6 data and the test accounts,
/// the same way the server does it on its first start.
/// </summary>
internal static class DatabaseInitializer
{
    /// <summary>
    /// Re-creates the database.
    /// </summary>
    public static async ValueTask RunAsync()
    {
        var loggerFactory = NullLoggerFactory.Instance;
        var contextProvider = new PersistenceContextProvider(loggerFactory, null);
        var (result, _) = await Measure.RunAsync(async () =>
        {
            using (await contextProvider.ReCreateDatabaseAsync().ConfigureAwait(false))
            {
                var initialization = new DataInitialization(contextProvider, loggerFactory);
                await initialization.CreateInitialDataAsync(3, true).ConfigureAwait(false);
            }

            return true;
        }).ConfigureAwait(false);

        Console.WriteLine($"Database initialized in {result.Duration.TotalSeconds:0.0} s.");
    }
}
