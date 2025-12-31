// <copyright file="AreaSkillAttackHandlerPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for area skill attack packets for version 0.97.
/// </summary>
[PlugIn(nameof(AreaSkillAttackHandlerPlugIn097), "Handler for area skill attack packets for version 0.97.")]
[Guid("b7e54b88-0e22-4c2d-8f49-307ece733ce1")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
internal sealed class AreaSkillAttackHandlerPlugIn097 : IPacketHandlerPlugIn
{
    private readonly AreaSkillAttackAction _attackAction = new();

    /// <inheritdoc />
    public bool IsEncryptionExpected => true;

    /// <inheritdoc />
    public byte Key => AreaSkill095.Code;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var span = packet.Span;
        if (span.Length < 7)
        {
            return;
        }

        var skillId = span[3];
        var targetX = span[4];
        var targetY = span[5];
        var rotation = span[6];
        ushort targetId = 0;
        if (span.Length >= 11)
        {
            targetId = BinaryPrimitives.ReadUInt16BigEndian(span.Slice(9, 2));
        }

        await this._attackAction.AttackAsync(player, targetId, skillId, new Point(targetX, targetY), rotation).ConfigureAwait(false);
    }
}
