// <copyright file="ChangeConfirmationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Mcp;

using System.Text.Json.Nodes;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Verifies expiry, single-use confirmation and strict allowlist matching.</summary>
[TestFixture]
public sealed class ChangeConfirmationTests
{
    /// <summary>Expiry and process replacement require a fresh preview.</summary>
    [Test]
    public void ExpiredAndRestartedConfirmationsAreRefused()
    {
        var clock = new MutableClock();
        var confirmations = new AdminChangeConfirmation(clock);
        var actor = new AdminActor("key", "admin", "mcp", [AdminRoles.Administrator]);
        var change = new JsonObject { ["before"] = 1, ["after"] = 2 };
        var preview = confirmations.Preview(actor, "config_update", new { id = 1 }, change);
        var token = preview["confirmationToken"]!.GetValue<string>();
        Assert.Throws<AdminOperationException>(() => new AdminChangeConfirmation(clock).Consume(actor, "config_update", new { id = 1 }, change, token));
        clock.Now = clock.Now.AddMinutes(6);
        Assert.Throws<AdminOperationException>(() => confirmations.Consume(actor, "config_update", new { id = 1 }, change, token));
    }

    /// <summary>Even simultaneous matching applies can consume a token only once.</summary>
    [Test]
    public async Task ConfirmationIsAtomicallySingleUseAsync()
    {
        var confirmations = new AdminChangeConfirmation(TimeProvider.System);
        var actor = new AdminActor("key", "admin", "mcp", [AdminRoles.Administrator]);
        var change = new JsonObject { ["before"] = 1, ["after"] = 2 };
        var token = confirmations.Preview(actor, "config_update", new { id = 1 }, change)["confirmationToken"]!.GetValue<string>();
        var results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
        {
            try
            {
                confirmations.Consume(actor, "config_update", new { id = 1 }, change, token);
                return true;
            }
            catch (AdminOperationException)
            {
                return false;
            }
        }))).ConfigureAwait(false);
        Assert.That(results.Count(r => r), Is.EqualTo(1));
    }

    /// <summary>Only exact names and explicit area wildcards narrow a key.</summary>
    [TestCase("", "config_update", true)]
    [TestCase("players_*, server_overview", "players_ban", true)]
    [TestCase("players_*, server_overview", "server_stop", false)]
    [TestCase("accounts_get", "accounts_get_extra", false)]
    [TestCase("castle_siege_*", "castle_siege_queue_state", true)]
    [TestCase("*", "config_update", false)]
    [TestCase("config_*,", "config_update", false)]
    [TestCase("config*", "config_update", false)]
    public void ToolGroupsFailClosed(string groups, string tool, bool allowed)
        => Assert.That(AdminToolGroups.Allows(groups, tool), Is.EqualTo(allowed));

    /// <summary>A caller cannot exhaust another caller's preview capacity, even concurrently.</summary>
    [Test]
    public async Task PreviewCapacityIsIsolatedPerCallerAsync()
    {
        var confirmations = new AdminChangeConfirmation(TimeProvider.System);
        var actor = new AdminActor("busy-key", "admin", "mcp", [AdminRoles.Administrator]);
        var issued = await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => Task.Run(() =>
        {
            try
            {
                confirmations.Preview(actor, "config_update", new { id = 1 }, new JsonObject());
                return true;
            }
            catch (AdminOperationException)
            {
                return false;
            }
        }))).ConfigureAwait(false);
        Assert.That(issued.Count(i => i), Is.EqualTo(64));
        Assert.DoesNotThrow(() => confirmations.Preview(actor with { Id = "other-key" }, "config_update", new { id = 1 }, new JsonObject()));
    }

    private sealed class MutableClock : TimeProvider
    {
        internal DateTimeOffset Now { get; set; } = new(2026, 10, 24, 0, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => this.Now;
    }
}
