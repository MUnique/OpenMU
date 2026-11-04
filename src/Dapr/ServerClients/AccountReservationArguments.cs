// <copyright file="AccountReservationArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>Identifies an exclusive offline account write reservation.</summary>
/// <param name="AccountName">The canonical account name.</param>
/// <param name="ReservationId">The unpredictable reservation owner identifier.</param>
public sealed record AccountReservationArguments(string AccountName, int ReservationId);
