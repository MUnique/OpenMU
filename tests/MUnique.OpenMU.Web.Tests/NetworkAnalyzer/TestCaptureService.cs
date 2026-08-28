// <copyright file="TestCaptureService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using MUnique.OpenMU.Network.Analyzer;

/// <summary>
/// A <see cref="IPacketCaptureService"/> for the tests of the network analyzer page.
/// </summary>
public sealed class TestCaptureService : IPacketCaptureService
{
    private readonly Dictionary<Guid, LiveCapturedConnection> _captures = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TestCaptureService"/> class.
    /// </summary>
    /// <param name="connections">The connections which should be provided.</param>
    public TestCaptureService(params ICapturedConnectionInfo[] connections)
    {
        this.Connections = connections;
    }

    /// <summary>
    /// Gets the provided connections.
    /// </summary>
    public IReadOnlyList<ICapturedConnectionInfo> Connections { get; }

    /// <summary>
    /// Gets the identifiers of the connections whose capture has been stopped.
    /// </summary>
    public IList<Guid> StoppedCaptures { get; } = new List<Guid>();

    /// <summary>
    /// Gets the number of requests for the connections.
    /// </summary>
    public int RequestedConnectionsCount { get; private set; }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<ICapturedConnectionInfo>> GetConnectionsAsync()
    {
        this.RequestedConnectionsCount++;
        return ValueTask.FromResult(this.Connections);
    }

    /// <inheritdoc />
    public ValueTask<ICapturedConnectionInfo?> FindConnectionAsync(Guid connectionId)
    {
        return ValueTask.FromResult(this.Connections.FirstOrDefault(connection => connection.Id == connectionId));
    }

    /// <inheritdoc />
    public ValueTask<ICapturedConnectionInfo?> FindConnectionAsync(int serverId, string accountOrCharacterName)
    {
        return ValueTask.FromResult(this.Connections.FirstOrDefault(connection => connection.ServerId == serverId
            && (connection.CharacterName == accountOrCharacterName || connection.AccountName == accountOrCharacterName)));
    }

    /// <inheritdoc />
    public async ValueTask<ILiveCapturedConnection?> StartCaptureAsync(Guid connectionId)
    {
        if (await this.FindConnectionAsync(connectionId).ConfigureAwait(false) is not { } connectionInfo)
        {
            return null;
        }

        if (!this._captures.TryGetValue(connectionId, out var capture))
        {
            capture = new LiveCapturedConnection(connectionInfo);
            this._captures.Add(connectionId, capture);
            connectionInfo.AddCaptureSink(capture);
        }

        return capture;
    }

    /// <inheritdoc />
    public void StopCapture(Guid connectionId)
    {
        this.StoppedCaptures.Add(connectionId);
        if (this._captures.Remove(connectionId, out var capture))
        {
            this.Connections.FirstOrDefault(connection => connection.Id == connectionId)?.RemoveCaptureSink(capture);
        }
    }

    /// <inheritdoc />
    public ILiveCapturedConnection? GetRunningCapture(Guid connectionId)
    {
        return this._captures.GetValueOrDefault(connectionId);
    }
}
