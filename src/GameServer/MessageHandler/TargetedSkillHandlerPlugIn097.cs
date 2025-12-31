// <copyright file="TargetedSkillHandlerPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for targeted skill packets for version 0.97.
/// </summary>
[PlugIn(nameof(TargetedSkillHandlerPlugIn097), "Handler for targeted skill packets for version 0.97.")]
[Guid("da02baca-7dcc-468d-aa59-33eda2e5b69f")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
internal sealed class TargetedSkillHandlerPlugIn097 : TargetedSkillHandlerPlugIn
{
    /// <inheritdoc />
    public override async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var span = packet.Span;
        if (span.Length < 6)
        {
            return;
        }

        var skillId = span[3];
        var targetId = BinaryPrimitives.ReadUInt16BigEndian(span.Slice(4, 2));
        await this.HandleAsync(player, skillId, targetId).ConfigureAwait(false);
    }
}
