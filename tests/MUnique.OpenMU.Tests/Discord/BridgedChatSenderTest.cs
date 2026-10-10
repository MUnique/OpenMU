// <copyright file="BridgedChatSenderTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using MUnique.OpenMU.GameLogic.Discord;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;

/// <summary>
/// Tests for the <see cref="BridgedChatSender"/> and the guarantee it relies on.
/// </summary>
[TestFixture]
public class BridgedChatSenderTest
{
    /// <summary>
    /// Tests that a character can't get the prefix of bridged senders, even without a configured name expression,
    /// so no character can pass for a Discord user.
    /// </summary>
    /// <param name="name">The name.</param>
    [TestCase("@Bob")]
    [TestCase("Bo@b")]
    public void CharacterNameWithPrefixIsRefused(string name)
    {
        Assert.That(CreateCharacterAction.IsValidCharacterName(name, null), Is.False);
        Assert.That(CreateCharacterAction.IsValidCharacterName(name, ".*"), Is.False);
    }

    /// <summary>
    /// Tests that other names still follow the configured expression.
    /// </summary>
    [Test]
    public void OtherNamesFollowTheExpression()
    {
        Assert.That(CreateCharacterAction.IsValidCharacterName("Bob", null), Is.True);
        Assert.That(CreateCharacterAction.IsValidCharacterName("Bob", "^[a-z]+$"), Is.False);
    }

    /// <summary>
    /// Tests that a bridged sender is recognized and named without its prefix.
    /// </summary>
    [Test]
    public void BridgedSenderIsRecognized()
    {
        Assert.That(BridgedChatSender.IsBridged("@Hero"), Is.True);
        Assert.That(BridgedChatSender.IsBridged("Hero"), Is.False);
        Assert.That(BridgedChatSender.GetName("@Hero"), Is.EqualTo("Hero"));
    }
}
