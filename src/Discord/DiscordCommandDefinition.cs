// <copyright file="DiscordCommandDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// The definition of a slash command of the Discord bot.
/// </summary>
/// <param name="Name">The name, e.g. <c>who</c>.</param>
/// <param name="DescriptionKey">The resource key of the description.</param>
/// <param name="OptionName">The name of the text option, if the command has one.</param>
/// <param name="OptionDescriptionKey">The resource key of the description of the option.</param>
public sealed record DiscordCommandDefinition(string Name, string DescriptionKey, string? OptionName = null, string? OptionDescriptionKey = null);
