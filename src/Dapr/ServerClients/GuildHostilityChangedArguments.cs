// <copyright file="GuildHostilityChangedArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for the notification that a hostility between two guilds was created or removed.
/// </summary>
/// <param name="GuildIdA">The identifier of the first guild.</param>
/// <param name="AllianceGuildIdsA">The identifiers of the alliance guilds of the first guild.</param>
/// <param name="GuildIdB">The identifier of the second guild.</param>
/// <param name="AllianceGuildIdsB">The identifiers of the alliance guilds of the second guild.</param>
/// <param name="Created">A value indicating whether the hostility was created or removed.</param>
public record GuildHostilityChangedArguments(uint GuildIdA, IReadOnlyList<uint> AllianceGuildIdsA, uint GuildIdB, IReadOnlyList<uint> AllianceGuildIdsB, bool Created);
