// <copyright file="LeaderboardsPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.Threading;
using Bunit;
using Bunit.Extensions.WaitForHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Tests for the <see cref="Leaderboards"/> page.
/// </summary>
[TestFixture]
[NonParallelizable]
public class LeaderboardsPageTests
{
    private readonly Queue<TaskCompletionSource<IReadOnlyList<MiniGameRankingSummary>>> _pendingLoads = new();

    private BunitContext _context = null!;

    private CultureInfo _previousUiCulture = null!;

    /// <summary>
    /// Sets up the test context with a Blood Castle which saves its ranking, and a player context
    /// whose leaderboard queries only complete when the test completes them.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        this._pendingLoads.Clear();

        var gameConfiguration = new Persistence.BasicModel.GameConfiguration();
        gameConfiguration.MiniGameDefinitions.Add(new Persistence.BasicModel.MiniGameDefinition
        {
            Id = Guid.NewGuid(),
            Type = MiniGameType.BloodCastle,
            SaveRankingStatistics = true,
        });
        var gameConfigurationSource = new Mock<IDataSource<GameConfiguration>>();
        gameConfigurationSource
            .Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(gameConfiguration);

        var playerContext = new Mock<IPlayerContext>();
        playerContext
            .Setup(c => c.GetMiniGameLeaderboardAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                var load = new TaskCompletionSource<IReadOnlyList<MiniGameRankingSummary>>(TaskCreationOptions.RunContinuationsAsynchronously);
                this._pendingLoads.Enqueue(load);
                return new ValueTask<IReadOnlyList<MiniGameRankingSummary>>(load.Task);
            });
        var persistenceContextProvider = new Mock<IPersistenceContextProvider>();
        persistenceContextProvider.Setup(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>())).Returns(playerContext.Object);

        this._context = new BunitContext();
        this._context.Services.AddSingleton(new MiniGameReadOperations(persistenceContextProvider.Object, gameConfigurationSource.Object));
        this._context.Services.AddSingleton(Mock.Of<IToastService>());
        this._context.Services.AddSingleton(Mock.Of<ILogger<Leaderboards>>());
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
    }

    /// <summary>
    /// Restores the culture and disposes the test context.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>
    /// When the period is changed twice, and the load of the first change completes after the load
    /// of the second one, the leaderboard of the current selection stays shown.
    /// </summary>
    [Test]
    public void LateResultOfPreviousSelectionDoesNotOverwriteCurrentOne()
    {
        var cut = this._context.Render<Leaderboards>();
        this.CompleteNextLoad("Initial");
        cut.WaitForState(() => cut.Markup.Contains("Initial", StringComparison.Ordinal));

        cut.Find("select").Change(nameof(Leaderboards.LeaderboardPeriod.Last30Days));
        cut.Find("select").Change(nameof(Leaderboards.LeaderboardPeriod.Last7Days));
        Assert.That(this._pendingLoads, Has.Count.EqualTo(2));
        var previousLoad = this._pendingLoads.Dequeue();
        this.CompleteNextLoad("Current");
        cut.WaitForState(() => cut.Markup.Contains("Current", StringComparison.Ordinal));

        previousLoad.SetResult([new MiniGameRankingSummary(Guid.NewGuid(), "Previous", null, 10, 10, 1)]);

        // The ignored result causes no render, so give it some time to show up wrongly.
        Assert.Throws<WaitForFailedException>(() => cut.WaitForState(() => cut.Markup.Contains("Previous", StringComparison.Ordinal), TimeSpan.FromMilliseconds(500)));
        Assert.That(cut.Instance.Entries.Select(e => e.CharacterName), Is.EqualTo(new[] { "Current" }));
        Assert.That(cut.Markup, Does.Contain("Current"));
    }

    private void CompleteNextLoad(string characterName)
    {
        Assert.That(this._pendingLoads, Is.Not.Empty);
        this._pendingLoads.Dequeue().SetResult([new MiniGameRankingSummary(Guid.NewGuid(), characterName, null, 100, 50, 2)]);
    }
}
