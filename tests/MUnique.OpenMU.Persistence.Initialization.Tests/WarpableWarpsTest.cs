// <copyright file="WarpableWarpsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the season 6 warps which the original client doesn't offer in its warp window are only
/// available to game masters, in a new database and after the update.
/// </summary>
[TestFixture]
internal class WarpableWarpsTest
{
    private static readonly int[] WarpsOutsideOfOriginalWarpWindow = [26, 44, 48];

    /// <summary>
    /// Tests that a new season 6 database has Crywolf, Loren Market and the second LaCleon warp
    /// not warpable, and all other warps warpable.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasWarpsOutsideOfOriginalWarpWindowNotWarpableAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        var notWarpable = gameConfiguration.WarpList.Where(warp => !warp.IsWarpable).Select(warp => warp.Index).Order();

        Assert.That(notWarpable, Is.EqualTo(WarpsOutsideOfOriginalWarpWindow));
    }

    /// <summary>
    /// Tests that the update makes these warps not warpable in a database which has them warpable, that
    /// it leaves a warp alone whose map was changed, and that applying it twice keeps the result.
    /// </summary>
    [Test]
    public async Task UpdateMakesWarpsOutsideOfOriginalWarpWindowNotWarpableAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var warp in gameConfiguration.WarpList)
        {
            warp.IsWarpable = true;
        }

        var lorenMarket = gameConfiguration.WarpList.Single(warp => warp.Index == 44);
        lorenMarket.Gate = gameConfiguration.WarpList.Single(warp => warp.Index == 2).Gate; // changed by an admin

        var update = new RestrictWarpsOutsideOfOriginalWarpWindowPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var notWarpable = gameConfiguration.WarpList.Where(warp => !warp.IsWarpable).Select(warp => warp.Index).Order();
        Assert.That(notWarpable, Is.EqualTo(new[] { 26, 48 }));
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
