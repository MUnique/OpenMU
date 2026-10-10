// <copyright file="DuelEndedPlugInSeason3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="IDuelEndedPlugIn"/> for the clients before Season 4,
/// which get the end of the duel with its own packet code.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DuelEndedPlugInSeason3_Name), Description = nameof(PlugInResources.DuelEndedPlugInSeason3_Description), ResourceType = typeof(PlugInResources))]
[Guid("FF33F197-EE8D-4268-8658-8EB84AE7EFAF")]
[MaximumClient(3, 255, ClientLanguage.Invariant)]
public class DuelEndedPlugInSeason3 : IDuelEndedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelEndedPlugInSeason3"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelEndedPlugInSeason3(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask DuelEndedAsync()
    {
        await this._player.Connection.SendDuelEndSeason3Async(this._player.GetId(this._player), this._player.Name).ConfigureAwait(false);
    }
}
