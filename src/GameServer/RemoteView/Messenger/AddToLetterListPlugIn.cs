// <copyright file="AddToLetterListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IAddToLetterListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("AddToLetterListPlugIn", "The default implementation of the IAddToLetterListPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("d30bea99-9d77-4182-99be-e08095c1969f")]
public class AddToLetterListPlugIn : IAddToLetterListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddToLetterListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AddToLetterListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask AddToLetterListAsync(LetterHeader letter, ushort newLetterIndex, bool newLetter)
    {
        await this._player.Connection.SendAddLetterAsync(
            newLetterIndex,
            letter.SenderName ?? string.Empty,
            letter.LetterDate.ToUniversalTime().AddHours(this._player.Account?.TimeZone ?? 0).ToString("yyyy-MM-dd HH:mm:ss"),
            letter.Subject ?? string.Empty,
            newLetter ? AddLetter.LetterState.New : letter.ReadFlag ? AddLetter.LetterState.Read : AddLetter.LetterState.Unread)
            .ConfigureAwait(false);
    }
}