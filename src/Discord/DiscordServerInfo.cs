// <copyright file="DiscordServerInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

/// <summary>
/// A Discord server which the bot is in.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The name.</param>
public sealed record DiscordServerInfo(ulong Id, string Name);
