// <copyright file="BotChatLine.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// One line a server-side bot may say. The list a line is in (e.g.
/// <see cref="BotChatConfiguration.GreetingLines"/>) decides when it is said.
/// </summary>
/// <remarks>
/// The text is a <see cref="LocalizedString"/>: every receiving player sees the translation of
/// their own language, falling back to the English text. Placeholders in curly braces
/// (e.g. <c>{event}</c>) are filled in when the line is said; which ones a list offers is
/// documented on the list.
/// </remarks>
public class BotChatLine
{
    /// <summary>
    /// Gets or sets the text of the line.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLine_Text_Name), Description = nameof(PlugInResources.BotChatLine_Text_Description))]
    public LocalizedString Text { get; set; }

    /// <summary>
    /// Gets or sets when the line may be said, by the local time of the server.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLine_When_Name), Description = nameof(PlugInResources.BotChatLine_When_Description))]
    public BotChatTimeOfDay When { get; set; }

    /// <summary>
    /// Gets or sets the gesture the bot makes with the line, seen by the players around it.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLine_Gesture_Name), Description = nameof(PlugInResources.BotChatLine_Gesture_Description))]
    public BotChatGesture Gesture { get; set; }

    /// <inheritdoc />
    public override string ToString() => this.Text.ValueInNeutralLanguage;
}
