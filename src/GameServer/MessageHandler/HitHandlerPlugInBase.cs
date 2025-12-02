// <copyright file="HitHandlerPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Network.Packets.ClientToServer;

/// <summary>
/// Handler for hit packets.
/// </summary>
internal abstract class HitHandlerPlugInBase : IPacketHandlerPlugIn
{
    private readonly HitAction _hitAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public abstract byte Key { get; }

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < 7)
        {
            return;
        }

        using var loggerScope = player.Logger.BeginScope(this.GetType());
        HitRequest message = packet;
        var currentMap = player.CurrentMap;
        if (currentMap is null)
        {
            player.Logger.LogWarning($"Current player map not set. Possible hacker action. Character name: {player.Name}");
            return;
        }

        if (currentMap.GetObject(message.TargetId) is not IAttackable target)
        {
            player.Logger.LogWarning($"Object {message.TargetId} of current player map not found alive. Possible hacker action. Character name: {player.Name}");
        }
        else
        {
            await this._hitAction.HitAsync(player, target, message.AttackAnimation, message.LookingDirection.ParseAsDirection()).ConfigureAwait(false);
        }
    }
}