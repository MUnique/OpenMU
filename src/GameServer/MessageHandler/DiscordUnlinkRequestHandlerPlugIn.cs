// <copyright file="DiscordUnlinkRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Discord;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the request to remove the link of the account to a Discord user.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DiscordUnlinkRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.DiscordUnlinkRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("AE310CAA-1876-453A-B3B4-58137915CF50")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
[BelongsToGroup(ChatCommandGroupHandlerPlugIn.GroupKey)]
internal class DiscordUnlinkRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly DiscordIntegrationAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DiscordUnlinkRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (player.SelectedCharacter is null)
        {
            return;
        }

        await this._action.UnlinkAndShowInfoAsync(player).ConfigureAwait(false);
    }
}
