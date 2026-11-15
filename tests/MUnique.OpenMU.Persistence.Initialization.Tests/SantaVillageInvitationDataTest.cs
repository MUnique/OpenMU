// <copyright file="SantaVillageInvitationDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the invitation to the santa village and of the spawn gate of the santa village.
/// </summary>
[TestFixture]
internal class SantaVillageInvitationDataTest
{
    private const byte SantaVillageNumber = 62;

    /// <summary>
    /// Tests that a new season 6 database contains the invitation and the spawn gate.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsInvitationAndSpawnGateAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the invitation and the spawn gate to a database which was created before they existed.
    /// Applying it twice doesn't add them twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsInvitationAndSpawnGateToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.Items.Remove(gameConfiguration.Items.Single(IsInvitation));
        var santaVillage = gameConfiguration.Maps.Single(map => map.Number == SantaVillageNumber);
        santaVillage.ExitGates.Clear();

        var update = new AddSantaVillageInvitationUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertData(gameConfiguration);
    }

    private static bool IsInvitation(DataModel.Configuration.Items.ItemDefinition item) => item.Group == 13 && item.Number == 66;

    private static void AssertData(GameConfiguration gameConfiguration)
    {
        var invitation = gameConfiguration.Items.Single(IsInvitation);
        Assert.That(invitation.Durability, Is.EqualTo(10));
        Assert.That(invitation.Width, Is.EqualTo(1));
        Assert.That(invitation.Height, Is.EqualTo(1));
        Assert.That(invitation.Name.ValueInNeutralLanguage, Is.EqualTo("Invitation to Santa Village"));

        var santaVillage = gameConfiguration.Maps.Single(map => map.Number == SantaVillageNumber);
        var spawnGate = santaVillage.ExitGates.Single(gate => gate.IsSpawnGate);
        var terrain = new GameMapTerrain(santaVillage);
        for (var x = spawnGate.X1; x <= spawnGate.X2; x++)
        {
            for (var y = spawnGate.Y1; y <= spawnGate.Y2; y++)
            {
                Assert.That(terrain.WalkMap[x, y], Is.True, $"({x}, {y}) is not walkable");
                Assert.That(terrain.SafezoneMap[x, y], Is.True, $"({x}, {y}) is not in the safezone");
            }
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
