// <copyright file="CastleSiegeTaxProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Provides the Castle Siege taxes which apply to a player and collects the resulting tribute.
/// </summary>
public sealed class CastleSiegeTaxProvider
{
    /// <summary>
    /// The maximum percentage which can be configured for Chaos Machine and NPC store taxes.
    /// </summary>
    internal const int MaximumPercentageTax = 3;

    /// <summary>
    /// The maximum Land of Trials entry fee.
    /// </summary>
    internal const int MaximumHuntTax = 300_000;

    /// <summary>
    /// The increment used when changing the Land of Trials entry fee.
    /// </summary>
    internal const int HuntTaxStep = 10_000;

    /// <summary>
    /// Gets the Chaos Machine tax rate which applies to a player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The additional percentage, or zero when no tax applies.</returns>
    public ValueTask<int> GetChaosTaxAsync(Player player)
        => this.GetTaxAsync(player, GetContext(player), CastleSiegeTaxType.ChaosMachine);

    /// <summary>
    /// Gets the NPC store tax rate which applies to a player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The additional percentage, or zero when no tax applies.</returns>
    public ValueTask<int> GetStoreTaxAsync(Player player)
        => this.GetTaxAsync(player, GetContext(player), CastleSiegeTaxType.Store);

    /// <summary>
    /// Gets the Land of Trials entry fee which applies to a player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The flat entry fee, or zero when the player is exempt.</returns>
    public ValueTask<int> GetHuntEntryFeeAsync(Player player)
        => this.GetHuntEntryFeeAsync(player, GetContext(player));

    /// <summary>
    /// Determines whether a player belongs to the castle owner's guild or alliance.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns><see langword="true"/> when the player is exempt from Castle Siege taxes.</returns>
    public ValueTask<bool> IsExemptAsync(Player player)
        => this.IsExemptAsync(player, GetContext(player));

    /// <summary>
    /// Removes a Chaos Machine crafting cost, including Castle Siege tax, and persists the tax as tribute.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="baseCost">The untaxed crafting cost.</param>
    /// <returns><see langword="true"/> when the player paid the complete cost.</returns>
    public ValueTask<bool> TryPayChaosCostAsync(Player player, int baseCost)
        => this.TryPayChaosCostAsync(
            player,
            baseCost,
            player.OpenedNpc?.Definition.NpcWindow == NpcWindow.ChaosMachine ? GetContext(player) : null);

    /// <summary>
    /// Removes an NPC store cost, including Castle Siege tax, and persists the tax as tribute.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="baseCost">The untaxed store cost.</param>
    /// <returns><see langword="true"/> when the player paid the complete cost.</returns>
    public ValueTask<bool> TryPayStoreCostAsync(Player player, long baseCost)
        => this.TryPayPercentageCostAsync(player, baseCost, GetContext(player), CastleSiegeTaxType.Store);

    /// <summary>
    /// Gets the initialized Castle Siege context of a player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The context, or <see langword="null"/> when Castle Siege is not active.</returns>
    internal static CastleSiegeContext? GetContext(Player player)
    {
        return player.GameContext.PlugInManager
            .GetActivePlugInsOf<IPeriodicTaskPlugIn>()
            .OfType<CastleSiegePlugIn>()
            .FirstOrDefault()
            ?.GetContext(player.GameContext);
    }

    /// <summary>
    /// Resolves the persistent identifier under which a player's guild participates in alliance events.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The persistent identifier of the alliance master guild, or <see langword="null"/>.</returns>
    internal static ValueTask<Guid?> GetPersistentAllianceMasterGuildIdAsync(Player player)
    {
        return player.GuildStatus is { } guildStatus
               && player.GameContext is IGameServerContext gameServerContext
            ? gameServerContext.GuildServer.GetPersistentAllianceMasterGuildIdAsync(guildStatus.GuildId)
            : new ValueTask<Guid?>((Guid?)null);
    }

    /// <summary>
    /// Resolves the persistent guild identifier when the player is its guild master.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The persistent guild identifier, or <see langword="null"/>.</returns>
    internal static ValueTask<Guid?> GetPersistentGuildMasterIdAsync(Player player)
    {
        return player.GuildStatus is { Position: GuildPosition.GuildMaster } guildStatus
               && player.GameContext is IGameServerContext gameServerContext
            ? gameServerContext.GuildServer.GetPersistentGuildIdAsync(guildStatus.GuildId)
            : new ValueTask<Guid?>((Guid?)null);
    }

    /// <summary>
    /// Removes a Chaos Machine crafting cost against a known Castle Siege context.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="baseCost">The untaxed crafting cost.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns><see langword="true"/> when the player paid the complete cost.</returns>
    internal ValueTask<bool> TryPayChaosCostAsync(Player player, int baseCost, CastleSiegeContext? context)
        => this.TryPayPercentageCostAsync(
            player,
            baseCost,
            player.OpenedNpc?.Definition.NpcWindow == NpcWindow.ChaosMachine ? context : null,
            CastleSiegeTaxType.ChaosMachine);

    /// <summary>
    /// Removes an NPC store cost against a known Castle Siege context.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="baseCost">The untaxed store cost.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns><see langword="true"/> when the player paid the complete cost.</returns>
    internal ValueTask<bool> TryPayStoreCostAsync(Player player, long baseCost, CastleSiegeContext? context)
        => this.TryPayPercentageCostAsync(player, baseCost, context, CastleSiegeTaxType.Store);

    /// <summary>
    /// Gets the configured Land of Trials fee against a known Castle Siege context.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns>The flat entry fee, or zero when the player is exempt.</returns>
    internal ValueTask<int> GetHuntEntryFeeAsync(Player player, CastleSiegeContext? context)
        => this.GetTaxAsync(player, context, CastleSiegeTaxType.HuntZone);

    /// <summary>
    /// Determines whether a player belongs to the castle owner's guild or alliance.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns><see langword="true"/> when the player is exempt from Castle Siege taxes.</returns>
    internal async ValueTask<bool> IsExemptAsync(Player player, CastleSiegeContext? context)
    {
        if (!IsEconomyActive(context))
        {
            return false;
        }

        var persistentGuildId = await GetPersistentAllianceMasterGuildIdAsync(player).ConfigureAwait(false);
        return IsEconomyActive(context) && persistentGuildId == context.SiegeData.OwnerGuildId;
    }

    /// <summary>
    /// Determines whether a player is the guild master of the castle-owner guild.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns><see langword="true"/> when the player is the castle owner guild master.</returns>
    internal async ValueTask<bool> IsOwnerGuildMasterAsync(Player player, CastleSiegeContext? context)
    {
        if (!IsEconomyActive(context))
        {
            return false;
        }

        var persistentGuildId = await GetPersistentGuildMasterIdAsync(player).ConfigureAwait(false);
        return IsEconomyActive(context) && persistentGuildId == context.SiegeData.OwnerGuildId;
    }

    /// <summary>
    /// Pays the configured Land of Trials fee and persists it as tribute.
    /// </summary>
    /// <param name="player">The entering player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns><see langword="true"/> when entry is allowed and the required fee was paid.</returns>
    internal async ValueTask<bool> TryPayHuntEntryFeeAsync(Player player, CastleSiegeContext? context)
    {
        if (!IsEconomyActive(context))
        {
            return false;
        }

        var persistentGuildId = await GetPersistentAllianceMasterGuildIdAsync(player).ConfigureAwait(false);
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!IsEconomyActive(context))
            {
                return false;
            }

            var isExempt = persistentGuildId == context.SiegeData.OwnerGuildId;
            if (!isExempt && !context.SiegeData.IsHuntZoneEnabled)
            {
                return false;
            }

            var fee = isExempt ? 0 : Math.Min(context.SiegeData.TaxHunt, MaximumHuntTax);
            if (fee < 0)
            {
                return false;
            }

            return this.TryPayAndCollect(player, context, 0, fee);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    private static bool IsEconomyActive([NotNullWhen(true)] CastleSiegeContext? context)
        => context is
        {
            Configuration.Enabled: true,
            SiegeData:
            {
                IsOccupied: true,
                OwnerGuildId: not null,
            },
        };

    private static int GetTax(CastleSiegeContext context, CastleSiegeTaxType taxType)
    {
        return taxType switch
        {
            CastleSiegeTaxType.ChaosMachine => Math.Min((int)context.SiegeData.TaxChaos, MaximumPercentageTax),
            CastleSiegeTaxType.Store => Math.Min((int)context.SiegeData.TaxStore, MaximumPercentageTax),
            CastleSiegeTaxType.HuntZone => Math.Min(context.SiegeData.TaxHunt, MaximumHuntTax),
            _ => 0,
        };
    }

    private async ValueTask<int> GetTaxAsync(
        Player player,
        CastleSiegeContext? context,
        CastleSiegeTaxType taxType)
    {
        if (!IsEconomyActive(context))
        {
            return 0;
        }

        var persistentGuildId = await GetPersistentAllianceMasterGuildIdAsync(player).ConfigureAwait(false);
        return IsEconomyActive(context) && persistentGuildId != context.SiegeData.OwnerGuildId
            ? GetTax(context, taxType)
            : 0;
    }

    private async ValueTask<bool> TryPayPercentageCostAsync(
        Player player,
        long baseCost,
        CastleSiegeContext? context,
        CastleSiegeTaxType taxType)
    {
        if (baseCost is < 0 or > int.MaxValue)
        {
            return false;
        }

        if (!IsEconomyActive(context))
        {
            return player.TryRemoveMoney((int)baseCost);
        }

        if (GetTax(context, taxType) == 0)
        {
            return player.TryRemoveMoney((int)baseCost);
        }

        var persistentGuildId = await GetPersistentAllianceMasterGuildIdAsync(player).ConfigureAwait(false);
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!IsEconomyActive(context))
            {
                return player.TryRemoveMoney((int)baseCost);
            }

            var isExempt = persistentGuildId == context.SiegeData.OwnerGuildId;
            var rate = isExempt ? 0 : GetTax(context, taxType);
            var tax = checked((baseCost * rate) / 100);
            return this.TryPayAndCollect(player, context, baseCost, tax);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    private bool TryPayAndCollect(
        Player player,
        CastleSiegeContext context,
        long baseCost,
        long tax)
    {
        var totalCost = checked(baseCost + tax);
        if (totalCost is < 0 or > int.MaxValue)
        {
            return false;
        }

        if (tax > 0 && context.SiegeData.TributeMoney > long.MaxValue - tax)
        {
            return false;
        }

        if (!player.TryRemoveMoney((int)totalCost))
        {
            return false;
        }

        if (tax == 0)
        {
            return true;
        }

        context.SiegeData.TributeMoney += tax;
        context.IsEconomyPersistencePending = true;
        return true;
    }
}
