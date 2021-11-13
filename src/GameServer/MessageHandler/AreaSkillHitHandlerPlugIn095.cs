// <copyright file="AreaSkillHitHandlerPlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for area skill hit packets.
/// </summary>
/// <remarks>
/// TODO: It's usually required to perform a <see cref="AreaSkillAttackAction"/> before, so this check has to be implemented.
/// </remarks>
[PlugIn(nameof(AreaSkillHitHandlerPlugIn095), "Handler for area skill hit packets.")]
[Guid("71C2E116-D1B2-4F07-9A3B-41CEA1975108")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
internal class AreaSkillHitHandlerPlugIn095 : AreaSkillHitHandlerMultiTargetPlugInBase, IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => AreaSkillHit095.Code;

    /// <inheritdoc/>
    public void HandlePacket(Player player, Span<byte> packet)
    {
        if (packet.Length < 8)
        {
            return;
        }

        AreaSkillHit095 message = packet;

        if (packet.Length < AreaSkillHit095.GetRequiredSize(message.TargetCount)
            || !this.TryGetSkillEntry(player, message.SkillIndex, out var skillEntry))
        {
            return;
        }

        for (var i = 0; i < message.TargetCount; i++)
        {
            this.AttackTarget(player, skillEntry, message[i].TargetId);
        }
    }
}