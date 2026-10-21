// <copyright file="PersistentObjectsLookupControllerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the <see cref="PersistentObjectsLookupController"/>.
/// </summary>
[TestFixture]
public class PersistentObjectsLookupControllerTests
{
    private readonly ItemDefinition _jewelOfBless = new() { Name = "Jewel of Bless" };
    private readonly ItemDefinition _jewelOfSoul = new() { Name = "Jewel of Soul" };

    private Mock<IDataSource<GameConfiguration>> _dataSource = null!;
    private Mock<IContext> _gameConfigurationContext = null!;
    private PersistentObjectsLookupController _controller = null!;

    /// <summary>
    /// Sets up the game configuration data source with its context.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._gameConfigurationContext = CreateContextMock();

        this._dataSource = new Mock<IDataSource<GameConfiguration>>();
        this._dataSource.Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<GameConfiguration>().Object);
        this._dataSource.Setup(s => s.GetContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(this._gameConfigurationContext.Object);
        this._dataSource.Setup(s => s.IsSupporting(typeof(ItemDefinition))).Returns(true);
        this._dataSource.Setup(s => s.GetAll<ItemDefinition>()).Returns([this._jewelOfBless, this._jewelOfSoul]);

        this._controller = new PersistentObjectsLookupController(
            Mock.Of<IPersistenceContextProvider>(),
            this._dataSource.Object,
            NullLogger<PersistentObjectsLookupController>.Instance);
    }

    /// <summary>
    /// When the context of the game configuration source is passed, the suggestions
    /// come from the already loaded configuration instead of querying the context again.
    /// </summary>
    [Test]
    public async Task GameConfigurationContext_UsesLoadedConfigurationAsync()
    {
        var result = await this._controller.GetSuggestionsAsync<ItemDefinition>("bless", this._gameConfigurationContext.Object).ConfigureAwait(false);

        Assert.That(result, Is.EquivalentTo(new[] { this._jewelOfBless }));
        this._gameConfigurationContext.Verify(c => c.GetAsync<ItemDefinition>(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// When another context which supports the type is passed, the suggestions are queried
    /// from that context, so that the selected object belongs to it.
    /// </summary>
    [Test]
    public async Task OtherSupportingContext_QueriesContextAsync()
    {
        var otherItem = new ItemDefinition { Name = "Jewel of Bless" };
        var otherContext = CreateContextMock();
        otherContext.Setup(c => c.GetAsync<ItemDefinition>(It.IsAny<CancellationToken>()))
            .ReturnsAsync([otherItem]);

        var result = await this._controller.GetSuggestionsAsync<ItemDefinition>("bless", otherContext.Object).ConfigureAwait(false);

        Assert.That(result, Is.EquivalentTo(new[] { otherItem }));
        this._dataSource.Verify(s => s.GetAll<ItemDefinition>(), Times.Never);
    }

    private static Mock<IContext> CreateContextMock()
    {
        var context = new Mock<IContext>();
        context.Setup(c => c.IsSupporting(typeof(ItemDefinition))).Returns(true);
        return context;
    }
}
