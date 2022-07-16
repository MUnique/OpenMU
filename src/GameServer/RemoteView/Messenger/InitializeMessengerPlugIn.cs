// <copyright file="InitializeMessengerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IInitializeMessengerPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("InitializeMessengerPlugIn", "The default implementation of the IInitializeMessengerPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("cb079c23-e90c-473d-87ae-317937158924")]
public class InitializeMessengerPlugIn : IInitializeMessengerPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeMessengerPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public InitializeMessengerPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask InitializeMessengerAsync(MessengerInitializationData data, int maxLetters)
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.SelectedCharacter is null)
        {
            return;
        }

        int Write()
        {
            var friendList = data.Friends;
            var size = MessengerInitializationRef.GetRequiredSize(friendList.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var message = new MessengerInitializationRef(span)
            {
                FriendCount = (byte)friendList.Count,
                LetterCount = (byte)this._player.SelectedCharacter.Letters.Count,
                MaximumLetterCount = (byte)maxLetters,
            };

            int i = 0;
            foreach (var friend in friendList)
            {
                var friendBlock = message[i];
                friendBlock.Name = friend;
                friendBlock.ServerId = (byte)SpecialServerId.Offline;
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);

        foreach (var requesterName in data.OpenFriendRequests)
        {
            await this._player.InvokeViewPlugInAsync<IShowFriendRequestPlugIn>(p => p.ShowFriendRequestAsync(requesterName)).ConfigureAwait(false);
        }

        var letters = this._player.SelectedCharacter.Letters;
        for (ushort l = 0; l < letters.Count; l++)
        {
            await this._player.InvokeViewPlugInAsync<IAddToLetterListPlugIn>(p => p.AddToLetterListAsync(letters[l], l, false)).ConfigureAwait(false);
        }
    }
}