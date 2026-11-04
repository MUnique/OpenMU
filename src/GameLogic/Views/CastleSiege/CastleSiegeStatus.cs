// <copyright file="CastleSiegeStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The Castle Siege status shown by the guardsman window.
/// </summary>
/// <param name="State">The current Castle Siege state.</param>
/// <param name="StateStart">The start of the current state, in server time.</param>
/// <param name="StateEnd">The end of the current state, in server time.</param>
/// <param name="SiegeStart">The start of the current or next battle, in server time.</param>
/// <param name="OwnerGuildName">The name of the castle owner guild, or an empty string.</param>
/// <param name="OwnerGuildMasterName">The name of the castle owner guild master, or an empty string.</param>
/// <param name="RemainingTime">The time until the current state ends.</param>
public sealed record CastleSiegeStatus(
    CastleSiegeState State,
    DateTime StateStart,
    DateTime StateEnd,
    DateTime SiegeStart,
    string OwnerGuildName,
    string OwnerGuildMasterName,
    TimeSpan RemainingTime);
