// <copyright file="IndexPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Dashboard;

using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Services;

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
        var accountSource = new Mock<IDataSource<Account>>();
        accountSource.Setup(s => s.GetContextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(playerContext.Object);

        var guildContext = new Mock<IGuildServerContext>();
        guildContext.Setup(c => c.GetGuildStatisticsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new GuildStatistics(56, 3));
        var contextProvider = new Mock<IMigratableDatabaseContextProvider>();
        contextProvider.Setup(p => p.CreateNewGuildContext()).Returns(guildContext.Object);
        contextProvider.Setup(p => p.CanConnectToDatabaseAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);

        context.Services.AddSingleton(serverProvider);
        context.Services.AddSingleton(new OnlinePlayerHistory(serverProvider, NullLogger<OnlinePlayerHistory>.Instance));
        context.Services.AddSingleton(accountSource.Object);
        context.Services.AddSingleton(contextProvider.Object);
        context.Services.AddSingleton<IPersistenceContextProvider>(contextProvider.Object);
        context.Services.AddSingleton(new DataUpdateService(contextProvider.Object, plugInManager));
        context.Services.AddSingleton(new CastleSiegeManagementService(serverProvider));
        context.Services.AddSingleton<NavigationHistory>();
        context.Services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        return context;
    }
}
