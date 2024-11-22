// <copyright file="ObjectMovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Buffers;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IObjectMovedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ObjectMovedPlugIn), "The default implementation of the IObjectMovedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("29ee689f-636c-47e7-a930-b60ce8e8993c")]
[MinimumClient(1, 0, ClientLanguage.Invariant)]
public class ObjectMovedPlugIn : IObjectMovedPlugIn
{
    private const short TeleportTargetNumber = 0x0F;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectMovedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ObjectMovedPlugIn(RemotePlayer player) => this._player = player;

    /// <summary>
    /// Gets or sets a value indicating whether the directions provided by <see cref="ISupportWalk.GetDirectionsAsync"/> should be send when an object moved.
    /// This is usually not required, because the game client calculates a proper path anyway and doesn't use the suggested path.
    /// </summary>
    public bool SendWalkDirections { get; set; } = true;

    /// <inheritdoc/>
    public async ValueTask ObjectMovedAsync(ILocateable obj, MoveType type)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        var objectId = obj.GetId(this._player);
        switch (type)
        {
            case MoveType.Instant:
                await connection.SendObjectMovedAsync(this.GetInstantMoveCode(), objectId, obj.Position.X, obj.Position.Y).ConfigureAwait(false);
                break;

            case MoveType.Teleport when obj is Player movedPlayer && movedPlayer != this._player:
                await this._player.InvokeViewPlugInAsync<INewPlayersInScopePlugIn>(p => p.NewPlayersInScopeAsync(movedPlayer.GetAsEnumerable(), false)).ConfigureAwait(false);
                await this._player.InvokeViewPlugInAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(movedPlayer, movedPlayer, TeleportTargetNumber, true)).ConfigureAwait(false);
                break;

            case MoveType.Teleport when obj is NonPlayerCharacter movedNpc:
                await this._player.InvokeViewPlugInAsync<INewNpcsInScopePlugIn>(p => p.NewNpcsInScopeAsync(movedNpc.GetAsEnumerable(), false)).ConfigureAwait(false);
                if (movedNpc is Monster monster)
                {
                    await this._player.InvokeViewPlugInAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(monster, monster, TeleportTargetNumber, true)).ConfigureAwait(false);
                }

                break;

            case MoveType.Teleport:
                // no other types available
                break;

            case MoveType.Walk:
                await this.ObjectWalkedAsync(obj).ConfigureAwait(false);
                break;
        }
    }

    /// <summary>
    /// Sends the network message.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="objectId">The object identifier.</param>
    /// <param name="sourcePoint">The origin point.</param>
    /// <param name="targetPoint">The target point.</param>
    /// <param name="steps">The steps.</param>
    /// <param name="rotation">The rotation.</param>
    /// <param name="stepsLength">Length of the steps.</param>
    protected virtual async ValueTask SendWalkAsync(IConnection connection, ushort objectId, Point sourcePoint, Point targetPoint, Memory<Direction> steps, Direction rotation, int stepsLength)
    {
        int Write()
        {
            var stepsSize = steps.Length == 0 ? 1 : (steps.Length / 2) + 2;
            var size = ObjectWalkedRef.GetRequiredSize(stepsSize);
            var span = connection.Output.GetSpan(size)[..size];

            var walkPacket = new ObjectWalkedRef(span)
            {
                HeaderCode = this.GetWalkCode(),
                ObjectId = objectId,
                TargetX = targetPoint.X,
                TargetY = targetPoint.Y,
                TargetRotation = rotation.ToPacketByte(),
                StepCount = (byte)stepsLength,
            };

            this.SetStepData(walkPacket, steps.Span, stepsSize);
            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private async ValueTask ObjectWalkedAsync(ILocateable obj)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var objectId = obj.GetId(this._player);

        using var rentArray = this.SendWalkDirections ? MemoryPool<Direction>.Shared.Rent(16) : null;
        var steps = rentArray?.Memory.Slice(0, 16) ?? Memory<Direction>.Empty;

        var stepsLength = 0;
        Point targetPoint;
        var rotation = Direction.Undefined;
        if (obj is IRotatable rotatable)
        {
            rotation = rotatable.Rotation;
        }

        if (obj is ISupportWalk supportWalk)
        {
            if (this.SendWalkDirections)
            {
                stepsLength = await supportWalk.GetDirectionsAsync(steps).ConfigureAwait(false);
                if (stepsLength > 0)
                {
                    // The last one is the rotation
                    rotation = steps.Span[stepsLength - 1];
                    steps = steps[..(stepsLength - 1)];
                    stepsLength--;
                }
            }

            targetPoint = supportWalk.WalkTarget;
        }
        else
        {
            targetPoint = obj.Position;
        }

        await this.SendWalkAsync(connection, objectId, obj.Position, targetPoint, steps, rotation, stepsLength).ConfigureAwait(false);
    }

    private void SetStepData(ObjectWalkedRef walkPacket, Span<Direction> steps, int stepsSize)
    {
        if (steps == default || walkPacket.StepCount == 0)
        {
            return;
        }

        walkPacket.StepData[0] = (byte)(steps[0].ToPacketByte() << 4 | stepsSize);
        for (int i = 0; i < stepsSize - 1; i += 2)
        {
            var index = 1 + (i / 2);
            var firstStep = steps[i].ToPacketByte();
            var secondStep = steps.Length > i + 1 ? steps[i + 1].ToPacketByte() : 0;
            walkPacket.StepData[index] = (byte)(firstStep << 4 | secondStep);
        }
    }

    private byte GetInstantMoveCode()
    {
        if (this._player.ClientVersion.Season == 0)
        {
            return 0x11;
        }

        switch (this._player.ClientVersion.Language)
        {
            case ClientLanguage.Japanese: return 0xDC;
            case ClientLanguage.English:
            case ClientLanguage.Vietnamese:
                return 0x15;
            case ClientLanguage.Filipino: return 0xD6;
            case ClientLanguage.Chinese:
            case ClientLanguage.Korean: return 0xD7;
            case ClientLanguage.Thai: return 0xD9;
            default:
                return (byte)PacketType.Teleport;
        }
    }

    protected byte GetWalkCode()
    {
        if (this._player.ClientVersion.Season == 0)
        {
            return 0x10;
        }

        switch (this._player.ClientVersion.Language)
        {
            case ClientLanguage.English: return 0xD4;
            case ClientLanguage.Japanese: return 0x1D;
            case ClientLanguage.Chinese:
            case ClientLanguage.Vietnamese:
                return 0xD9;
            case ClientLanguage.Filipino: return 0xDD;
            case ClientLanguage.Korean: return 0xD3;
            case ClientLanguage.Thai: return 0xD7;
            default:
                return (byte)PacketType.Walk;
        }
    }
}