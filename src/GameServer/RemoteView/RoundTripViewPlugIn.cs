// <copyright file="RoundTripViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IRoundTripViewPlugIn"/> which answers with a <see cref="RoundTripResponse"/>.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RoundTripViewPlugIn_Name), Description = nameof(PlugInResources.RoundTripViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("47EA0970-2417-4663-87F1-244C7DD11439")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class RoundTripViewPlugIn : IRoundTripViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoundTripViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public RoundTripViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask AnswerRoundTripAsync(uint token)
    {
        await this._player.Connection.SendRoundTripResponseAsync(token).ConfigureAwait(false);
    }
}
