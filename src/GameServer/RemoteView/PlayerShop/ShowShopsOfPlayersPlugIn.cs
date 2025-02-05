// <copyright file="ShowShopsOfPlayersPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.PlayerShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowShopsOfPlayersPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowShopsOfPlayersPlugIn", "The default implementation of the IShowShopsOfPlayersPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("619df3b3-6559-4336-975f-04a2f5867f38")]
public class ShowShopsOfPlayersPlugIn : IShowShopsOfPlayersPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowShopsOfPlayersPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowShopsOfPlayersPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowShopsOfPlayersAsync(ICollection<Player> playersWithShop)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int Write()
        {
            var size = PlayerShopsRef.GetRequiredSize(playersWithShop.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new PlayerShopsRef(span)
            {
                ShopCount = (byte)playersWithShop.Count,
            };

            int i = 0;
            foreach (var shopPlayer in playersWithShop)
            {
                var shopBlock = packet[i];
                if (shopPlayer.ShopStorage is not null
                    && shopPlayer.SelectedCharacter?.StoreName is { } storeName)
                {
                    shopBlock.PlayerId = shopPlayer.GetId(this._player);
                    shopBlock.StoreName = storeName;
                }

                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}