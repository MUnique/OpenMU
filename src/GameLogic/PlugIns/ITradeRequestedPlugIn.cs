// <copyright file="ITradeRequestedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a player asks another player for a trade.
/// </summary>
/// <remarks>
/// It's called before the request is checked or shown, so it also sees the requests which can't
/// be delivered - e.g. to a server-side bot, which has no trade window.
/// </remarks>
[Guid("CF5C02DA-A5D3-4587-8EE9-B7F03217F19D")]
[PlugInPoint("Trade requested", "Plugins which are called when a player asks another player for a trade.")]
public interface ITradeRequestedPlugIn
{
    /// <summary>
    /// Is called when the player asks the partner for a trade.
    /// </summary>
    /// <param name="requester">The asking player.</param>
    /// <param name="partner">The asked player.</param>
    /// <returns>The task.</returns>
    ValueTask TradeRequestedAsync(Player requester, Player partner);
}
