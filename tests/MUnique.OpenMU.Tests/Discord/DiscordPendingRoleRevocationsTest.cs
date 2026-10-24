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
        revocations.Remove(1);
        var called = false;
        await revocations.ProcessAsync(_ =>
        {
            called = true;
            return Task.FromResult(true);
        }).ConfigureAwait(false);
        Assert.That(called, Is.False);
    }
}
