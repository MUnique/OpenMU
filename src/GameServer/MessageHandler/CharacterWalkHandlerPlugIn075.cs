// <copyright file="CharacterWalkHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for walk packets.
    /// </summary>
    [PlugIn("Character walk handler", "Packet handler for walk packets.")]
    [Guid("9FD41038-39D9-4D3D-A1DD-A87DB6388248")]
    [MinimumClient(0, 75, ClientLanguage.Invariant)]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    internal class CharacterWalkHandlerPlugIn075 : CharacterMoveBaseHandlerPlugIn
    {
        /// <inheritdoc/>
        public override byte Key => 0x10;

        /// <inheritdoc/>
        public override MoveType MoveType => MoveType.Walk;
    }
}