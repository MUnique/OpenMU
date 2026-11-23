// <copyright file="ChangeLanguageChatCommandPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Tests for the <see cref="ChangeLanguageChatCommandPlugIn"/>.
/// </summary>
[TestFixture]
public class ChangeLanguageChatCommandPlugInTest
{
    /// <summary>
    /// Verifies that the chosen language is stored in the account, so it is still used after the next login.
    /// </summary>
    [Test]
    public async ValueTask ChosenLanguageIsStoredInTheAccountAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Account!.LanguageIsoCode = "en";
        var plugin = new ChangeLanguageChatCommandPlugIn();

        await plugin.HandleCommandAsync(player, "/language de").ConfigureAwait(false);

        Assert.That(player.Culture.TwoLetterISOLanguageName, Is.EqualTo("de"));
        Assert.That(player.Account.LanguageIsoCode, Is.EqualTo("de"));
    }

    /// <summary>
    /// Verifies that an unknown language changes neither the culture nor the account.
    /// </summary>
    [Test]
    public async ValueTask UnknownLanguageKeepsTheAccountAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Account!.LanguageIsoCode = "en";
        var cultureBefore = player.Culture;
        var plugin = new ChangeLanguageChatCommandPlugIn();

        await plugin.HandleCommandAsync(player, "/language xx").ConfigureAwait(false);

        Assert.That(player.Culture, Is.EqualTo(cultureBefore));
        Assert.That(player.Account.LanguageIsoCode, Is.EqualTo("en"));
    }
}
