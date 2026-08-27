// <copyright file="IPacketCaptureService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

/// <summary>
/// Service which provides the connections of all servers which run in this process, and which
/// captures their traffic on request.
/// </summary>
public interface IPacketCaptureService
{
    /// <summary>
    /// Gets the connections of all servers which can provide them.
    /// </summary>
    /// <returns>The connections of all servers which can provide them.</returns>
    ValueTask<IReadOnlyList<ICapturedConnectionInfo>> GetConnectionsAsync();

    /// <summary>
    /// Gets the connection with the specified identifier.
    /// </summary>
    /// <param name="connectionId">The identifier of the connection.</param>
    /// <returns>The connection, if it's still connected; Otherwise, <see langword="null"/>.</returns>
    ValueTask<ICapturedConnectionInfo?> FindConnectionAsync(Guid connectionId);

    /// <summary>
    /// Gets the connection of the specified account or character name.
    /// </summary>
    /// <param name="serverId">The identifier of the server.</param>
    /// <param name="accountOrCharacterName">The name of the account or character.</param>
    /// <returns>The connection, if one was found; Otherwise, <see langword="null"/>.</returns>
    ValueTask<ICapturedConnectionInfo?> FindConnectionAsync(int serverId, string accountOrCharacterName);

    /// <summary>
    /// Starts to capture the traffic of the specified connection, or returns the already
    /// running capture of it.
    /// </summary>
    /// <param name="connectionId">The identifier of the connection.</param>
    /// <returns>The capture of the connection, if it's still connected; Otherwise, <see langword="null"/>.</returns>
    /// <remarks>
    /// Each call has to be followed by a <see cref="StopCapture"/> when the caller isn't
    /// interested anymore. The capture stops when the last interested caller is gone.
    /// </remarks>
    ValueTask<ILiveCapturedConnection?> StartCaptureAsync(Guid connectionId);

    /// <summary>
    /// Stops the capture of the specified connection, when no other caller is interested in
    /// it anymore.
    /// </summary>
    /// <param name="connectionId">The identifier of the connection.</param>
    void StopCapture(Guid connectionId);

    /// <summary>
    /// Gets the currently running capture of the specified connection.
    /// </summary>
    /// <param name="connectionId">The identifier of the connection.</param>
    /// <returns>The running capture, if there is one; Otherwise, <see langword="null"/>.</returns>
    ILiveCapturedConnection? GetRunningCapture(Guid connectionId);
}
