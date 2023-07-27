// <copyright file="WanderingMerchantsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WanderingMerchants;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugins spawns and moves the wandering merchants.
/// </summary>
[PlugIn(nameof(WanderingMerchantsPlugIn), "This plugins spawns and moves the wandering merchants.")]
[Guid("8B2CD316-C4B0-452F-8C7D-CE696356D437")]
public class WanderingMerchantsPlugIn : PeriodicTaskBasePlugIn<WanderingMerchantsConfiguration, WanderingMerchantsState>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new WanderingMerchantsConfiguration
        {
            Message = null,
            PreStartMessageDelay = TimeSpan.Zero,

            // we check every minute if we have to move a merchant.
            TaskDuration = TimeSpan.FromMinutes(1),
            MinimumSpawnDuration = TimeSpan.FromMinutes(60),
            MaximumSpawnDuration = TimeSpan.FromMinutes(180),
        };
    }

    /// <inheritdoc />
    protected override bool IsItTimeToStart(IGameContext gameContext)
    {
        var state = this.GetStateByGameContext(gameContext);
        if (!state.Merchants.Any())
        {
            return false;
        }

        var minNext = state.Merchants.Min(m => m.NextWanderingAt);
        return DateTime.UtcNow >= minNext;
    }

    /// <inheritdoc />
    protected override async ValueTask OnStartedAsync(WanderingMerchantsState state)
    {
        var wanderingNow = state.Merchants.Where(m => m.NextWanderingAt <= DateTime.UtcNow).ToList();
        foreach (var merchantState in wanderingNow)
        {
            await this.HandleMerchantAsync(state, merchantState).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override WanderingMerchantsState CreateState(IGameContext gameContext)
    {
        return new WanderingMerchantsState(gameContext);
    }

    /// <inheritdoc />
    protected override ValueTask OnFinishedAsync(WanderingMerchantsState state)
    {
        return default;
    }

    /// <inheritdoc />
    protected override ValueTask OnPrepareEventAsync(WanderingMerchantsState state)
    {
        return default;
    }

    /// <inheritdoc />
    protected override ValueTask OnPreparedAsync(WanderingMerchantsState state)
    {
        return default;
    }

    private async Task HandleMerchantAsync(WanderingMerchantsState state, MerchantSpawnState merchantState)
    {
        var logger = state.Context.LoggerFactory.CreateLogger(this.GetType().Name);
        using var scope = logger.BeginScope(state.Context);

        var oldMerchant = merchantState.Merchant;
        var nextSpawn = merchantState.PossibleSpawns.Count == 1
            ? merchantState.PossibleSpawns.First()
            : merchantState.PossibleSpawns.Where(s => s != merchantState.Merchant?.SpawnArea).SelectRandom()!;

        if (oldMerchant?.SpawnArea == nextSpawn)
        {
            logger.LogDebug("Same spawn area for merchant {merchant}.", oldMerchant);
            return;
        }

        if (nextSpawn.GameMap is not { } nextMapDefinition)
        {
            logger.LogWarning("Spawn area {spawnArea} has no map defined.", nextSpawn);
            return;
        }

        var nextMap = await state.Context.GetMapAsync((ushort)nextMapDefinition.Number).ConfigureAwait(false);
        if (nextMap is null)
        {
            logger.LogWarning("Could not create map {map} for next wandering.", nextMapDefinition);
            return;
        }

        await this.RemoveOldMerchantIfNeededAsync(oldMerchant, logger, merchantState).ConfigureAwait(false);
        await this.SpawnMerchantAsync(nextSpawn, merchantState, nextMap, logger).ConfigureAwait(false);
    }

    private async Task RemoveOldMerchantIfNeededAsync(NonPlayerCharacter? merchant, ILogger logger, MerchantSpawnState merchantState)
    {
        if (merchant is null)
        {
            return;
        }

        var oldMap = merchant.CurrentMap;
        await oldMap.RemoveAsync(merchant).ConfigureAwait(false);
        await merchant.DisposeAsync().ConfigureAwait(false);
        logger.LogDebug("Old merchant {merchant} has been removed from {map}.", merchant, oldMap.Definition);
        merchantState.Merchant = null;
    }

    private async Task SpawnMerchantAsync(MonsterSpawnArea nextSpawn, MerchantSpawnState merchantState, GameMap nextMap, ILogger logger)
    {
        var newMerchant = new NonPlayerCharacter(nextSpawn, merchantState.MerchantDefinition, nextMap);
        merchantState.Merchant = newMerchant;
        newMerchant.Initialize();
        await nextMap.AddAsync(newMerchant).ConfigureAwait(false);
        merchantState.NextWanderingAt = DateTime.UtcNow.AddMinutes(Rand.NextInt((int)this.Configuration!.MinimumSpawnDuration.TotalMinutes, (int)this.Configuration!.MaximumSpawnDuration.TotalMinutes));
        logger.LogDebug("New merchant {merchant} has been created on {map}. Next wandering at {next}", newMerchant, nextMap.Definition, merchantState.NextWanderingAt);
    }
}