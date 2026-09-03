// <copyright file="CastleSiegeAdministration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;

/// <summary>
/// Provides serialized administrative operations and read-only status snapshots for one Castle Siege plug-in.
/// </summary>
/// <remarks>
/// This is deliberately a game-logic service instead of exposing mutable runtime state to management clients.
/// It is intended for trusted in-process callers, such as the administration panel.
/// </remarks>
public sealed class CastleSiegeAdministration
{
    private readonly CastleSiegePlugIn _plugIn;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeAdministration"/> class.
    /// </summary>
    /// <param name="plugIn">The active Castle Siege plug-in.</param>
    public CastleSiegeAdministration(CastleSiegePlugIn plugIn)
    {
        this._plugIn = plugIn;
    }

    /// <summary>
    /// Gets a stable snapshot of the Castle Siege runtime state.
    /// </summary>
    /// <param name="gameContext">The game context which hosts the Castle Siege event.</param>
    /// <returns>The snapshot, or <see langword="null"/> when the event has not initialized yet.</returns>
    public async ValueTask<CastleSiegeAdministrationSnapshot?> GetSnapshotAsync(IGameContext gameContext)
    {
        var context = this._plugIn.GetContext(gameContext);
        if (context is null)
        {
            return null;
        }

        CastleSiegeAdministrationSnapshot snapshot;
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!context.IsInitialized)
            {
                return null;
            }

            snapshot = new CastleSiegeAdministrationSnapshot(
                context.CurrentState,
                context.StateStartTimeUtc,
                context.StateEndTimeUtc,
                context.SiegeData.IsOccupied,
                context.SiegeData.OwnerGuildId,
                null,
                context.SiegeData.TaxChaos,
                context.SiegeData.TaxStore,
                context.SiegeData.TaxHunt,
                context.SiegeData.TributeMoney,
                context.RegisteredGuilds.Values
                    .OrderBy(registration => registration.RegistrationOrder)
                    .Select(registration => new CastleSiegeRegistrationSnapshot(
                        registration.GuildId,
                        registration.GuildName,
                        registration.Marks,
                        registration.RegistrationOrder))
                    .ToList(),
                CreateNpcSnapshots(context));
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        var ownerGuildName = await GetGuildNameAsync(context, snapshot.OwnerGuildId).ConfigureAwait(false);
        return snapshot with { OwnerGuildName = ownerGuildName };
    }

    /// <summary>
    /// Requests a state transition on the next Castle Siege timer tick.
    /// </summary>
    /// <param name="gameContext">The game context which hosts the Castle Siege event.</param>
    /// <param name="state">The state to enter.</param>
    /// <returns>The operation result.</returns>
    public CastleSiegeAdministrationResult ForceState(IGameContext gameContext, CastleSiegeState state)
    {
        if (!Enum.IsDefined(state))
        {
            return CastleSiegeAdministrationResult.Failed("The selected Castle Siege state is invalid.");
        }

        if (this._plugIn.GetContext(gameContext) is null)
        {
            return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
        }

        this._plugIn.ForceState(gameContext, state);
        return CastleSiegeAdministrationResult.Succeeded;
    }

    /// <summary>
    /// Assigns the castle to the guild with the specified name.
    /// </summary>
    /// <param name="gameContext">The game context which hosts the Castle Siege event.</param>
    /// <param name="guildName">The guild name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask<CastleSiegeAdministrationResult> SetOwnerAsync(IGameContext gameContext, string guildName)
    {
        if (string.IsNullOrWhiteSpace(guildName))
        {
            return CastleSiegeAdministrationResult.Failed("Enter a guild name.");
        }

        if (gameContext is not IGameServerContext gameServerContext)
        {
            return CastleSiegeAdministrationResult.Failed("Changing the owner requires a game-server context.");
        }

        // Resolve the name before taking the event lock. This works for offline guilds and avoids holding the
        // state-machine lock while the guild server performs its persistence lookup.
        var guildId = await gameServerContext.GuildServer
            .GetPersistentGuildIdByNameAsync(guildName.Trim())
            .ConfigureAwait(false);
        if (guildId is null)
        {
            return CastleSiegeAdministrationResult.Failed("The guild was not found.");
        }

        var context = this._plugIn.GetContext(gameContext);
        if (context is null)
        {
            return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
        }

        var ownershipChanged = false;
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!context.IsInitialized)
            {
                return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
            }

            if (context.CurrentState == CastleSiegeState.Start)
            {
                return CastleSiegeAdministrationResult.Failed("The owner cannot be changed while the siege battle is running.");
            }

            ownershipChanged = CastleSiegeCrownMechanics.ApplyOwner(context, guildId.Value);
            await context.SaveOwnerAsync().ConfigureAwait(false);
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        if (ownershipChanged)
        {
            await CastleSiegeEconomyNotifier.BroadcastTaxRatesAsync(context).ConfigureAwait(false);
        }

        await CastleSiegeCrownMechanics.BroadcastOwnershipChangeAsync(context, guildName.Trim()).ConfigureAwait(false);
        return CastleSiegeAdministrationResult.Succeeded;
    }

    /// <summary>
    /// Clears all registrations and selected guilds, then schedules the first idle state.
    /// </summary>
    /// <param name="gameContext">The game context which hosts the Castle Siege event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask<CastleSiegeAdministrationResult> ResetCycleAsync(IGameContext gameContext)
    {
        var context = this._plugIn.GetContext(gameContext);
        if (context is null)
        {
            return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!context.IsInitialized)
            {
                return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
            }

            if (context.CurrentState is CastleSiegeState.Ready or CastleSiegeState.Start or CastleSiegeState.End)
            {
                return CastleSiegeAdministrationResult.Failed("The Castle Siege cycle cannot be reset while siege preparation, battle, or result processing is active.");
            }

            await context.ClearRegistrationsAsync().ConfigureAwait(false);
            context.FinalGuildList.Clear();
            await context.SaveFinalGuildListAsync().ConfigureAwait(false);
            context.ClearPlayerJoinSides();
            context.ParticipantTracking.Clear();
            context.MiddleOwnerGuildId = null;
            context.CrownUser = null;
            context.PreviousCrownUser = null;
            Array.Clear(context.SwitchUsers);
            context.CrownAccumulatedTime = TimeSpan.Zero;
            context.IsCrownAvailable = false;
            context.LastCrownUpdateUtc = DateTime.MinValue;
            context.LastBroadcastSwitchInfos.Clear();
            context.LastBroadcastCrownAvailability = null;
            context.NextParticipantUpdateUtc = DateTime.MaxValue;
            await context.NpcController.DespawnAllAsync().ConfigureAwait(false);

            // ForceState is processed by the periodic task, which owns schedule calculation and state-enter notifications.
            this._plugIn.ForceState(gameContext, CastleSiegeState.Idle1);
            return CastleSiegeAdministrationResult.Succeeded;
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    /// <summary>
    /// Updates all Castle Siege tax settings.
    /// </summary>
    /// <param name="gameContext">The game context which hosts the Castle Siege event.</param>
    /// <param name="chaosTax">The Chaos Machine tax percentage.</param>
    /// <param name="storeTax">The personal-store tax percentage.</param>
    /// <param name="huntTax">The Land of Trials entrance fee.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask<CastleSiegeAdministrationResult> SetTaxesAsync(
        IGameContext gameContext,
        byte chaosTax,
        byte storeTax,
        int huntTax)
    {
        if (chaosTax > CastleSiegeTaxProvider.MaximumPercentageTax
            || storeTax > CastleSiegeTaxProvider.MaximumPercentageTax
            || huntTax is < 0 or > CastleSiegeTaxProvider.MaximumHuntTax)
        {
            return CastleSiegeAdministrationResult.Failed("One or more tax values are outside the supported range.");
        }

        var context = this._plugIn.GetContext(gameContext);
        if (context is null)
        {
            return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!context.IsInitialized)
            {
                return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
            }

            if (context.CurrentState == CastleSiegeState.Start)
            {
                return CastleSiegeAdministrationResult.Failed("Taxes cannot be changed while the siege battle is running.");
            }

            context.SiegeData.TaxChaos = chaosTax;
            context.SiegeData.TaxStore = storeTax;
            context.SiegeData.TaxHunt = huntTax;
            await context.SaveOwnerAsync().ConfigureAwait(false);
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        await CastleSiegeEconomyNotifier.BroadcastTaxRatesAsync(context).ConfigureAwait(false);
        return CastleSiegeAdministrationResult.Succeeded;
    }

    /// <summary>
    /// Clears the accumulated Castle Siege tribute.
    /// </summary>
    /// <param name="gameContext">The game context which hosts the Castle Siege event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask<CastleSiegeAdministrationResult> ClearTributeAsync(IGameContext gameContext)
    {
        var context = this._plugIn.GetContext(gameContext);
        if (context is null)
        {
            return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!context.IsInitialized)
            {
                return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
            }

            if (context.CurrentState == CastleSiegeState.Start)
            {
                return CastleSiegeAdministrationResult.Failed("Tribute cannot be cleared while the siege battle is running.");
            }

            context.SiegeData.TributeMoney = 0;
            await context.SaveOwnerAsync().ConfigureAwait(false);
            return CastleSiegeAdministrationResult.Succeeded;
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    /// <summary>
    /// Removes one guild registration.
    /// </summary>
    /// <param name="gameContext">The game context which hosts the Castle Siege event.</param>
    /// <param name="guildId">The persistent identifier of the registered guild.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask<CastleSiegeAdministrationResult> RemoveRegistrationAsync(IGameContext gameContext, Guid guildId)
    {
        var context = this._plugIn.GetContext(gameContext);
        if (context is null)
        {
            return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!context.IsInitialized)
            {
                return CastleSiegeAdministrationResult.Failed("Castle Siege has not initialized on this game server.");
            }

            if (context.CurrentState != CastleSiegeState.RegisterGuild)
            {
                return CastleSiegeAdministrationResult.Failed("Guild registrations can only be changed during the registration state.");
            }

            if (!context.RegisteredGuilds.TryGetValue(guildId, out var registration))
            {
                return CastleSiegeAdministrationResult.Failed("The registration no longer exists.");
            }

            await context.RemoveRegistrationAsync(registration).ConfigureAwait(false);
            return CastleSiegeAdministrationResult.Succeeded;
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    private static IReadOnlyList<CastleSiegeNpcAdministrationSnapshot> CreateNpcSnapshots(CastleSiegeContext context)
    {
        var runtimes = context.NpcController.GetRuntimeSnapshot()
            .ToDictionary(
                runtime => (runtime.Definition.MonsterDefinition?.Number, runtime.Definition.InstanceId),
                runtime => runtime);
        return context.Configuration.NpcDefinitions
            .OrderBy(definition => definition.MonsterDefinition?.Number)
            .ThenBy(definition => definition.InstanceId)
            .Select(definition => CreateNpcSnapshot(context, definition, runtimes.GetValueOrDefault((definition.MonsterDefinition?.Number, definition.InstanceId))))
            .ToList();
    }

    private static CastleSiegeNpcAdministrationSnapshot CreateNpcSnapshot(
        CastleSiegeContext context,
        CastleSiegeNpcDefinition definition,
        CastleSiegeNpcRuntime? runtime)
    {
        var state = runtime?.PersistedState;
        var attackable = runtime?.SpawnedInstance as CastleSiegeAttackableNpc;
        return new CastleSiegeNpcAdministrationSnapshot(
            definition.MonsterDefinition?.Number ?? 0,
            definition.InstanceId,
            state?.DefenseLevel ?? 0,
            state?.RegenLevel ?? 0,
            state?.LifeLevel ?? 0,
            attackable?.Health ?? state?.CurrentHp ?? 0,
            attackable?.MaximumHealth ?? (runtime is null ? 0 : context.NpcController.GetMaximumHealth(runtime)),
            runtime?.IsAlive ?? false,
            definition.IsPersistedToDatabase);
    }

    private static async ValueTask<string?> GetGuildNameAsync(CastleSiegeContext context, Guid? guildId)
    {
        if (guildId is null || context.GameContext is not IGameServerContext gameServerContext)
        {
            return null;
        }

        var runtimeGuildId = await gameServerContext.GuildServer.GetGuildIdAsync(guildId.Value).ConfigureAwait(false);
        return runtimeGuildId == 0
            ? null
            : (await gameServerContext.GuildServer.GetGuildAsync(runtimeGuildId).ConfigureAwait(false))?.Name;
    }
}
