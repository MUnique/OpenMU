// <copyright file="ITrader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using System.Threading;

/// <summary>
/// Interface of a trader.
/// </summary>
public interface ITrader : IWorldObserver
{
    /// <summary>
    /// Gets the character name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the traders level.
    /// </summary>
    int Level { get; }

    /// <summary>
    /// Gets or sets the current trading partner.
    /// </summary>
    ITrader? TradingPartner { get; set; }

    /// <summary>
    /// Gets or sets the money which is currently in the trade.
    /// </summary>
    int TradingMoney { get; set; }

    /// <summary>
    /// Gets or sets the short guild identifier.
    /// </summary>
    GuildMemberStatus? GuildStatus { get; set; }

    /// <summary>
    /// Gets the inventory.
    /// </summary>
    IInventoryStorage? Inventory { get; }

    /// <summary>
    /// Gets the temporary storage, which holds the items of the trade.
    /// </summary>
    IStorage? TemporaryStorage { get; }

    /// <summary>
    /// Gets or sets the backup inventory.
    /// </summary>
    BackupItemStorage? BackupInventory { get; set; }

    /// <summary>
    /// Gets or sets the available money.
    /// </summary>
    int Money { get; set; }

    /// <summary>
    /// Gets the state of the player.
    /// </summary>
    StateMachine PlayerState { get; }

    /// <summary>
    /// Gets the persistence context of the trader. It needs to be updated when a trade finishes.
    /// </summary>
    IPlayerContext PersistenceContext { get; }

    /// <summary>
    /// Gets the game context of the trader.
    /// </summary>
    IGameContext GameContext { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is template player.
    /// In this case, trading is not allowed.
    /// </summary>
    bool IsTemplatePlayer { get; }

    /// <summary>
    /// Saves the progress of the trader.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Success of the save operation.</returns>
    ValueTask<bool> SaveProgressAsync(CancellationToken cancellationToken = default);
}