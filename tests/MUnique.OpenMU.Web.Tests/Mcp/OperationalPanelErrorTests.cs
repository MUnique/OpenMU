// <copyright file="OperationalPanelErrorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using Bunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Components;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>Expected operational failures stay within the component or HTTP boundary.</summary>
[TestFixture]
public sealed class OperationalPanelErrorTests
{
    /// <summary>Audit failures in start, stop and confirmed delete handlers show a toast without losing the circuit.</summary>
    [TestCase(ServerState.Stopped, "btn-success")]
    [TestCase(ServerState.Started, "btn-warning")]
    [TestCase(ServerState.Stopped, "btn-danger")]
    public async Task ServerControlsContainAuditFailuresAsync(ServerState state, string buttonClass)
    {
        using var context = new BunitContext();
        var server = new Mock<IGameServer>();
        server.SetupGet(s => s.Id).Returns(4);
        server.SetupGet(s => s.Type).Returns(ServerType.GameServer);
        server.SetupGet(s => s.ServerState).Returns(state);
        var provider = new Mock<IServerProvider>();
        provider.SetupGet(p => p.Servers).Returns(new List<IManageableServer> { server.Object });
        var toast = new Mock<IToastService>();
        var manager = new Mock<IGameServerInstanceManager>();
        context.Services.AddSingleton<IToastService>(toast.Object);
        context.Services.AddSingleton(manager.Object);
        context.Services.AddSingleton(Mock.Of<IConnectServerInstanceManager>());
        context.Services.AddSingleton(Mock.Of<IPersistenceContextProvider>());
        context.Services.AddSingleton(Mock.Of<IDataSource<GameConfiguration>>());
        context.Services.AddSingleton<IModalService>(new TestModalService(true));
        context.Services.AddSingleton(new ServerWriteOperations(provider.Object, AdminOperationTestServices.CreateExecutor(audit: new InMemoryAdminAuditRepository { RejectWrites = true }), new AdminWriteSettings()));
        var component = context.Render<ServerItem>(parameters => parameters.Add(p => p.Server, server.Object));
        await component.Find("button." + buttonClass).ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()).ConfigureAwait(false);
        toast.Verify(t => t.ShowError(It.Is<string>(message => message.Contains("Audit storage is unavailable", StringComparison.Ordinal)), null), Times.Once);
        server.Verify(s => s.StartAsync(), Times.Never);
        server.Verify(s => s.ShutdownAsync(), Times.Never);
        manager.Verify(m => m.RemoveGameServerAsync(It.IsAny<byte>()), Times.Never);
        Assert.That(component.FindAll("button"), Is.Not.Empty);
    }

    /// <summary>Account creation reports an audit outage without escaping its UI handler or touching game data.</summary>
    [Test]
    public async Task AccountCreationContainsAuditFailureAsync()
    {
        var source = new Mock<IDataSource<Account>>();
        var toast = new Mock<IToastService>();
        using var service = new AccountService(source.Object, new TestModalService(true),
            AdminOperationTestServices.CreateExecutor(audit: new InMemoryAdminAuditRepository { RejectWrites = true }), toast.Object);
        await service.CreateNewInModalDialogAsync().ConfigureAwait(false);
        toast.Verify(t => t.ShowError(It.Is<string>(message => message.Contains("Audit storage is unavailable", StringComparison.Ordinal)), null), Times.Once);
        source.Verify(s => s.GetContextAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Never);
    }

    /// <summary>Bad announcement arguments become safe HTTP 400 responses and never reach a server.</summary>
    [TestCase("missing")]
    [TestCase("empty")]
    [TestCase("long")]
    public void InvalidAnnouncementsHaveClientErrorStatus(string scenario)
    {
        var server = new Mock<IGameServer>();
        server.SetupGet(s => s.Id).Returns(4);
        var provider = new Mock<IServerProvider>();
        provider.SetupGet(p => p.Servers).Returns(new List<IManageableServer> { server.Object });
        var operations = new ServerWriteOperations(provider.Object, AdminOperationTestServices.CreateExecutor(frontend: "api"), new AdminWriteSettings());
        var error = Assert.ThrowsAsync<AdminOperationException>(async () => await operations.SendMessageAsync(scenario == "missing" ? 9 : 4, scenario == "long" ? new string('x', 513) : scenario == "empty" ? "" : "Hello", MessageType.GoldenCenter, default).ConfigureAwait(false));
        var boundary = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>()) { Exception = error! };
        new AdminOperationExceptionFilter().OnException(boundary);
        Assert.That(((ObjectResult)boundary.Result!).StatusCode, Is.EqualTo(400));
        server.Verify(s => s.SendGlobalMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()), Times.Never);
    }
}
