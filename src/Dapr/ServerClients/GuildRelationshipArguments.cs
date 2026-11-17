// <copyright file="GuildRelationshipArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for determining the relationship between two guilds.
/// </summary>
/// <param name="GuildIdA">The first guild identifier.</param>
/// <param name="GuildIdB">The second guild identifier.</param>
public record GuildRelationshipArguments(uint GuildIdA, uint GuildIdB);
