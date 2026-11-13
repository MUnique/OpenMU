// <copyright file="BotChatReply.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// One answer of a server-side bot to a player who whispered to it or mentioned its name in the
/// normal chat next to it (see <see cref="BotChatConfiguration.WhisperReplies"/>).
/// </summary>
/// <remarks>
/// The bot picks among the replies whose <see cref="Keywords"/> match the player's message best;
/// when none matches, it uses a reply without keywords.
/// </remarks>
public class BotChatReply
{
    /// <summary>
    /// Gets or sets the comma-separated words or phrases of a player's message this reply answers,
    /// e.g. <c>how are you, how r u</c>. A reply without keywords is the fallback when no keyword
    /// matches.
    /// </summary>
    /// <remarks>
    /// The keywords are compared as whole words, ignoring case and punctuation, so <c>hi</c> doesn't
    /// match "this". They may be written in several languages at once - the message's language is
    /// not known.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatReply_Keywords_Name), Description = nameof(PlugInResources.BotChatReply_Keywords_Description))]
    public string Keywords { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text of the reply.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLine_Text_Name), Description = nameof(PlugInResources.BotChatLine_Text_Description))]
    public LocalizedString Text { get; set; }

    /// <summary>
    /// Gets or sets the gesture the bot makes with the reply, seen by the players around it.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLine_Gesture_Name), Description = nameof(PlugInResources.BotChatLine_Gesture_Description))]
    public BotChatGesture Gesture { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{this.Keywords}: {this.Text.ValueInNeutralLanguage}";
}
