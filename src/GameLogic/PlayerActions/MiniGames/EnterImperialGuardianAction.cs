// <copyright file="EnterImperialGuardianAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Action to enter the imperial guardian event.
/// </summary>
/// <remarks>
/// Each day of the week has its own <see cref="MiniGameDefinition"/>, whose game level is the day
/// (1 = monday, ..., 7 = sunday). From monday to saturday, a Gaion's Order is required to enter,
/// on sunday a Complete Secromicon. Each player needs an own ticket, so the definitions don't define
/// a ticket item. Instead, the ticket is checked and consumed here.
/// </remarks>
public class EnterImperialGuardianAction
{
    private const byte UndefinedTicketSlot = 0xFF;
    private const byte TicketGroup = 14;
    private const short GaionsOrderNumber = 102;
    private const short CompleteSecromiconNumber = 109;

    private readonly EnterMiniGameAction _enterAction = new();

    /// <summary>
    /// Gets the day of the week of the event (1 = monday, ..., 7 = sunday).
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="eventDefinition">The definition of the event.</param>
    /// <returns>The day of the week.</returns>
    public static byte GetDay(IGameContext gameContext, ImperialGuardianEventDefinition eventDefinition)
    {
        if (eventDefinition.FixedDay is >= 1 and <= ImperialGuardianEventDefinition.Sunday)
        {
            return eventDefinition.FixedDay;
        }

        var dayOfWeek = TimeZoneInfo.ConvertTime(DateTime.UtcNow, gameContext.ServerTimeZone).DayOfWeek;
        return dayOfWeek == DayOfWeek.Sunday ? ImperialGuardianEventDefinition.Sunday : (byte)dayOfWeek;
    }

    /// <summary>
    /// Tries to enter the imperial guardian event.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask TryEnterAsync(Player player)
    {
        var eventDefinition = ImperialGuardianFeaturePlugIn.GetEventDefinition(player.GameContext);
        var day = GetDay(player.GameContext, eventDefinition);
        var definition = player.GameContext.Configuration.MiniGameDefinitions
            .FirstOrDefault(d => d.Type == MiniGameType.ImperialGuardian && d.GameLevel == day);
        if (definition is null)
        {
            await ShowResultAsync(player, ImperialGuardianEnterResult.NotOpen, day).ConfigureAwait(false);
            return;
        }

        if (eventDefinition.IsPartyRequired && player.Party is null)
        {
            await ShowResultAsync(player, ImperialGuardianEnterResult.PartyRequired, day).ConfigureAwait(false);
            return;
        }

        if (FindTicket(player, day) is not { } ticket)
        {
            await ShowResultAsync(player, ImperialGuardianEnterResult.MissingTicket, day).ConfigureAwait(false);
            return;
        }

        if ((player.Attributes?[Stats.Level] ?? 0) < definition.MinimumCharacterLevel)
        {
            // The client doesn't show a message for this result.
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ImperialGuardianLevelTooLow), definition.MinimumCharacterLevel).ConfigureAwait(false);
            await ShowResultAsync(player, ImperialGuardianEnterResult.CharacterLevelTooLow, day).ConfigureAwait(false);
            return;
        }

        await this._enterAction.TryEnterMiniGameAsync(player, MiniGameType.ImperialGuardian, day, UndefinedTicketSlot).ConfigureAwait(false);
        if (player.CurrentMiniGame is ImperialGuardianContext)
        {
            await ConsumeTicketAsync(player, ticket).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Finds the ticket which is required to enter the event on the day.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="day">The day of the week.</param>
    /// <returns>The ticket, or <c>null</c> if the player has none.</returns>
    internal static Item? FindTicket(Player player, byte day)
    {
        var ticketNumber = day == ImperialGuardianEventDefinition.Sunday ? CompleteSecromiconNumber : GaionsOrderNumber;
        return player.Inventory?.Items.FirstOrDefault(item =>
            item.Durability > 0
            && item.Definition is { Group: TicketGroup } definition
            && definition.Number == ticketNumber);
    }

    private static ValueTask ShowResultAsync(Player player, ImperialGuardianEnterResult result, byte day)
    {
        return player.InvokeViewPlugInAsync<IImperialGuardianViewPlugIn>(p => p.ShowEnterResultAsync(result, day, 0, ImperialGuardianWeather.Sun, TimeSpan.Zero));
    }

    private static async ValueTask ConsumeTicketAsync(Player player, Item ticket)
    {
        ticket.Durability -= 1;
        if (ticket.Durability > 0)
        {
            await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(ticket, false)).ConfigureAwait(false);
            return;
        }

        if (player.Inventory is { } inventory)
        {
            await inventory.RemoveItemAsync(ticket).ConfigureAwait(false);
        }

        await player.DestroyInventoryItemAsync(ticket).ConfigureAwait(false);
    }
}
