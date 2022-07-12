// <copyright file="CharacterMoveBaseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Abstract packet handler for move packets.
/// </summary>
internal abstract class CharacterMoveBaseHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public abstract byte Key { get; }

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < InstantMoveRequest.Length)
        {
            return;
        }

        // We don't move the player anymore by his request. This was usually requested after a player performed a skill.
        // However, it adds way for cheaters to move through the map.
        // So, we just allow it for developers when the debugger is attached.
        // When handling a skill which moves to the target, we'll handle the move on server-side, instead.
        InstantMoveRequest moveRequest = packet;
        await player.MoveAsync(new Point(moveRequest.TargetX, moveRequest.TargetY));
    }
}