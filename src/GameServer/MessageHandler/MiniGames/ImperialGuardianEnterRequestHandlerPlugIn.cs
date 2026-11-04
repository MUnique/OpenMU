// <copyright file="ImperialGuardianEnterRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameServer.Properties;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the request to enter the imperial guardian event (0xF7 0x01), which is sent by the window of Jerint.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ImperialGuardianEnterRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.ImperialGuardianEnterRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C83F2A6D-91E4-4B57-A0D6-2E7B5F48C913")]
[BelongsToGroup(ImperialGuardianGroupHandlerPlugIn.GroupKey)]
internal class ImperialGuardianEnterRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly EnterImperialGuardianAction _enterAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => EnterEmpireGuardianEvent.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        // The client sends the request again with each click on the button, so it's ignored while the player is in a game.
        if (packet.Length < EnterEmpireGuardianEvent.Length
            || player.SelectedCharacter?.CharacterClass is null
            || player.CurrentMiniGame is not null
            || player.OpenedNpc?.Definition.NpcWindow != NpcWindow.JerintGaionEvententry)
        {
            return;
        }

        // The item slot of the request is always 1, so the ticket is searched in the inventory.
        await this._enterAction.TryEnterAsync(player).ConfigureAwait(false);

        if (player.CurrentMiniGame is not null)
        {
            // The client closes the window when it changes the map, so the player isn't at Jerint anymore.
            player.OpenedNpc = null;
        }
    }
}
