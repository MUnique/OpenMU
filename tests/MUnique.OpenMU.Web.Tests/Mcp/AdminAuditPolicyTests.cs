// <copyright file="AdminAuditPolicyTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Checks the narrowly scoped setup/demo panel audit fallback.</summary>
[TestFixture]
public sealed class AdminAuditPolicyTests
{
    /// <summary>Only initial setup and explicit demo panel callers may proceed with a log-only audit.</summary>
    [TestCase("panel", true, false, true)]
    [TestCase("panel", false, true, true)]
    [TestCase("panel", false, false, false)]
    [TestCase("mcp", true, true, false)]
    [TestCase("api", true, true, false)]
    public async Task AuditOutageRespectsFrontendAndHostPolicyAsync(string frontend, bool setup, bool demo, bool allowed)
    {
        var messages = new List<string>();
        var logger = new Mock<ILogger<AdminOperationExecutor>>();
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(invocation => messages.Add(invocation.Arguments[2].ToString()!)));
        var audit = new InMemoryAdminAuditRepository { RejectWrites = true };
        var executor = AdminOperationTestServices.CreateExecutor(frontend, audit,
            () => new AdminActor(setup ? "setup" : "admin", "Test caller", frontend, [AdminRoles.Administrator]) { IsInitialSetup = setup }, demo, logger.Object);
        var executed = false;
        async ValueTask<bool> RunAsync() => await executor.ExecuteAsync(AdminRoles.Operator, "server_stop", new { serverId = 4, password = "private-test-password" }, _ =>
        {
            executed = true;
            return ValueTask.FromResult(true);
        }, default).ConfigureAwait(false);
        if (allowed)
        {
            Assert.That(await RunAsync().ConfigureAwait(false), Is.True);
            Assert.That(string.Join('\n', messages), Does.Contain("Non-durable panel audit attempt").And.Contain("Non-durable panel audit outcome").And.Contain("Succeeded").And.Contain("[redacted]").And.Not.Contain("private-test-password"));
        }
        else
        {
            var error = Assert.ThrowsAsync<AdminOperationException>(async () => await RunAsync().ConfigureAwait(false));
            Assert.That(error!.Kind, Is.EqualTo(AdminOperationErrorKind.Unavailable));
        }

        Assert.That(executed, Is.EqualTo(allowed));
        Assert.That((await audit.ReadAsync(0, 100).ConfigureAwait(false)).Total, Is.Zero);
    }

    /// <summary>Setup writes still use durable storage when it is available.</summary>
    [Test]
    public async Task SetupUsesDurableAuditWhenAvailableAsync()
    {
        var audit = new InMemoryAdminAuditRepository();
        var executor = AdminOperationTestServices.CreateExecutor(audit: audit,
            actor: () => new AdminActor("setup", "Initial setup", "panel", [AdminRoles.Administrator]) { IsInitialSetup = true });
        await executor.ExecuteAsync(AdminRoles.Operator, "server_start", new { serverId = 4 }, _ => ValueTask.FromResult(true), default).ConfigureAwait(false);
        var entry = (await audit.ReadAsync(0, 100).ConfigureAwait(false)).Items.Single();
        Assert.That(entry.PrincipalId, Is.EqualTo("setup"));
        Assert.That(entry.Frontend, Is.EqualTo("panel"));
        Assert.That(entry.Outcome, Is.EqualTo("Succeeded"));
    }
}
