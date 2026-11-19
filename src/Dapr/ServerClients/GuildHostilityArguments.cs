// <copyright file="GuildHostilityArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for creating or removing a hostility between two guilds.
/// </summary>
/// <param name="GuildIdA">The identifier of the first guild.</param>
/// <param name="GuildIdB">The identifier of the second guild.</param>
/// <param name="Create">A value indicating whether the hostility should be created or removed.</param>
public record GuildHostilityArguments(uint GuildIdA, uint GuildIdB, bool Create);
