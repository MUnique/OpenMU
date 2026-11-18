// <copyright file="ShowLetterExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowLetterPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowLetterExtendedPlugIn_Name), Description = nameof(PlugInResources.ShowLetterExtendedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("9FC6D771-FCCE-4780-B68E-5FBFF3FE1EB0")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class ShowLetterExtendedPlugIn : IShowLetterPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowLetterExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowLetterExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowLetterAsync(LetterBody letterBody)
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.SelectedCharacter is null)
        {
            return;
        }

        var appearanceSerializer = this._player.AppearanceSerializer;
        var letterIndex = this._player.SelectedCharacter.Letters.IndexOf(letterBody.Header!);
        int Write()
        {
            var size = OpenLetterExtendedRef.GetRequiredSize(letterBody.Message);
            var span = connection.Output.GetSpan(size)[..size];
            var result = new OpenLetterExtendedRef(span)
            {
                LetterIndex = (ushort)letterIndex,
                Animation = letterBody.Animation,
                Rotation = letterBody.Rotation,
                Message = letterBody.Message,
            };

            if (letterBody.SenderAppearance is not null)
            {
                appearanceSerializer.WriteAppearanceData(result.SenderAppearance, letterBody.SenderAppearance, false);
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}