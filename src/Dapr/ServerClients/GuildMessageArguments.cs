// <copyright file="GuildMessageArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for a guild chat message.
/// </summary>
/// <param name="GuildId">The guild identifier.</param>
/// <param name="Sender">The sender character name.</param>
/// <param name="Message">The chat message.</param>
public record GuildMessageArguments(uint GuildId, string Sender, string Message);