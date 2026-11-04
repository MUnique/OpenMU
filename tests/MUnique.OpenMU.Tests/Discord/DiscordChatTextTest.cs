// <copyright file="DiscordChatTextTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using MUnique.OpenMU.Discord.ChatBridge;

/// <summary>
/// Tests for the <see cref="DiscordChatText"/>.
/// </summary>
[TestFixture]
public class DiscordChatTextTest
{
    /// <summary>
    /// Tests the conversion of Discord messages for the game.
    /// </summary>
    /// <param name="text">The Discord message.</param>
    /// <param name="expected">The expected message for the game.</param>
    [TestCase("Hello", "Hello")]
    [TestCase("**bold** _italic_ ~~strike~~ `code` ||spoiler||", "bold italic strike code spoiler")]
    [TestCase("Hi <@123456> and <@&42> in <#77>", "Hi and in")]
    [TestCase("@everyone look", "look")]
    [TestCase("Nice <:pepe:123456789> <a:dance:42>", "Nice :pepe: :dance:")]
    [TestCase("Line 1\nLine 2\r\n\tLine 3", "Line 1 Line 2 Line 3")]
    [TestCase("  ", "")]
    public void MessageIsConvertedForTheGame(string text, string expected)
    {
        Assert.That(DiscordChatText.ToGame(text, 100), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that long messages are cut.
    /// </summary>
    [Test]
    public void LongMessageIsCut()
    {
        Assert.That(DiscordChatText.ToGame("Hello wonderful world", 15), Is.EqualTo("Hello wonderful"));
    }

    /// <summary>
    /// Tests that the sender in the game has a prefix, and fits into the name field of the game.
    /// </summary>
    [Test]
    public void SenderHasPrefixAndFits()
    {
        Assert.That(DiscordChatText.ToGameSender("Hero"), Is.EqualTo("@Hero"));
        Assert.That(DiscordChatText.ToGameSender("LongName10"), Is.EqualTo("@LongName1"));
    }

    /// <summary>
    /// Tests that the markdown in messages of the game is escaped for Discord.
    /// </summary>
    [Test]
    public void GameMessageIsEscapedForDiscord()
    {
        Assert.That(DiscordChatText.ToDiscord("Hero_1", "*wow*"), Is.EqualTo(@"**Hero\_1**: \*wow\*"));
    }
}
