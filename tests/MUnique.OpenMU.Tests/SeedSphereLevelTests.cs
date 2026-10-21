// <copyright file="SeedSphereLevelTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the socket option level which results from mounting a seed sphere.
/// </summary>
[TestFixture]
public class SeedSphereLevelTests
{
    private GameConfiguration _gameConfiguration = null!;

    /// <summary>
    /// Sets up the test environment by initializing the season 6 configuration data.
    /// </summary>
    [OneTimeSetUp]
    public async ValueTask SetupAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new DataInitialization(contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(0, false).ConfigureAwait(false);
        this._gameConfiguration = (await contextProvider.CreateNewConfigurationContext().GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    /// <summary>
    /// Tests that the socket option level of each seed sphere is defined by its sphere,
    /// so that a Mono seed sphere results in level 1 and a fifth level seed sphere in level 5.
    /// </summary>
    [Test]
    public void SocketOptionLevelFollowsSphereOfSeedSphere()
    {
        var seedSpheres = this._gameConfiguration.Items
            .Where(i => i.Group == 12 && i.Number >= SeedSphereCrafting.SeedSphereNumberStart && i.Number < SeedSphereCrafting.SeedSphereNumberStart + (5 * SeedSphereCrafting.SeedTypes))
            .ToList();
        Assert.That(seedSpheres, Has.Count.EqualTo(5 * SeedSphereCrafting.SeedTypes));

        foreach (var seedSphere in seedSpheres)
        {
            // The sphere level of a seed sphere is initialized as its drop level, starting at 0.
            Assert.That(MountSeedSphereCrafting.GetSphereLevel(seedSphere), Is.EqualTo(seedSphere.DropLevel + 1), seedSphere.Name.ValueInNeutralLanguage);
        }
    }
}
