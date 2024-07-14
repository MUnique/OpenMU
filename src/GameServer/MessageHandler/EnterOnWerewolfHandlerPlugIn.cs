// <copyright file="EnterOnWerewolfHandlerPlugIn.cs" company="MUnique">
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
/// Handler for <see cref="EnterOnWerewolfRequest"/> packets.
/// Called when: A player is running the quest "Infiltrate The Barracks of Balgass" (nr. 5), talking to the Werewolf npc in Crywolf.
/// </summary>
[PlugIn(nameof(EnterOnWerewolfHandlerPlugIn), "Handler for EnterOnWerewolfRequest packets.")]
[Guid("3ECA37DE-2D8E-4617-8A44-B1A616B7C74C")]
[MinimumClient(3, 0, ClientLanguage.Invariant)]
[BelongsToGroup(NpcActionGroupHandlerPlugIn.GroupKey)]
internal class EnterOnWerewolfHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <summary>
    /// The enter action.
    /// </summary>
    private readonly EnterBarracksOfBalgassAction _enterAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => EnterOnWerewolfRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._enterAction.TryEnterQuestMapAsync(player).ConfigureAwait(false);
    }
}