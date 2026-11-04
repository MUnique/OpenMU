// <copyright file="BotConversation.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// A short exchange of two server-side bots, like the chatter of the townsfolk in a role-playing
/// game: one bot says the <see cref="Opener"/>, the other one answers with the <see cref="Reply"/>,
/// and the first one may have the last word with a <see cref="Riposte"/>.
/// </summary>
/// <remarks>
/// Both texts are <see cref="LocalizedString"/>s; every listening player sees the translation of
/// their own language. Placeholders: <c>{map}</c>, <c>{location}</c>, <c>{level}</c> and
/// <c>{class}</c> of the speaking bot, and <c>{partner}</c> - the name of the other bot.
/// </remarks>
public class BotConversation
{
    /// <summary>
    /// Gets or sets what the first bot says.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotConversation_Opener_Name), Description = nameof(PlugInResources.BotConversation_Opener_Description))]
    public LocalizedString Opener { get; set; }

    /// <summary>
    /// Gets or sets what the second bot answers.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotConversation_Reply_Name), Description = nameof(PlugInResources.BotConversation_Reply_Description))]
    public LocalizedString Reply { get; set; }

    /// <summary>
    /// Gets or sets what the first bot says back to the reply, a few seconds later; empty for a
    /// conversation of two lines.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotConversation_Riposte_Name), Description = nameof(PlugInResources.BotConversation_Riposte_Description))]
    public LocalizedString Riposte { get; set; }

    /// <summary>
    /// Gets or sets where the two bots may have this conversation.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotConversation_Where_Name), Description = nameof(PlugInResources.BotConversation_Where_Description))]
    public BotConversationPlace Where { get; set; }

    /// <summary>
    /// Gets or sets when the bots may have this conversation, by the local time of the server.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatLine_When_Name), Description = nameof(PlugInResources.BotChatLine_When_Description))]
    public BotChatTimeOfDay When { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{this.Opener.ValueInNeutralLanguage} - {this.Reply.ValueInNeutralLanguage}";
}
