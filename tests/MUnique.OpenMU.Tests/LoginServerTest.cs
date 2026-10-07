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
}
