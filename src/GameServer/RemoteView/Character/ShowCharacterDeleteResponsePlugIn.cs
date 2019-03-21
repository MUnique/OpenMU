// <copyright file="ShowCharacterDeleteResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Character;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowCharacterDeleteResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowCharacterDeleteResponsePlugIn", "The default implementation of the IShowCharacterDeleteResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("303af9ef-4f7d-4e07-9976-a0241311e50d")]
    public class ShowCharacterDeleteResponsePlugIn : IShowCharacterDeleteResponsePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowCharacterDeleteResponsePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowCharacterDeleteResponsePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowCharacterDeleteResponse(CharacterDeleteResult result)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 0x05))
            {
                var packet = writer.Span;
                packet[2] = 0xF3;
                packet[3] = 0x02;
                packet[4] = (byte)result;
                writer.Commit();
            }
        }
    }
}