// <copyright file="DiscordIntegrationInfoRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for the request of the Discord integration info. The request also announces that the client supports the external chat messages.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DiscordIntegrationInfoRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.DiscordIntegrationInfoRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("956C19CC-95CD-4EC0-8E38-63D62B47B127")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
[BelongsToGroup(ChatCommandGroupHandlerPlugIn.GroupKey)]
internal class DiscordIntegrationInfoRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly DiscordIntegrationAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DiscordIntegrationInfoRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (player.SelectedCharacter is null)
        {
            return;
        }

        await this._action.ShowIntegrationInfoAsync(player).ConfigureAwait(false);
    }
}
