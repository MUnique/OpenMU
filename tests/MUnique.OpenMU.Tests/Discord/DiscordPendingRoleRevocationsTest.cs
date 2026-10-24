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
    /// <summary>
    /// Tests that the users, for which the removal fails (e.g. the bot isn't connected), are retried later.
    /// </summary>
    [Test]
    public async Task FailedRevocationsStayPendingAsync()
    {
        var revocations = new DiscordPendingRoleRevocations();
        revocations.Add(1);
        revocations.Add(2);
        revocations.Add(2);
        Assert.That(revocations.Count, Is.EqualTo(2));

        var unavailable = await revocations.ProcessAsync(_ => Task.FromResult(false)).ConfigureAwait(false);
        Assert.That(unavailable, Is.Zero);
        Assert.That(revocations.Count, Is.EqualTo(2));

        var partial = await revocations.ProcessAsync(userId => Task.FromResult(userId == 1)).ConfigureAwait(false);
        Assert.That(partial, Is.EqualTo(1));
        Assert.That(revocations.Count, Is.EqualTo(1));

        var revoked = new List<ulong>();
        await revocations.ProcessAsync(userId =>
        {
            revoked.Add(userId);
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
        revocations.Add(1);
        await revocations.LinkAsync(1, () => Task.CompletedTask).ConfigureAwait(false);
        var called = false;
        await revocations.ProcessAsync(_ =>
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
        revocations.Add(1);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var resume = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var operations = new List<string>();

        var processing = revocations.ProcessAsync(async _ =>
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
        revocations.Add(1);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var resume = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var processing = revocations.ProcessAsync(async _ =>
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
}
