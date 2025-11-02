// <copyright file="ShowCashShopEventItemListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCashShopEventItemListPlugIn"/>.
/// </summary>
[PlugIn(nameof(ShowCashShopEventItemListPlugIn), "The default implementation of the IShowCashShopEventItemListPlugIn.")]
[Guid("B8C9D0E1-F2A3-4B5C-6D7E-8F9A0B1C2D3E")]
public class ShowCashShopEventItemListPlugIn : IShowCashShopEventItemListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCashShopEventItemListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCashShopEventItemListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowCashShopEventItemListAsync()
    {
        var connection = this._player.Connection;
        if (connection is not { Connected: true })
        {
            return;
        }

        var gameConfiguration = this._player.GameContext?.Configuration;
        if (gameConfiguration is null)
        {
            return;
        }

        // Get event items from game configuration
        var eventProducts = gameConfiguration.CashShopProducts
            .Where(p => p.IsEventItem && p.IsCurrentlyAvailable && p.Item is not null)
            .ToList();

        int Write()
        {
            const int productSize = 16; // CashShopProduct structure size
            var size = CashShopEventItemListResponseRef.GetRequiredSize(eventProducts.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new CashShopEventItemListResponseRef(span)
            {
                ItemCount = (byte)eventProducts.Count,
            };

            int offset = CashShopEventItemListResponseRef.GetRequiredSize(0);
            foreach (var product in eventProducts)
            {
                var productRef = new CashShopProductRef(span[offset..]);
                productRef.ProductId = (uint)product.ProductId;

                // Determine which coin type to use based on the price fields
                if (product.PriceWCoinC > 0)
                {
                    productRef.Price = (uint)product.PriceWCoinC;
                    productRef.CoinType = 0; // WCoinC
                }
                else if (product.PriceWCoinP > 0)
                {
                    productRef.Price = (uint)product.PriceWCoinP;
                    productRef.CoinType = 1; // WCoinP
                }
                else if (product.PriceGoblinPoints > 0)
                {
                    productRef.Price = (uint)product.PriceGoblinPoints;
                    productRef.CoinType = 2; // GoblinPoints
                }

                productRef.CategoryIndex = 0; // Default category
                productRef.ItemGroup = (ushort)product.Item!.Group;
                productRef.ItemNumber = (ushort)product.Item!.Number;
                productRef.ItemLevel = product.ItemLevel;

                offset += productSize;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
