// <copyright file="FtpRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler
{
    using System;
    using System.Text;
    using log4net;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handles the ftp related request. The client is sending its version, and the server answers
    /// with the current version and the ftp address where the client can load a patch.
    /// </summary>
    internal class FtpRequestHandler : IPacketHandler<Client>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FtpRequestHandler));

        private static readonly byte[] PatchOk = { 0xC1, 4, 2, 0 };

        private static readonly byte[] Xor3Keys = { 0xFC, 0xCF, 0xAB };

        private readonly IConnectServerSettings connectServerSettings;

        private byte[] patchPacket;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpRequestHandler"/> class.
        /// </summary>
        /// <param name="connectServerSettings">The settings.</param>
        public FtpRequestHandler(IConnectServerSettings connectServerSettings)
        {
            this.connectServerSettings = connectServerSettings;
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
        public void HandlePacket(Client client, Span<byte> packet)
        {
            if (packet.Length < 6)
            {
                return;
            }

            Log.DebugFormat("Client {0}:{1} version: {2}.{3}.{4}", client.Address, client.Port, packet[3], packet[4], packet[5]);
            if (client.FtpRequestCount >= this.connectServerSettings.MaxFtpRequests)
            {
                Log.DebugFormat("Client {0}:{1} reached maxFtpRequests", client.Address, client.Port);
                client.Connection.Disconnect();
                return;
            }

            byte[] response;
            if (VersionCompare(this.connectServerSettings.CurrentPatchVersion, 0, packet, 3, this.connectServerSettings.CurrentPatchVersion.Length) == VersionCompareResult.VersionTooLow)
            {
                response = this.GetPatchPacket();
            }
            else
            {
                response = PatchOk;
            }

            using (var writer = client.Connection.StartSafeWrite(response[0], response.Length))
            {
                response.CopyTo(writer.Span);
                writer.Commit();
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

        private static void Xor3Bytes(byte[] data, int size)
        {
            for (int i = 0; i < size; i++)
            {
                data[i] ^= Xor3Keys[i % 3];
            }
        }

        private byte[] GetPatchPacket()
        {
            if (this.patchPacket != null)
            {
                return this.patchPacket;
            }

            var packet = new byte[0x8A];
            packet[0] = 0xC1;
            packet[1] = 0x8A;
            packet[2] = 0x05;
            packet[3] = 0x01;
            packet[4] = this.connectServerSettings.CurrentPatchVersion[2];

            // adress starting at 6
            var adressBytes = Encoding.ASCII.GetBytes(this.connectServerSettings.PatchAddress);
            Xor3Bytes(adressBytes, adressBytes.Length);
            Buffer.BlockCopy(adressBytes, 0, packet, 6, adressBytes.Length);
            this.patchPacket = packet;
            return packet;
        }
    }
}
