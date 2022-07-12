// <copyright file="ActiveQuestListRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for the list of active quests request packets (0xF6, 0x1A identifier).
/// </summary>
[PlugIn("Quest - Request active quests list", "Packet handler for character focus packets (0xF3, 0x15 identifier).")]
[Guid("521ED931-BDB0-422E-8E8A-3CCB1BEB639C")]
[BelongsToGroup(QuestGroupHandlerPlugIn.GroupKey)]
internal class ActiveQuestListRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => ActiveQuestListRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await player.InvokeViewPlugInAsync<ICurrentlyActiveQuestsPlugIn>(p => p.ShowActiveQuestsAsync()).ConfigureAwait(false);
    }
}