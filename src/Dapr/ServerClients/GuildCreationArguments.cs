// <copyright file="GuildCreationArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for a guild creation.
/// </summary>
/// <param name="Name">The guild name.</param>
/// <param name="MasterName">The guild master character name.</param>
/// <param name="MasterId">The guild master character identifier.</param>
/// <param name="Logo">The guild emblem data.</param>
/// <param name="ServerId">The server identifier.</param>
public record GuildCreationArguments(string Name, string MasterName, Guid MasterId, byte[] Logo, byte ServerId);