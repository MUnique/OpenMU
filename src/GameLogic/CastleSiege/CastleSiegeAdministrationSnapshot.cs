// <copyright file="CastleSiegeAdministrationSnapshot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// An immutable snapshot of Castle Siege state for administrative display.
/// </summary>
/// <param name="State">The current Castle Siege state.</param>
/// <param name="StateStartTimeUtc">The UTC time at which the current state started.</param>
/// <param name="StateEndTimeUtc">The UTC time at which the current state ends.</param>
/// <param name="IsOccupied">Whether a guild owns the castle.</param>
/// <param name="OwnerGuildId">The persistent identifier of the owning guild.</param>
/// <param name="OwnerGuildName">The name of the owning guild, when it can be resolved.</param>
/// <param name="ChaosTax">The Chaos Machine tax percentage.</param>
/// <param name="StoreTax">The personal-store tax percentage.</param>
/// <param name="HuntTax">The Land of Trials entrance fee.</param>
/// <param name="TributeMoney">The accumulated tribute.</param>
/// <param name="Registrations">The registered guilds.</param>
/// <param name="Npcs">The configured Castle Siege NPCs.</param>
public sealed record CastleSiegeAdministrationSnapshot(
    CastleSiegeState State,
    DateTime StateStartTimeUtc,
    DateTime StateEndTimeUtc,
    bool IsOccupied,
    Guid? OwnerGuildId,
    string? OwnerGuildName,
    byte ChaosTax,
    byte StoreTax,
    int HuntTax,
    long TributeMoney,
    IReadOnlyList<CastleSiegeRegistrationSnapshot> Registrations,
    IReadOnlyList<CastleSiegeNpcAdministrationSnapshot> Npcs);
