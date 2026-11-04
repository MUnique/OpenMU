// <copyright file="CastleSiegeRegistrationSnapshot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// An immutable snapshot of a guild registration.
/// </summary>
/// <param name="GuildId">The persistent guild identifier.</param>
/// <param name="GuildName">The guild name.</param>
/// <param name="Marks">The submitted Mark of Lord count.</param>
/// <param name="RegistrationOrder">The registration tie-break order.</param>
/// <param name="IsGuildDeleted">Whether the persistent guild can no longer be found.</param>
public sealed record CastleSiegeRegistrationSnapshot(
    Guid GuildId,
    string GuildName,
    int Marks,
    int RegistrationOrder,
    bool IsGuildDeleted = false);
