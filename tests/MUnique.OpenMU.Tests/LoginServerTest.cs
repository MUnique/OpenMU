// <copyright file="LoginServerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

/// <summary>
/// Tests for the <see cref="LoginServer.LoginServer"/>.
/// </summary>
[TestFixture]
public class LoginServerTest
{
    /// <summary>
    /// Tests that a log off of another server doesn't remove the login of the account.
    /// Previously, any log off removed it, so the account could be logged in a second time.
    /// </summary>
    [Test]
    public async ValueTask LogOffOfAnotherServerKeepsTheLoginAsync()
    {
        var loginServer = new LoginServer.LoginServer();
        Assert.That(await loginServer.TryLoginAsync("account", 1).ConfigureAwait(false), Is.True);

        await loginServer.LogOffAsync("account", 2).ConfigureAwait(false);

        Assert.That(await loginServer.TryLoginAsync("account", 2).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that a log off of the server of the login removes it.
    /// </summary>
    [Test]
    public async ValueTask LogOffOfTheServerRemovesTheLoginAsync()
    {
        var loginServer = new LoginServer.LoginServer();
        Assert.That(await loginServer.TryLoginAsync("account", 1).ConfigureAwait(false), Is.True);

        await loginServer.LogOffAsync("account", 1).ConfigureAwait(false);

        Assert.That(await loginServer.TryLoginAsync("account", 2).ConfigureAwait(false), Is.True);
    }
    /// <summary>A crashed caller cannot keep an account reserved indefinitely or release a replacement lease.</summary>
    [Test]
    public async Task ExpiredReservationAllowsLoginAndPreservesNewOwnerAsync()
    {
        var clock = new ReservationClock();
        var server = new LoginServer.LoginServer(clock);
        Assert.That(await server.TryReserveAccountAsync("account", 1000).ConfigureAwait(false), Is.True);
        Assert.That(await server.TryLoginAsync("account", 1).ConfigureAwait(false), Is.False);
        clock.Now = clock.Now.AddMinutes(6);
        Assert.That(await server.TryReserveAccountAsync("account", 1001).ConfigureAwait(false), Is.True);
        await server.ReleaseAccountReservationAsync("account", 1000).ConfigureAwait(false);
        Assert.That(await server.TryLoginAsync("account", 1).ConfigureAwait(false), Is.False);
        clock.Now = clock.Now.AddMinutes(6);
        Assert.That(await server.TryLoginAsync("account", 1).ConfigureAwait(false), Is.True);
        clock.Now = clock.Now.AddMinutes(10);
        Assert.That(await server.TryLoginAsync("account", 2).ConfigureAwait(false), Is.False);
    }

    private sealed class ReservationClock : TimeProvider
    {
        internal DateTimeOffset Now { get; set; } = DateTimeOffset.UtcNow;

        public override DateTimeOffset GetUtcNow() => this.Now;
    }
}
