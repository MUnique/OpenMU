// <copyright file="IndexPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Dashboard;

using System.Globalization;
using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Services;
using MUnique.OpenMU.Web.Tests.Mcp;

/// <summary>
/// Tests for the dashboard on the start page (<see cref="Index"/>).
/// </summary>
[TestFixture]
public class IndexPageTests
{
    /// <summary>
    /// The statistics of the servers and of the database are shown.
    /// </summary>
    [Test]
    public void ShowsStatistics()
    {
        using var context = CreateContext(isAdministrator: false);

        var page = context.Render<Index>();

        page.WaitForAssertion(() =>
        {
            var values = page.FindAll(".statistic-value").Select(v => v.TextContent.Trim()).ToList();
            Assert.That(values[0], Is.EqualTo(15.ToString("N0")));
            Assert.That(values[1], Does.StartWith("2").And.EndWith("/ 3"));
            Assert.That(values[2], Is.EqualTo(1234.ToString("N0")));
            Assert.That(values[3], Is.EqualTo(56.ToString("N0")));
        });
    }

    /// <summary>
    /// The cards with system information and actions for administrators are not shown to other users.
    /// </summary>
    [Test]
    public void HidesAdministratorCardsForViewers()
    {
        using var context = CreateContext(isAdministrator: false);

        var page = context.Render<Index>();

        Assert.That(page.Markup, Does.Not.Contain(Resources.DashboardAdministratorsOnly));
    }

    /// <summary>
    /// The cards with system information and actions are shown to administrators.
    /// </summary>
    [Test]
    public void ShowsAdministratorCardsForAdministrators()
    {
        using var context = CreateContext(isAdministrator: true);

        var page = context.Render<Index>();

        page.WaitForAssertion(() =>
        {
            Assert.That(page.Markup, Does.Contain(Resources.DashboardUptime));
            Assert.That(page.Markup, Does.Contain(Resources.DashboardConfigurationUpdates));
        });
    }

    /// <summary>A failure in one statistics source does not hide the other source.</summary>
    [TestCase(true)]
    [TestCase(false)]
    public void ShowsAvailableStatisticsWhenOtherSourceFails(bool accountFailure)
    {
        using var context = CreateContext(isAdministrator: false);
        var persistence = Mock.Get(context.Services.GetRequiredService<IPersistenceContextProvider>());
        if (accountFailure)
        {
            persistence.Setup(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>())).Throws(new InvalidOperationException("Unavailable accounts"));
        }
        else
        {
            persistence.Setup(p => p.CreateNewGuildContext()).Throws(new InvalidOperationException("Unavailable guilds"));
        }

        var page = context.Render<Index>();
        page.WaitForAssertion(() =>
        {
            var values = page.FindAll(".statistic-value").Select(v => v.TextContent.Trim()).ToList();
            Assert.That(values[accountFailure ? 3 : 2], Is.EqualTo((accountFailure ? 56 : 1234).ToString("N0", CultureInfo.CurrentCulture)));
            Assert.That(values[accountFailure ? 2 : 3], Is.EqualTo(Resources.DashboardNotAvailable));
        });
    }

    private static BunitContext CreateContext(bool isAdministrator)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var authorization = context.AddAuthorization();
        authorization.SetAuthorized("user");
        if (isAdministrator)
        {
            authorization.SetPolicies(AdminPolicies.Administrator);
        }

        var serverProvider = TestServers.CreateProvider(
            TestServers.Create(ServerType.GameServer, ServerState.Started, 10),
            TestServers.Create(ServerType.GameServer, ServerState.Stopped, 5),
            TestServers.Create(ServerType.ConnectServer, ServerState.Started, 30));

        var playerContext = new Mock<IPlayerContext>();
        playerContext.Setup(c => c.GetAccountStatisticsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountStatistics(1234, 5, 2, 2000));
        var configurations = new Mock<IDataSource<GameConfiguration>>();
        configurations.Setup(c => c.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GameConfiguration());

        var guildContext = new Mock<IGuildServerContext>();
        guildContext.Setup(c => c.GetGuildStatisticsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new GuildStatistics(56, 3));
        var contextProvider = new Mock<IMigratableDatabaseContextProvider>();
        contextProvider.Setup(p => p.CreateNewGuildContext()).Returns(guildContext.Object);
        contextProvider.Setup(p => p.CreateNewPlayerContext(It.IsAny<GameConfiguration>())).Returns(playerContext.Object);
        contextProvider.Setup(p => p.CanConnectToDatabaseAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);

        context.Services.AddSingleton(serverProvider);
        context.Services.AddSingleton(new OnlinePlayerHistory(serverProvider, NullLogger<OnlinePlayerHistory>.Instance));
        context.Services.AddSingleton(configurations.Object);
        context.Services.AddSingleton(Mock.Of<ILoginServer>());
        context.Services.AddSingleton<IAdminReadModel, TestReadModel>();
        context.Services.AddAdminReadOperations();
        context.Services.AddSingleton(contextProvider.Object);
        context.Services.AddSingleton<IPersistenceContextProvider>(contextProvider.Object);
        context.Services.AddSingleton(new DataUpdateService(contextProvider.Object, plugInManager));
        context.Services.AddSingleton(new CastleSiegeManagementService(serverProvider));
        context.Services.AddSingleton<NavigationHistory>();
        context.Services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        return context;
    }
}
