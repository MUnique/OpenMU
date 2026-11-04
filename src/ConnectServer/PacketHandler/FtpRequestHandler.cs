// <copyright file="FtpRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ConnectServer;

/// <summary>
/// Handles the ftp related request. The client is sending its version, and the server answers
/// with the current version and the ftp address where the client can load a patch.
/// </summary>
internal class FtpRequestHandler : IPacketHandler<Client>
{
    private static readonly byte[] PatchOk = { 0xC1, 4, 2, 0 };

    private static readonly byte[] Xor3Keys = { 0xFC, 0xCF, 0xAB };

    private readonly IConnectServerSettings _connectServerSettings;
    private readonly ILogger<FtpRequestHandler> _logger;

    private byte[]? _patchPacket;

    /// <summary>
    /// Initializes a new instance of the <see cref="FtpRequestHandler" /> class.
    /// </summary>
    /// <param name="connectServerSettings">The settings.</param>
    /// <param name="logger">The logger.</param>
    public FtpRequestHandler(IConnectServerSettings connectServerSettings, ILogger<FtpRequestHandler> logger)
    {
        this._connectServerSettings = connectServerSettings;
        this._logger = logger;
    }

    /// <summary>
    /// The version compare result.
    /// </summary>
    private enum VersionCompareResult
    {
        VersionTooLow = -1,
        VersionMatch = 0,
        VersionHigher = 1,
    }

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Client client, Memory<byte> packet)
    {
        if (packet.Length < 6)
        {
            return;
        }

        void LogVersion(Span<byte> span)
        {
            this._logger.LogDebug($"Client {client.Address}:{client.Port} version: {span[3]}.{span[4]}.{span[5]}");
        }

        if (this._logger.IsEnabled(LogLevel.Debug))
        {
            LogVersion(packet.Span);
        }

        if (client.FtpRequestCount >= this._connectServerSettings.MaxFtpRequests)
        {
            if (this._logger.IsEnabled(LogLevel.Debug))
            {
                this._logger.LogDebug("Client {0}:{1} reached maxFtpRequests", client.Address, client.Port);
            }

            await client.Connection.DisconnectAsync().ConfigureAwait(false);
            return;
        }

        int WritePatchPacket()
        {
            if (this._patchPacket is { } cachedPacket)
            {
                var span = client.Connection.Output.GetSpan(cachedPacket.Length);
                cachedPacket.CopyTo(span);
            }
            else
            {
                var length = ClientNeedsPatchRef.Length;
                var span = client.Connection.Output.GetSpan(length)[..length];
                var packet = new ClientNeedsPatchRef(span);
                packet.PatchAddress = this._connectServerSettings.PatchAddress;
                var addressSize = Encoding.UTF8.GetByteCount(this._connectServerSettings.PatchAddress);

                Xor3Bytes(span.Slice(6), addressSize);
                packet.PatchVersion = this._connectServerSettings.CurrentPatchVersion[2];

                this._patchPacket = span.ToArray();
            }

            return this._patchPacket.Length;
        }

        int WriteOkayPacket()
        {
            var span = client.Connection.Output.GetSpan(PatchOk.Length)[..PatchOk.Length];
            PatchOk.CopyTo(span);
            return PatchOk.Length;
        }

        if (VersionCompare(this._connectServerSettings.CurrentPatchVersion, 0, packet.Span, 3, this._connectServerSettings.CurrentPatchVersion.Length) == VersionCompareResult.VersionTooLow)
        {
            await client.Connection.SendAsync(WritePatchPacket).ConfigureAwait(false);
        }
        else
        {
            await client.Connection.SendAsync(WriteOkayPacket).ConfigureAwait(false);
        }

        client.FtpRequestCount++;
    }

    /// <summary>
    /// Compares the actual version of the client with the expected version.
    /// </summary>
    /// <param name="expectedVersion">The expected version.</param>
    /// <param name="expectedIndex">The expected index.</param>
    /// <param name="actualVersion">The actual version.</param>
    /// <param name="actualIndex">The actual index.</param>
    /// <param name="count">The count.</param>
    /// <returns>The compare result.</returns>
    private static VersionCompareResult VersionCompare(byte[] expectedVersion, int expectedIndex, Span<byte> actualVersion, int actualIndex, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            if (expectedVersion[i + expectedIndex] > actualVersion[i + actualIndex])
            {
                return VersionCompareResult.VersionTooLow;
            }

            if (expectedVersion[i + expectedIndex] < actualVersion[i + actualIndex])
            {
                return VersionCompareResult.VersionHigher;
            }
        }

        return VersionCompareResult.VersionMatch;
    }

    private static void Xor3Bytes(Span<byte> data, int size)
    {
        for (int i = 0; i < size; i++)
        {
            data[i] ^= Xor3Keys[i % 3];
        }
    }
}