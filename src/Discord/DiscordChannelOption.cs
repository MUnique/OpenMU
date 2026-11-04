// <copyright file="DiscordChannelOption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// A text channel of a Discord server, which can be chosen for notifications.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The name.</param>
/// <param name="CategoryName">The name of the category of the channel, or <c>null</c>.</param>
public sealed record DiscordChannelOption(ulong Id, string Name, string? CategoryName);
