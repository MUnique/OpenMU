// <copyright file="AllianceGuildEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// An entry of the alliance guild list.
/// </summary>
/// <param name="Id">The unique identifier of the guild.</param>
/// <param name="GuildName">The name of the guild.</param>
/// <param name="MemberCount">The number of members in the guild.</param>
/// <param name="Logo">The logo of the guild.</param>
public record AllianceGuildEntry(uint Id, string GuildName, int MemberCount, Memory<byte> Logo);