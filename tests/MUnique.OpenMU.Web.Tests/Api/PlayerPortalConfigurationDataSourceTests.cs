// <copyright file="PlayerPortalConfigurationDataSourceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.PlayerPortal.Api;

/// <summary>Regression tests for the player portal's least-privilege configuration read path.</summary>
[TestFixture]
public sealed class PlayerPortalConfigurationDataSourceTests
{
    /// <summary>
    /// The first configuration read must use the restricted context, never the administrative context.
    /// </summary>
    [Test]
    public async Task GetOwnerUsesOnlyConfigurationContextAsync()
    {
        var owner = new GameConfiguration();
        var configurationContext = new Mock<IConfigurationContext>();
        configurationContext
            .Setup(context => context.GetAsync<GameConfiguration>(It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<IEnumerable<GameConfiguration>>(new[] { owner }));

        var persistence = new Mock<IPersistenceContextProvider>();
        persistence
            .Setup(provider => provider.CreateNewConfigurationContext())
            .Returns(configurationContext.Object);
        persistence
            .Setup(provider => provider.CreateNewContext())
            .Throws(new InvalidOperationException("Administrative context must not be requested."));

        using var source = new PlayerPortalGameConfigurationDataSource(
            NullLogger<PlayerPortalGameConfigurationDataSource>.Instance, persistence.Object);

        var loaded = await source.GetOwnerAsync().ConfigureAwait(false);

        Assert.That(loaded, Is.SameAs(owner));
        persistence.Verify(provider => provider.CreateNewConfigurationContext(), Times.Once);
        persistence.Verify(provider => provider.CreateNewContext(), Times.Never);
    }
}
