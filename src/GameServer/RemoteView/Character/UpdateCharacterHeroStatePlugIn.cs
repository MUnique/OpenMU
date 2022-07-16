// <copyright file="UpdateCharacterHeroStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateCharacterHeroStatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("UpdateCharacterHeroStatePlugIn", "The default implementation of the IUpdateCharacterHeroStatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("d1ce36d6-cbdd-4bcb-99c7-c7495d8597d9")]
public class UpdateCharacterHeroStatePlugIn : IUpdateCharacterHeroStatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCharacterHeroStatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateCharacterHeroStatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateCharacterHeroStateAsync(Player affectedPlayer)
    {
        if (affectedPlayer.SelectedCharacter is { } character)
        {
            await this._player.Connection.SendHeroStateChangedAsync(affectedPlayer.GetId(this._player), character.State.Convert()).ConfigureAwait(false);
        }
    }
}