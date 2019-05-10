// <copyright file="CharacterMoveHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for move packets.
    /// </summary>
    [PlugIn("Character move handler (ENG)", "Packet handler for walk packets.")]
    [Guid("d3b04177-131f-4bf5-a228-1f10d22d54f2")]
    [Client(0, 0, ClientLanguage.English)]
    internal class CharacterMoveHandlerPlugIn : CharacterMoveBaseHandlerPlugIn
    {
        /// <inheritdoc/>
        public override byte Key => (int)PacketType.Teleport;
    }
}