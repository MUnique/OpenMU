// <copyright file="CastleSiegePanelErrorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>Operational audit errors remain visible in the Castle Siege panel.</summary>
[TestFixture]
public sealed class CastleSiegePanelErrorTests
{
    /// <summary>The workflow shows both pre-action failures and uncertain outcomes without replacing their guidance.</summary>
    [TestCase(false, "Audit storage is unavailable")]
    [TestCase(true, "Check the current state before retrying")]
    public async Task DisplaysSafeOperationalGuidanceAsync(bool uncertainOutcome, string guidance)
    {
        using var context = new BunitContext();
        var audit = new InMemoryAdminAuditRepository { RejectWrites = !uncertainOutcome, RejectCompletion = uncertainOutcome };
        var persistence = new InMemoryPersistenceContextProvider();
        GameConfiguration configuration;
        var calendarDay = DateTime.UtcNow.Date;
        using (var storage = persistence.CreateNewContext())
        {
            configuration = storage.CreateNew<GameConfiguration>();
            var siege = storage.CreateNew<CastleSiegeConfiguration>();
            siege.Enabled = true;
            configuration.CastleSiegeConfiguration = siege;
            var registration = storage.CreateNew<CastleSiegeStateScheduleEntry>();
            registration.State = CastleSiegeState.RegisterGuild;
            registration.DayOfWeek = calendarDay.AddDays(-1).DayOfWeek;
            siege.StateSchedule.Add(registration);
            var end = storage.CreateNew<CastleSiegeStateScheduleEntry>();
            end.State = CastleSiegeState.End;
            end.DayOfWeek = calendarDay.AddDays(2).DayOfWeek;
            siege.StateSchedule.Add(end);
            await storage.SaveChangesAsync().ConfigureAwait(false);
        }

        using var loggers = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var plugin = new CastleSiegePlugIn();
        var plugins = new PlugInManager(null, loggers, null, null);
        plugins.RegisterPlugInAtPlugInPoint<IPeriodicTaskPlugIn>(plugin);
        var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        await using var game = new GameServerContext(
            new MUnique.OpenMU.Persistence.BasicModel.GameServerDefinition { ServerID = 4, GameConfiguration = configuration, ServerConfiguration = new MUnique.OpenMU.Persistence.BasicModel.GameServerConfiguration() },
            Mock.Of<IGuildServer>(), Mock.Of<IEventPublisher>(), Mock.Of<ILoginServer>(), Mock.Of<IFriendServer>(), persistence,
            initializer, loggers, plugins, NullDropGenerator.Instance, new ConfigurationChangeMediator()) { ServerTimeZone = TimeZoneInfo.Utc };
        game.StopPeriodicTasks();
        await plugin.ExecuteTaskAsync(game).ConfigureAwait(false);
        var server = new Mock<IGameServer>();
        server.As<IGameServerContextProvider>().SetupGet(s => s.Context).Returns(game);
        server.SetupGet(s => s.Id).Returns(4);
        server.SetupGet(s => s.Type).Returns(ServerType.GameServer);
        var provider = new Mock<IServerProvider>();
        provider.SetupGet(p => p.Servers).Returns(new List<IManageableServer> { server.Object });
        var management = new CastleSiegeManagementService(provider.Object);
        var snapshot = await management.GetSnapshotAsync(4).ConfigureAwait(false);
        var initial = snapshot.Snapshot;
        Assert.That(initial, Is.Not.Null, "The public siege fixture must initialize before exercising the panel: " + snapshot.Error);
        var targetState = initial!.State == CastleSiegeState.RegisterGuild ? CastleSiegeState.End : CastleSiegeState.RegisterGuild;
        var toast = new Mock<IToastService>();
        context.Services.AddLogging();
        context.Services.AddSingleton<NavigationHistory>();
        context.Services.AddSingleton(management);
        context.Services.AddSingleton<IToastService>(toast.Object);
        context.Services.AddSingleton<IModalService>(new TestModalService());
        context.Services.AddSingleton(AdminOperationTestServices.CreateExecutor(audit: audit));
        var component = context.Render<CastleSiegeManagement>();
        component.Find("#castle-siege-state").Change(targetState.ToString());
        await component.Find("#castle-siege-state + button").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()).ConfigureAwait(false);
        await plugin.ExecuteTaskAsync(game).ConfigureAwait(false);
        toast.Verify(t => t.ShowError(It.Is<string>(message => message.Contains(guidance, StringComparison.Ordinal)), null), Times.Once);
        toast.Verify(t => t.ShowSuccess(It.IsAny<string>(), It.IsAny<string?>()), Times.Never);
        Assert.That(plugin.GetContext(game)!.CurrentState, Is.EqualTo(uncertainOutcome ? targetState : initial.State));
        Assert.That(component.Find("h1"), Is.Not.Null);
        if (uncertainOutcome)
        {
            Assert.That((await audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single().Outcome, Is.EqualTo("Pending"));
        }
    }
}
