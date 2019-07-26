// <copyright file="CharacterMoveHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for move packets.
    /// </summary>
    [PlugIn("Character move handler (0.75)", "Packet handler for move packets, version 0.75.")]
    [Guid("C1F633E1-3A15-45C6-9235-7B78520D3CC5")]
    [MinimumClient(0, 75, ClientLanguage.Invariant)]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    internal class CharacterMoveHandlerPlugIn075 : CharacterMoveBaseHandlerPlugIn
    {
        /// <inheritdoc/>
        public override byte Key => 0x11;

        /// <inheritdoc/>
        public override MoveType MoveType => MoveType.Instant;
    }
}