// <copyright file="DiscordPendingRoleRevocationsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using MUnique.OpenMU.Discord;

/// <summary>
/// Tests the retries of removing the role of linked players, e.g. after a link was removed in the admin panel.
/// </summary>
[TestFixture]
public class DiscordPendingRoleRevocationsTest
{
    private static readonly Func<ulong, Task<bool?>> NotLinked = _ => Task.FromResult<bool?>(false);

    /// <summary>
    /// Tests that the users, for which the removal fails (e.g. the bot isn't connected), are retried later.
    /// </summary>
    [Test]
    public async Task FailedRevocationsStayPendingAsync()
    {
        var revocations = new DiscordPendingRoleRevocations();
        revocations.Add(1, null);
        revocations.Add(2, null);
        revocations.Add(2, null);
        Assert.That(revocations.Count, Is.EqualTo(2));

        var unavailable = await revocations.ProcessAsync(NotLinked, _ => Task.FromResult(false)).ConfigureAwait(false);
        Assert.That(unavailable, Is.Zero);
        Assert.That(revocations.Count, Is.EqualTo(2));

        var partial = await revocations.ProcessAsync(NotLinked, entry => Task.FromResult(entry.UserId == 1)).ConfigureAwait(false);
        Assert.That(partial, Is.EqualTo(1));
        Assert.That(revocations.Count, Is.EqualTo(1));

        var revoked = new List<ulong>();
        await revocations.ProcessAsync(NotLinked, entry =>
        {
            revoked.Add(entry.UserId);
            return Task.FromResult(true);
        }).ConfigureAwait(false);
        Assert.That(revoked, Is.EqualTo(new ulong[] { 2 }));
        Assert.That(revocations.Count, Is.Zero);
    }

    /// <summary>
    /// Tests that a user, who was linked again, isn't revoked anymore.
    /// </summary>
    [Test]
    public async Task RelinkedUserIsNotRevokedAsync()
    {
        var revocations = new DiscordPendingRoleRevocations();
        revocations.Add(1, null);
        await revocations.LinkAsync(1, () => Task.CompletedTask).ConfigureAwait(false);
        var called = false;
        await revocations.ProcessAsync(NotLinked, _ =>
        {
            called = true;
            return Task.FromResult(true);
        }).ConfigureAwait(false);
        Assert.That(called, Is.False);
        Assert.That(revocations.Count, Is.Zero);
    }

    /// <summary>
    /// Tests that a link during a running removal adds the role after the removal, so that the linked user keeps the role.
    /// </summary>
    [Test]
    public async Task LinkDuringRunningRevocationAddsRoleAfterwardsAsync()
    {
        var revocations = new DiscordPendingRoleRevocations();
        revocations.Add(1, null);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var resume = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var operations = new List<string>();

        var processing = revocations.ProcessAsync(NotLinked, async _ =>
        {
            started.SetResult();
            await resume.Task.ConfigureAwait(false);
            lock (operations)
            {
                operations.Add("remove");
            }

            return true;
        });
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

        var linking = revocations.LinkAsync(1, () =>
        {
            lock (operations)
            {
                operations.Add("add");
            }

            return Task.CompletedTask;
        });
        Assert.That(linking.IsCompleted, Is.False);

        resume.SetResult();
        await Task.WhenAll(processing, linking).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(operations, Is.EqualTo(new[] { "remove", "add" }));
        Assert.That(revocations.Count, Is.Zero);
    }

    /// <summary>
    /// Tests that a running removal for one user doesn't block the link of another user.
    /// </summary>
    [Test]
    public async Task RunningRevocationDoesNotBlockOtherUsersAsync()
    {
        var revocations = new DiscordPendingRoleRevocations();
        revocations.Add(1, null);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var resume = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var processing = revocations.ProcessAsync(NotLinked, async _ =>
        {
            started.SetResult();
            await resume.Task.ConfigureAwait(false);
            return true;
        });
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

        var added = false;
        await revocations.LinkAsync(2, () =>
        {
            added = true;
            return Task.CompletedTask;
        }).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(added, Is.True);
        Assert.That(processing.IsCompleted, Is.False);

        resume.SetResult();
        await processing.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(revocations.Count, Is.Zero);
    }

    /// <summary>
    /// Tests that an unlink, which is delivered after the user was linked again, doesn't remove the role,
    /// and that a user stays pending when the link can't be checked.
    /// </summary>
    [Test]
    public async Task StaleUnlinkAfterRelinkKeepsRoleAsync()
    {
        var revocations = new DiscordPendingRoleRevocations();
        await revocations.LinkAsync(1, () => Task.CompletedTask).ConfigureAwait(false);

        // The late unlink event.
        revocations.Add(1, null);
        var revoked = false;
        Func<DiscordPendingRoleRevocation, Task<bool>> revokeAsync = _ =>
        {
            revoked = true;
            return Task.FromResult(true);
        };

        Assert.That(await revocations.ProcessAsync(_ => Task.FromResult<bool?>(null), revokeAsync).ConfigureAwait(false), Is.Zero);
        Assert.That(revocations.Count, Is.EqualTo(1));

        Assert.That(await revocations.ProcessAsync(_ => Task.FromResult<bool?>(true), revokeAsync).ConfigureAwait(false), Is.Zero);
        Assert.That(revoked, Is.False);
        Assert.That(revocations.Count, Is.Zero);
    }

    /// <summary>
    /// Tests that a removal keeps its Discord server, e.g. when the configured server changed in the meantime,
    /// and that a link removes the pending removals of the user in all servers.
    /// </summary>
    [Test]
    public async Task RevocationKeepsItsDiscordServerAsync()
    {
        var revocations = new DiscordPendingRoleRevocations();
        revocations.Add(1, 100);
        revocations.Add(1, 200);
        revocations.Add(2, 100);

        var revoked = new List<DiscordPendingRoleRevocation>();
        await revocations.ProcessAsync(NotLinked, entry =>
        {
            revoked.Add(entry);
            return Task.FromResult(entry.GuildId == 200);
        }).ConfigureAwait(false);
        Assert.That(revoked, Is.EquivalentTo(new[] { new DiscordPendingRoleRevocation(1, 100), new DiscordPendingRoleRevocation(1, 200), new DiscordPendingRoleRevocation(2, 100) }));
        Assert.That(revocations.Entries, Is.EquivalentTo(new[] { new DiscordPendingRoleRevocation(1, 100), new DiscordPendingRoleRevocation(2, 100) }));

        await revocations.LinkAsync(1, () => Task.CompletedTask).ConfigureAwait(false);
        Assert.That(revocations.Entries, Is.EqualTo(new[] { new DiscordPendingRoleRevocation(2, 100) }));
    }

    /// <summary>
    /// Tests that a removal keeps the name of the role, e.g. when the layout changes in the meantime.
    /// </summary>
    [Test]
    public async Task RevocationKeepsTheNameOfTheRoleAsync()
    {
        var revocations = new DiscordPendingRoleRevocations();
        revocations.Add(1, 100, "Old Linked Player");

        DiscordPendingRoleRevocation? revoked = null;
        await revocations.ProcessAsync(NotLinked, entry =>
        {
            revoked = entry;
            return Task.FromResult(false);
        }).ConfigureAwait(false);
        Assert.That(revoked, Is.EqualTo(new DiscordPendingRoleRevocation(1, 100, "Old Linked Player")));
        Assert.That(revocations.Count, Is.EqualTo(1), "a server which the bot can't reach keeps the removal pending");
    }
}
