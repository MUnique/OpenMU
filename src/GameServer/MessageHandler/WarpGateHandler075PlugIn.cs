// <copyright file="WarpGateHandler075PlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for warp gate packets.
/// This one is called when a player has entered a gate area, and sends a gate enter request.
/// </summary>
[PlugIn(nameof(WarpGateHandler075PlugIn), "Handler for warp gate packets.")]
[Guid("264CFC8C-30D1-4536-97FD-6811A9544997")]
internal class WarpGateHandler075PlugIn : IPacketHandlerPlugIn
{
    private readonly WarpGateAction _warpAction = new();

    private readonly WizardTeleportAction _teleportAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => EnterGateRequest075.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < EnterGateRequest075.Length)
        {
            return;
        }

        EnterGateRequest075 request = packet;
        var gateNumber = request.GateNumber;

        if (gateNumber == 0)
        {
            await this._teleportAction.TryTeleportWithSkillAsync(player, new Point(request.TeleportTargetX, request.TeleportTargetY)).ConfigureAwait(false);
            return;
        }

        var gate = player.SelectedCharacter?.CurrentMap?.EnterGates.FirstOrDefault(g => g.Number == gateNumber);
        if (gate is null)
        {
            player.Logger.LogWarning("Gate {0} not found in current map {1}", gateNumber, player.SelectedCharacter?.CurrentMap);
            return;
        }

        await this._warpAction.EnterGateAsync(player, gate).ConfigureAwait(false);
    }
}