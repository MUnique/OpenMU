// <copyright file="ChatCommandListRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the request of the list of available chat commands.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ChatCommandListRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.ChatCommandListRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B1E3C0F5-5D6A-4E52-9A73-8C0F1D2B4A66")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
[BelongsToGroup(ChatCommandGroupHandlerPlugIn.GroupKey)]
internal class ChatCommandListRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => ChatCommandListRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (player.SelectedCharacter is null)
        {
            return;
        }

        var commands = player.GetAvailableChatCommandInfos().ToList();
        await player.InvokeViewPlugInAsync<IChatCommandListViewPlugIn>(p => p.ShowChatCommandListAsync(commands)).ConfigureAwait(false);
    }
}
