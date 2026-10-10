// <copyright file="ShowDuelRequestPlugInSeason3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="IShowDuelRequestPlugIn"/> for the clients before Season 4,
/// which get the duel request with its own packet code instead of a sub code of the duel packet.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowDuelRequestPlugInSeason3_Name), Description = nameof(PlugInResources.ShowDuelRequestPlugInSeason3_Description), ResourceType = typeof(PlugInResources))]
[Guid("400D1B54-8B0A-489F-9360-0F380625CC77")]
[MaximumClient(3, 255, ClientLanguage.Invariant)]
public class ShowDuelRequestPlugInSeason3 : IShowDuelRequestPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowDuelRequestPlugInSeason3"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowDuelRequestPlugInSeason3(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowDuelRequestAsync(Player requester)
    {
        await this._player.Connection.SendDuelStartRequestSeason3Async(requester.GetId(this._player), requester.Name).ConfigureAwait(false);
    }
}
