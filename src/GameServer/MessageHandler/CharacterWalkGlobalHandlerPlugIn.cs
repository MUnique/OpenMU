// <copyright file="CharacterWalkGlobalHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles walking requests containing absolute global ushort coordinates.
/// </summary>
[PlugIn]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
internal sealed class CharacterWalkGlobalHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => WalkRequestGlobal.Code;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        WalkRequestGlobal request = packet;
        var requiredDirectionBytes = (request.StepCount + 1) / 2;
        if (packet.Length < 8 || request.Directions.Length != packet.Length - 8 || request.Directions.Length < requiredDirectionBytes)
        {
            return;
        }

        var source = new Point(request.SourceX, request.SourceY);
        var directions = new Direction[request.StepCount];
        for (int i = 0; i < directions.Length; i++)
        {
            var value = request.Directions[i / 2];
            value = (byte)(i % 2 == 0 ? value >> 4 : value & 0x0F);
            directions[i] = value.ParseAsDirection();
        }

        var steps = new WalkingStep[directions.Length];
        var previous = source;
        for (int i = 0; i < directions.Length; i++)
        {
            var target = previous.CalculateTargetPoint(directions[i]);
            steps[i] = new WalkingStep { Direction = directions[i], From = previous, To = target };
            previous = target;
        }

        if (steps.Length == 0)
        {
            player.Rotation = request.TargetRotation.ParseAsDirection();
            return;
        }

        await player.WalkToAsync(previous, steps).ConfigureAwait(false);
    }
}
