// <copyright file="EnterOnGatekeeperHandlerPlugIn.cs" company="MUnique">
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
/// Handler for <see cref="EnterOnGatekeeperRequest"/> packets.
/// Called when: A player is running the quest "Into the 'Darkness' Zone" (nr. 6), talking to the gatekeeper npc in 'Barracks of Balgass'.
/// </summary>
[PlugIn(nameof(EnterOnGatekeeperHandlerPlugIn), "Handler for EnterOnGatekeeperRequest packets.")]
[Guid("9133CB87-6776-48B8-987B-93806531B60C")]
[MinimumClient(3, 0, ClientLanguage.Invariant)]
[BelongsToGroup(NpcActionGroupHandlerPlugIn.GroupKey)]
internal class EnterOnGatekeeperHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <summary>
    /// The enter action.
    /// </summary>
    private readonly EnterBalgassRefugeeAction _enterAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => EnterOnGatekeeperRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._enterAction.TryEnterQuestMapAsync(player).ConfigureAwait(false);
    }
}