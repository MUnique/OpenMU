// <copyright file="BotChatLine.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// One line a server-side bot may say in a <see cref="BotChatSituation"/>.
/// </summary>
/// <remarks>
/// The text is a <see cref="LocalizedString"/>: every receiving player sees the translation of
/// their own language, falling back to the English text. Placeholders in curly braces
/// (e.g. <c>{event}</c>) are filled in when the line is said; which ones a situation offers is
/// documented on <see cref="BotChatSituation"/>.
/// </remarks>
public class BotChatLine
{
    /// <summary>
    /// Gets or sets the situation in which the line may be said.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLine_Situation_Name), Description = nameof(PlugInResources.BotChatLine_Situation_Description))]
    public BotChatSituation Situation { get; set; }

    /// <summary>
    /// Gets or sets the text of the line.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLine_Text_Name), Description = nameof(PlugInResources.BotChatLine_Text_Description))]
    public LocalizedString Text { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{this.Situation}: {this.Text.ValueInNeutralLanguage}";
}
