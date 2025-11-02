// <copyright file="ShowLetterPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowLetterPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowLetterPlugIn", "The default implementation of the IShowLetterPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("6ce73080-6916-465f-b6d1-34641687ded3")]
public class ShowLetterPlugIn : IShowLetterPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowLetterPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowLetterPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowLetterAsync(LetterBody letter)
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.SelectedCharacter is null)
        {
            return;
        }

        var appearanceSerializer = this._player.AppearanceSerializer;
        var letterIndex = this._player.SelectedCharacter.Letters.IndexOf(letter.Header!);
        int Write()
        {
            var size = OpenLetterRef.GetRequiredSize(letter.Message);
            var span = connection.Output.GetSpan(size)[..size];
            var result = new OpenLetterRef(span)
            {
                LetterIndex = (ushort)letterIndex,
                MessageSize = (ushort)Encoding.UTF8.GetByteCount(letter.Message),
                Animation = letter.Animation,
                Rotation = letter.Rotation,
                Message = letter.Message,
            };

            // The GM sign is the CharacterStatus.GameMaster value (32) in the appearance data.
            if (letter.SenderAppearance is not null)
            {
                appearanceSerializer.WriteAppearanceData(result.SenderAppearance, letter.SenderAppearance, false);
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}