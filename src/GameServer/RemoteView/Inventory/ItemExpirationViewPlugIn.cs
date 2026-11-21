// <copyright file="ItemExpirationViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="IItemExpirationViewPlugIn"/> which sends the period item packets (D2 11 and
/// D2 12) of the season 6 client. The client stores the expiration date per inventory slot and shows the time left
/// in the tooltip of the item.
/// </summary>
/// <remarks>
/// The client takes the date for the item which is in the slot when the packet arrives, so it's sent after the item.
/// It's needed again whenever the client creates the item again, e.g. after a move.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.ItemExpirationViewPlugIn_Name), Description = nameof(PlugInResources.ItemExpirationViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("03C4779E-F5B9-4D52-89C7-7F80FB4929AE")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
public class ItemExpirationViewPlugIn : IItemExpirationViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemExpirationViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ItemExpirationViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowExpirationsAsync(IEnumerable<Item> items)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        var timeLimitedItems = items.Where(item => item.ExpiresAt is not null).ToList();
        if (timeLimitedItems.Count == 0)
        {
            return;
        }

        await connection.SendPeriodItemListCountAsync().ConfigureAwait(false);
        foreach (var item in timeLimitedItems)
        {
            await connection.SendPeriodItemInfoAsync(item.ItemSlot, GetExpirationDate(item.ExpiresAt!.Value)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask ShowExpirationAsync(Item item)
    {
        if (item.ExpiresAt is not { } expiresAt || this._player.Connection is not { } connection)
        {
            return;
        }

        await connection.SendPeriodItemInfoAsync(item.ItemSlot, GetExpirationDate(expiresAt)).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the expiration date in Unix seconds, as the client expects it.
    /// </summary>
    private static uint GetExpirationDate(DateTime expiresAt)
    {
        var seconds = new DateTimeOffset(DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc)).ToUnixTimeSeconds();
        return (uint)Math.Clamp(seconds, 0, uint.MaxValue);
    }
}
