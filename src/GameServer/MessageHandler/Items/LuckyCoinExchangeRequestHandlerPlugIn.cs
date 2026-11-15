// <copyright file="LuckyCoinExchangeRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for the request to exchange lucky coins for an item at the NPC Delgado.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.LuckyCoinExchangeRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.LuckyCoinExchangeRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("8998E334-1714-4F4F-9467-D4DD76915BCE")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
internal class LuckyCoinExchangeRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly LuckyCoinActions _actions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => LuckyCoinExchangeRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < LuckyCoinExchangeRequest.Length)
        {
            return;
        }

        LuckyCoinExchangeRequest request = packet;
        await this._actions.ExchangeAsync(player, (int)request.CoinCount).ConfigureAwait(false);
    }
}
