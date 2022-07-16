// <copyright file="AvailableQuestsRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for available quest request packets (0xF6, 0x30 identifier).
/// </summary>
[PlugIn("Quest - Request available quests", "Packet handler for available quest request packets (0xF6, 0x30 identifier)")]
[Guid("12722085-06FE-4D03-848E-89180C017CDB")]
[BelongsToGroup(QuestGroupHandlerPlugIn.GroupKey)]
public class AvailableQuestsRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => AvailableQuestsRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await player.InvokeViewPlugInAsync<IShowAvailableQuestsPlugIn>(p => p.ShowAvailableQuestsAsync()).ConfigureAwait(false);
    }
}