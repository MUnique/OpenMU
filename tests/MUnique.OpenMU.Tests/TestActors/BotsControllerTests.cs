// <copyright file="BotsControllerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.TestActors;

using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.TestActors;
using MUnique.OpenMU.PlugIns;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

/// <summary>
/// Tests the population bot switch.
/// </summary>
[TestFixture]
public class BotsControllerTests
{
    /// <summary>
    /// <c>bots on --count N</c> writes N accounts with one character each and no presence rotation,
    /// and that configuration survives the round trip through the persisted plugin configuration.
    /// </summary>
    [Test]
    public void BotConfigurationRoundTripsThroughThePlugInConfiguration()
    {
        var configuration = new BotConfiguration { NumberOfAccounts = 10, MaxCharactersPerAccount = 5, PresenceRotation = true };
        var entity = new PlugInConfiguration { TypeId = typeof(BotFeaturePlugIn).GUID, IsActive = true };

        BotsController.Apply(configuration, enabled: true, count: 2);
        entity.SetConfiguration(configuration, null);
        var roundTripped = entity.GetConfiguration<BotConfiguration>(null);

        Assert.That(roundTripped, Is.Not.Null);
        Assert.That(roundTripped!.Enabled, Is.True);
        Assert.That(roundTripped.NumberOfAccounts, Is.EqualTo(2));
        Assert.That(roundTripped.MaxCharactersPerAccount, Is.EqualTo(1));
        Assert.That(roundTripped.PresenceRotation, Is.False);
    }

    /// <summary>
    /// Without a count, the documented default population is requested.
    /// </summary>
    [Test]
    public void DefaultCountIsUsedWhenNoneIsGiven()
    {
        var configuration = new BotConfiguration();

        BotsController.Apply(configuration, enabled: true, count: null);

        Assert.That(configuration.NumberOfAccounts, Is.EqualTo(BotsController.DefaultBotCount));
        Assert.That(configuration.MaxCharactersPerAccount, Is.EqualTo(1));
    }

    /// <summary>
    /// Switching off leaves the population settings alone - the accounts stay, the bots just log out.
    /// </summary>
    [Test]
    public void SwitchingOffKeepsThePopulationSettings()
    {
        var configuration = new BotConfiguration();
        BotsController.Apply(configuration, enabled: true, count: 3);

        BotsController.Apply(configuration, enabled: false, count: null);

        Assert.That(configuration.Enabled, Is.False);
        Assert.That(configuration.NumberOfAccounts, Is.EqualTo(3));
    }

    /// <summary>
    /// An unknown action is refused with a message which names the ones that exist.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task UnknownActionIsRefusedAsync()
    {
        var locator = new Mock<IGameServerContextLocator>();
        locator.Setup(l => l.GetContexts()).Returns([(0, new Mock<MUnique.OpenMU.GameLogic.IGameServerContext>().Object)]);
        var controller = new BotsController(locator.Object, new NullLogger<BotsController>());

        var result = await controller.HandleAsync("nonsense", null).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.BadRequest));
        Assert.That(result.Error, Does.Contain("status"));
    }

    /// <summary>
    /// A process without a game server says so instead of pretending.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task NoGameServerIsReportedAsync()
    {
        var locator = new Mock<IGameServerContextLocator>();
        locator.Setup(l => l.GetContexts()).Returns([]);
        var controller = new BotsController(locator.Object, new NullLogger<BotsController>());

        var result = await controller.HandleAsync("status", null).ConfigureAwait(false);

        Assert.That(result.Ok, Is.False);
        Assert.That(result.Code, Is.EqualTo(ActorErrorCodes.UnknownServer));
    }
}
