// <copyright file="ChangeLanguageChatCommandPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Tests for the <see cref="ChangeLanguageChatCommandPlugIn"/>, which accepts more than two-letter ISO codes,
/// as described by <see cref="ChatCommandValueReference.Language"/>.
/// </summary>
[TestFixture]
public class ChangeLanguageChatCommandPlugInTest
{
    /// <summary>
    /// Tests that the language can be requested by its two-letter ISO code, its three-letter ISO code
    /// or its culture name, which is compared case-insensitively.
    /// </summary>
    /// <param name="requestedLanguage">The requested language.</param>
    [TestCase("de")]
    [TestCase("deu")]
    [TestCase("DE")]
    public async ValueTask LanguageIsChangedAsync(string requestedLanguage)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Culture = CultureInfo.InvariantCulture;

        await new ChangeLanguageChatCommandPlugIn().HandleCommandAsync(player, "/language " + requestedLanguage).ConfigureAwait(false);

        Assert.That(player.Culture.Name, Is.EqualTo("de"));
    }

    /// <summary>
    /// Tests that the language describes the accepted values as a language or culture identifier,
    /// and not only as a two-letter ISO code.
    /// </summary>
    [Test]
    public void ParameterRefersToALanguage()
    {
        var info = ChatCommandTypeExtensions.TryCreateChatCommandInfo(typeof(ChangeLanguageChatCommandPlugIn), CultureInfo.InvariantCulture);

        Assert.That(info, Is.Not.Null);
        Assert.That(info!.Parameters.Single().ValueReference, Is.EqualTo(ChatCommandValueReference.Language));
    }
}
