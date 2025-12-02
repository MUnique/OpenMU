// <copyright file="AnimationHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for animation packets.
/// </summary>
[PlugIn("AnimationHandlerPlugIn", "Handler for animation packets.")]
[Guid("5cf7fa95-5ca2-4e14-bb08-4b64250a8ee8")]
internal class AnimationHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => AnimationRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < 5)
        {
            return;
        }

        AnimationRequest request = packet;

        var rotation = request.Rotation.ParseAsDirection();
        var animation = request.AnimationNumber;
        player.Rotation = rotation;

        player.Pose = animation switch
        {
            0x80 => CharacterPose.Sitting,
            0x81 => CharacterPose.Leaning,
            0x82 => CharacterPose.Hanging,
            _ => default,
        };

        await player.ForEachWorldObserverAsync<IShowAnimationPlugIn>(p => p.ShowAnimationAsync(player, animation, null, rotation), false).ConfigureAwait(false);
    }
}