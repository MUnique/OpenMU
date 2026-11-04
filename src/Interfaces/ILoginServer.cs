// -----------------------------------------------------------------------
// <copyright file="ILoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface of the login server, which keeps track of all connected accounts.
/// </summary>
public interface ILoginServer
{
    /// <summary>Exclusively reserves an offline account for an administrative write.</summary>
    /// <param name="accountName">The canonical account login name.</param>
    /// <param name="reservationId">An unpredictable positive ownership token greater than 255.</param>
    /// <returns>Whether the five-minute reservation is held; an online or reserved account is refused.</returns>
    /// <remarks>Callers must bound their work and persistence cancellation well below the lease lifetime, then release ownership.</remarks>
    ValueTask<bool> TryReserveAccountAsync(string accountName, int reservationId);

    /// <summary>Releases only the reservation owned by this request.</summary>
    /// <param name="accountName">The canonical account name.</param>
    /// <param name="reservationId">The ownership token returned by the caller.</param>
    ValueTask ReleaseAccountReservationAsync(string accountName, int reservationId);

    /// <summary>
    /// Tries to login the account on the specified server.
    /// </summary>
    /// <param name="accountName">Name of the account.</param>
    /// <param name="serverId">The server identifier.</param>
    /// <returns>The success.</returns>
    Task<bool> TryLoginAsync(string accountName, byte serverId);

    /// <summary>
    /// Logs the account off from the specified server.
    /// </summary>
    /// <param name="accountName">Name of the account.</param>
    /// <param name="serverId">The server identifier.</param>
    ValueTask LogOffAsync(string accountName, byte serverId);

    /// <summary>
    /// Gets a snapshot of the currently logged in accounts and their corresponding server ids.
    /// </summary>
    /// <returns>A snapshot of the currently logged in accounts and their corresponding server ids.</returns>
    ValueTask<Dictionary<string, byte>> GetSnapshotAsync();
}