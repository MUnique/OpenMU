// <copyright file="DuelStopRequestHandlerPlugInSeason3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Duel;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for duel stop request packets of the clients before Season 4, which stop the duel with its own code.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DuelStopRequestHandlerPlugInSeason3_Name), Description = nameof(PlugInResources.DuelStopRequestHandlerPlugInSeason3_Description), ResourceType = typeof(PlugInResources))]
[Guid("F9CBC20A-ED45-4647-A483-1DADCE1DF663")]
[MaximumClient(3, 255, ClientLanguage.Invariant)]
internal class DuelStopRequestHandlerPlugInSeason3 : IPacketHandlerPlugIn
{
    private readonly DuelActions _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DuelStopRequestSeason3.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._action.HandleStopDuelRequestAsync(player).ConfigureAwait(false);
    }
}
