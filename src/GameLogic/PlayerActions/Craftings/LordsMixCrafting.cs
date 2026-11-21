// <copyright file="LordsMixCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The Lord's Mix of the castle senior NPC: a crafting only for the guild master of the guild which owns the castle,
/// limited per day or per siege cycle by <see cref="CastleSiegeConfiguration.LordsMixLimit"/>.
/// </summary>
/// <remarks>
/// Its required items, price and results are configured in the <see cref="SimpleCraftingSettings"/>, like for other
/// simple craftings. The number of successful mixes is counted in <see cref="DataModel.Entities.CastleSiegeData.LordsMixCount"/>:
/// per day, a count of an earlier day doesn't count (server local time); per siege cycle, the castle siege resets it
/// when a siege ends.
/// </remarks>
public class LordsMixCrafting : SimpleItemCraftingHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LordsMixCrafting"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public LordsMixCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
    }

    /// <summary>
    /// Checks whether the player may use the Lord's Mix: the player has to be the guild master of the guild which owns the
    /// castle, and the limit of the period must not be reached yet. Otherwise, the player gets a message.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="context">The castle siege context.</param>
    /// <param name="today">The current date, in the server's local time.</param>
    /// <returns>The error, if the player can't use the Lord's Mix; otherwise, <c>null</c>.</returns>
    internal static async ValueTask<CraftingResult?> CheckPreconditionsAsync(Player player, CastleSiegeContext? context, DateTime today)
    {
        if (context is not { Configuration.Enabled: true, SiegeData.IsOccupied: true }
            || await CastleSiegeTaxProvider.GetPersistentGuildMasterIdAsync(player).ConfigureAwait(false) is not { } guildId
            || guildId != context.SiegeData.OwnerGuildId)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.LordsMixOnlyForCastleOwner)).ConfigureAwait(false);
            return CraftingResult.Failed;
        }

        if (IsLimitReached(context, today))
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.LordsMixLimitReached)).ConfigureAwait(false);
            return CraftingResult.Failed;
        }

        return null;
    }

    /// <summary>
    /// Counts a successful Lord's Mix in the castle siege data and saves it.
    /// </summary>
    /// <param name="context">The castle siege context.</param>
    /// <param name="today">The current date, in the server's local time.</param>
    internal static async ValueTask CountSuccessfulMixAsync(CastleSiegeContext context, DateTime today)
    {
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            context.SiegeData.LordsMixCount = GetCurrentCount(context, today) + 1;
            // The calendar date of the server's local time, stored with the UTC kind which the database requires.
            context.SiegeData.LordsMixCountDate = DateTime.SpecifyKind(today.Date, DateTimeKind.Utc);
            await context.SaveOwnerAsync().ConfigureAwait(false);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    /// <inheritdoc />
    protected override ValueTask<CraftingResult?> CheckPreconditionsAsync(Player player)
    {
        return CheckPreconditionsAsync(player, CastleSiegeContextResolver.GetContext(player), GetToday());
    }

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate)
    {
        var resultItems = await base.CreateOrModifyResultItemsAsync(requiredItems, player, socketSlot, successRate).ConfigureAwait(false);
        if (resultItems.Count > 0 && CastleSiegeContextResolver.GetContext(player) is { } context)
        {
            await CountSuccessfulMixAsync(context, GetToday()).ConfigureAwait(false);
        }

        return resultItems;
    }

    private static DateTime GetToday() => DateTime.Now.Date;

    private static bool IsLimitReached(CastleSiegeContext context, DateTime today)
    {
        var limit = context.Configuration.LordsMixLimit;
        return limit > 0 && GetCurrentCount(context, today) >= limit;
    }

    /// <summary>
    /// Gets the number of mixes of the current period: per day, a count of an earlier day is 0.
    /// </summary>
    private static int GetCurrentCount(CastleSiegeContext context, DateTime today)
    {
        return context.Configuration.LordsMixLimitPeriod == LordsMixLimitPeriod.Day
               && context.SiegeData.LordsMixCountDate?.Date != today.Date
            ? 0
            : context.SiegeData.LordsMixCount;
    }
}
