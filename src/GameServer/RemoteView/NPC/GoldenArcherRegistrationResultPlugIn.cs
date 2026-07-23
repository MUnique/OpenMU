// <copyright file="GoldenArcherRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IGoldenArcherRegistrationResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Guid("4A1B2C3D-4E5F-6A7B-8C9D-0E1F2A3B4C5D")]
public class GoldenArcherRegistrationResultPlugIn : IGoldenArcherRegistrationResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoldenArcherRegistrationResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public GoldenArcherRegistrationResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask RegistrationResultAsync()
    {
        var connection = this._player.Connection;
        if (connection != null)
        {
            // Calculate the number of renas in the inventory
            int invRenas = this._player.Inventory?.Items.Count(i => 
                i.Definition?.Group == 14 && (i.Definition?.Number == 21 || i.Definition?.Number == 30 || i.Definition?.Number == 31)) ?? 0;
            
            int registered = this._player.SelectedCharacter?.RegisteredRenas ?? 0;

            int WritePacket()
            {
                var span = connection.Output.GetSpan(10)[..10];
                span[0] = 0xC1; // Packet type (C1)
                span[1] = 0x0A; // Length (10 bytes)
                span[2] = 0x94; // Packet code for Event Chip/Rena Result
                span[3] = 0x00; // Result (0 = Success)
                
                // Registered Count (int / 32-bit) at offset 4
                span[4] = (byte)(registered & 0xFF);
                span[5] = (byte)((registered >> 8) & 0xFF);
                span[6] = (byte)((registered >> 16) & 0xFF);
                span[7] = (byte)((registered >> 24) & 0xFF);
                
                // Remaining Inventory Count (short / 16-bit) at offset 8
                span[8] = (byte)(invRenas & 0xFF);
                span[9] = (byte)((invRenas >> 8) & 0xFF);

                return 10;
            }

            await connection.SendAsync(WritePacket).ConfigureAwait(false);
        }
    }
}
