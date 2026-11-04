// <copyright file="OfflineAccountReservationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.LoginServer;

/// <summary>Verifies login exclusion and reservation ownership for offline administration.</summary>
[TestFixture]
public sealed class OfflineAccountReservationTests
{
    /// <summary>Online accounts cannot be reserved and reserved accounts cannot log in.</summary>
    [Test]
    public async Task ReservationsExcludeLoginsAndRespectOwnershipAsync()
    {
        var login = new LoginServer();
        Assert.That(await login.TryLoginAsync("online", 4).ConfigureAwait(false), Is.True);
        Assert.That(await login.TryReserveAccountAsync("online", 1000).ConfigureAwait(false), Is.False);
        Assert.That(await login.TryReserveAccountAsync("offline", 1000).ConfigureAwait(false), Is.True);
        Assert.That(await login.TryReserveAccountAsync("offline", 1001).ConfigureAwait(false), Is.False);
        Assert.That(await login.TryLoginAsync("offline", 4).ConfigureAwait(false), Is.False);
        Assert.That(await login.GetSnapshotAsync().ConfigureAwait(false), Does.Not.ContainKey("offline"));
        await login.ReleaseAccountReservationAsync("offline", 1001).ConfigureAwait(false);
        Assert.That(await login.TryLoginAsync("offline", 4).ConfigureAwait(false), Is.False);
        await login.ReleaseAccountReservationAsync("offline", 1000).ConfigureAwait(false);
        Assert.That(await login.TryLoginAsync("offline", 4).ConfigureAwait(false), Is.True);
    }

    /// <summary>A simultaneous login and reservation can have exactly one winner.</summary>
    [Test]
    public async Task ConcurrentLoginAndReservationHaveOneWinnerAsync()
    {
        var login = new LoginServer();
        var results = await Task.WhenAll(
            Task.Run(async () => await login.TryLoginAsync("racing", 4).ConfigureAwait(false)),
            Task.Run(async () => await login.TryReserveAccountAsync("racing", 1000).ConfigureAwait(false))).ConfigureAwait(false);
        Assert.That(results.Count(r => r), Is.EqualTo(1));
    }
}
