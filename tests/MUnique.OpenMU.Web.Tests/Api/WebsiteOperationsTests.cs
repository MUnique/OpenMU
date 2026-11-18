// <copyright file="WebsiteOperationsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Threading;
using Moq;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Operations.Website;
using MUnique.OpenMU.Web.Tests.Mcp;

/// <summary>
/// Tests for the <see cref="WebsiteOperations"/>, which a website uses with a key of the
/// <see cref="AdminRoles.WebManager"/> role.
/// </summary>
[TestFixture]
public class WebsiteOperationsTests
{
    /// <summary>
    /// The website role stands alone: an administrator doesn't have it, so an administration key can't use the website API.
    /// </summary>
    [Test]
    public void WebManagerIsNotImpliedByOtherRoles()
    {
        Assert.That(AdminRoles.GetEffectiveRoles(AdminRoles.WebManager), Is.EquivalentTo(new[] { AdminRoles.WebManager }));
        Assert.That(AdminRoles.GetEffectiveRoles(AdminRoles.Administrator), Does.Not.Contain(AdminRoles.WebManager));
        Assert.That(AdminRoles.All, Does.Not.Contain(AdminRoles.WebManager));
    }

    /// <summary>
    /// A website key without a selected function can't do anything, unlike an MCP key with an empty allowlist.
    /// </summary>
    [Test]
    public void KeyWithoutSelectedFunctionIsRefused()
    {
        var operations = CreateOperations(WebManager(string.Empty));

        var error = Assert.ThrowsAsync<AdminOperationException>(async () => await operations.StatusAsync(CancellationToken.None).ConfigureAwait(false));
        Assert.That(error!.Kind, Is.EqualTo(AdminOperationErrorKind.Forbidden));
    }

    /// <summary>
    /// A website key may only use the functions which were selected for it.
    /// </summary>
    [Test]
    public void KeyWithOtherFunctionIsRefused()
    {
        var operations = CreateOperations(WebManager("website_register"));

        var error = Assert.ThrowsAsync<AdminOperationException>(async () => await operations.StatusAsync(CancellationToken.None).ConfigureAwait(false));
        Assert.That(error!.Kind, Is.EqualTo(AdminOperationErrorKind.Forbidden));
    }

    /// <summary>
    /// An administration key can't use the website API.
    /// </summary>
    [Test]
    public void AdministratorIsRefused()
    {
        var operations = CreateOperations(new AdminActor("admin-key", "admin", "api", [AdminRoles.Administrator]));

        var error = Assert.ThrowsAsync<AdminOperationException>(async () => await operations.StatusAsync(CancellationToken.None).ConfigureAwait(false));
        Assert.That(error!.Kind, Is.EqualTo(AdminOperationErrorKind.Forbidden));
    }

    /// <summary>
    /// The status lists the game servers with their players, but no other servers.
    /// </summary>
    [Test]
    public async Task StatusListsGameServersAsync()
    {
        var operations = CreateOperations(WebManager(WebsitePermissions.Status), GameServer(1, "Server 1", ServerState.Started, 12), GameServer(0, "Server 0", ServerState.Stopped, 0), Mock.Of<IManageableServer>());

        var status = await operations.StatusAsync(CancellationToken.None).ConfigureAwait(false);

        Assert.That(status.Players, Is.EqualTo(12));
        Assert.That(status.Servers.Select(s => (s.Id, s.Name, s.IsOnline, s.Players)), Is.EqualTo(new[] { (0, "Server 0", false, 0), (1, "Server 1", true, 12) }));
    }

    private static AdminActor WebManager(string allowedFunctions)
        => new("website-key", "website", "api", [AdminRoles.WebManager]) { AllowedToolGroups = allowedFunctions };

    private static IManageableServer GameServer(int id, string name, ServerState state, int players)
    {
        var server = new Mock<IGameServer>();
        server.Setup(s => s.Id).Returns(id);
        server.Setup(s => s.Description).Returns(name);
        server.Setup(s => s.ServerState).Returns(state);
        server.Setup(s => s.CurrentConnections).Returns(players);
        server.Setup(s => s.MaximumConnections).Returns(100);
        return server.Object;
    }

    private static WebsiteOperations CreateOperations(AdminActor actor, params IManageableServer[] servers)
    {
        var serverProvider = new Mock<IServerProvider>();
        serverProvider.Setup(p => p.Servers).Returns(servers.ToList());
        return new WebsiteOperations(serverProvider.Object, AdminOperationTestServices.CreateExecutor("api", actor: () => actor));
    }
}
