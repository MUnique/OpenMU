// <copyright file="LuckyCoinCountRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameServer.MessageHandler.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the request of the count of the registered lucky coins, sent when the lucky coin registration dialog of the NPC Delgado opens.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.LuckyCoinCountRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.LuckyCoinCountRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E8340144-4DC4-4F7A-8F2B-530EE9853742")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
internal class LuckyCoinCountRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly LuckyCoinActions _actions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => LuckyCoinCountRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < LuckyCoinCountRequest.Length)
        {
            return;
        }

        await this._actions.ShowRegisteredCountAsync(player).ConfigureAwait(false);
    }
}
