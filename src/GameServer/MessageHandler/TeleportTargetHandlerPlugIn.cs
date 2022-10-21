// <copyright file="TeleportTargetHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for teleport target packets (teleport ally skill).
/// </summary>
[PlugIn(nameof(TeleportTargetHandlerPlugIn), "Handler for target teleport packets of the teleport ally skill.")]
[Guid("279881F9-0AE9-4EDA-8EB1-34D99D3243CC")]
internal class TeleportTargetHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly WizardTeleportAction _teleportAction = new();

    /// <inheritdoc />
    public byte Key => TeleportTargetRef.Code;

    /// <inheritdoc />
    public bool IsEncryptionExpected => true;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        TeleportTarget message = packet;

        await this._teleportAction.TryTeleportTargetWithSkillAsync(
            player,
            message.TargetId,
            new Point(message.TeleportTargetX, message.TeleportTargetY)).ConfigureAwait(false);
    }
}