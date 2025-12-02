// <copyright file="BuffRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for (elf soldier) buff request packets (0xF6, 0x31 identifier).
/// </summary>
[PlugIn("Quest - Request available quests", "Packet handler for (elf soldier) buff request packets (0xF6, 0x31 identifier)")]
[Guid("98F31A99-33CE-46FF-98BF-B66EF509C277")]
[BelongsToGroup(QuestGroupHandlerPlugIn.GroupKey)]
public class BuffRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly ElfSoldierBuffRequestAction _buffRequestAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => NpcBuffRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._buffRequestAction.RequestBuffAsync(player).ConfigureAwait(false);
    }
}