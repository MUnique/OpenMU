// <copyright file="BotChatLanguageHint.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// The words and letters by which a bot recognizes the language of a player's message, so it can
/// answer in that language - a Polish whisper gets a Polish answer, even when the player's account
/// language is English (see <see cref="BotChatConfiguration.MessageLanguages"/>).
/// </summary>
public class BotChatLanguageHint
{
    /// <summary>
    /// Gets or sets the language code, e.g. <c>pl</c>.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLanguageHint_Language_Name), Description = nameof(PlugInResources.BotChatLanguageHint_Language_Description))]
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the comma-separated typical words of the language, e.g. <c>jak, gdzie, dzięki</c>.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLanguageHint_Words_Name), Description = nameof(PlugInResources.BotChatLanguageHint_Words_Description))]
    public string Words { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the letters only this language uses, e.g. <c>ąćęłńóśźż</c>; one of them is as good
    /// as a typical word.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLanguageHint_Letters_Name), Description = nameof(PlugInResources.BotChatLanguageHint_Letters_Description))]
    public string Letters { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString() => this.Language;
}
