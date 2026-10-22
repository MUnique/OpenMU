// <copyright file="DiscordBotTokenTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using MUnique.OpenMU.Discord;

/// <summary>
/// Tests for the <see cref="DiscordBotToken"/>.
/// </summary>
[TestFixture]
public class DiscordBotTokenTest
{
    /// <summary>
    /// Tests that the identifier of the application is read from the first part of the token, which is
    /// base64 without padding.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="expected">The expected identifier.</param>
    [TestCase("MTIzNDU2Nzg5MDEyMzQ1Njc4.GxYzAb.c2VjcmV0", 123456789012345678UL)]
    [TestCase("MTIzNDU2Nzg5MDEyMzQ1Njc.a.b", 12345678901234567UL)]
    [TestCase("no token", null)]
    [TestCase("bm8gbnVtYmVy.a.b", null)]
    [TestCase(null, null)]
    public void ReadsApplicationId(string? token, ulong? expected)
    {
        Assert.That(DiscordBotToken.GetApplicationId(token), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that the invite link names the application and asks for the commands.
    /// </summary>
    [Test]
    public void InviteUrlNamesTheApplication()
    {
        var url = DiscordBotToken.CreateInviteUrl(123);

        Assert.That(url, Does.StartWith("https://discord.com/oauth2/authorize?client_id=123&"));
        Assert.That(url, Does.Contain("scope=bot+applications.commands"));
    }
}
