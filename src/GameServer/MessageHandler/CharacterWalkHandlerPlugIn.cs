// <copyright file="CharacterWalkHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for walk packets.
    /// </summary>
    [PlugIn("Character walk handler", "Packet handler for walk packets.")]
    [Guid("19056DEB-4321-4D25-8615-EE49A453DF03")]
    [MinimumClient(0, 97, ClientLanguage.Invariant)]
    internal class CharacterWalkHandlerPlugIn : CharacterMoveBaseHandlerPlugIn
    {
        /// <inheritdoc/>
        public override byte Key => WalkRequest.Code;

        /// <inheritdoc/>
        public override MoveType MoveType => MoveType.Walk;
    }
}