// <copyright file="OfflinePlayerMuHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using System.Threading;

/// <summary>
/// Server-side intelligence that drives an <see cref="OfflinePlayer"/> after the real
/// client disconnects. Mirrors the C++ <c>CMuHelper::Work()</c> loop including:
/// <list type="bullet">
///   <item>Basic / conditional / combo skill attack selection</item>
///   <item>Buff application (up to 3 configured buff skills)</item>
///   <item>Heal / drain-life based on HP %</item>
///   <item>Return-to-origin regrouping</item>
///   <item>Item pickup (Zen, Jewels, Excellent, Ancient, and named extra items)</item>
///   <item>Skill and movement animations broadcast to nearby observers</item>
///   <item>Pet control</item>
/// </list>
/// </summary>
public sealed class OfflinePlayerMuHelper : AsyncDisposable
{
    private readonly OfflinePlayer _player;

    private readonly CombatHandler _combatHandler;
    private readonly BuffHandler _buffHandler;
    private readonly ItemPickupHandler _itemPickupHandler;
    private readonly MovementHandler _movementHandler;
    private readonly RepairHandler _repairHandler;
    private readonly ZenConsumptionHandler _zenHandler;
    private readonly HealingHandler _healingHandler;
    private readonly PetHandler _petHandler;
    private readonly CancellationTokenSource _cts = new();
    private readonly EventHandler<DeathInformation> _deathHandler;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(500));

    private Task? _loopTask;

    private bool _isDead;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflinePlayerMuHelper"/> class.
    /// </summary>
    /// <param name="player">The offline player.</param>
    public OfflinePlayerMuHelper(OfflinePlayer player)
    {
        this._player = player;
        var config = player.MuHelperSettings;

        this._buffHandler = new BuffHandler(player, config);
        this._healingHandler = new HealingHandler(player, config);
        this._itemPickupHandler = new ItemPickupHandler(player, config);
        this._movementHandler = new MovementHandler(player, config);
        this._combatHandler = new CombatHandler(player, config, this._movementHandler);
        this._repairHandler = new RepairHandler(player, config);
        this._zenHandler = new ZenConsumptionHandler(player);
        this._petHandler = new PetHandler(player, config);

        if (config is null)
        {
            this._player.Logger.LogDebug("Offline player started for {CharacterName} without a valid MU Helper configuration.", this._player.Name);
        }
        else
        {
            this._player.Logger.LogDebug("Offline player configuration for {CharacterName}: MuHelperSettings={Settings}.", this._player.Name, config);
        }

        this._deathHandler = (_, e) => this.OnPlayerDied(e);
        this._player.Died += this._deathHandler;
    }

    /// <summary>Starts the AI loop and a separate pet AI.</summary>
    public void Start()
    {
        _ = this._petHandler.InitializeAsync();
        this._loopTask = this.RunLoopAsync(this._cts.Token);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        this._timer.Dispose();
        await this._cts.CancelAsync().ConfigureAwait(false);

        if (this._loopTask is { } loopTask)
        {
            try
            {
                await loopTask.ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException)
            {
                // Expected during shutdown.
            }
            catch (Exception ex)
            {
                this._player.Logger.LogError(ex, "Error in offline player helper loop task for {AccountLoginName}.", this._player.AccountLoginName);
            }
        }

        await this._petHandler.StopAsync().ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._player.Died -= this._deathHandler;
            this._cts.Dispose();
        }

        base.Dispose(disposing);
    }

    private void OnPlayerDied(DeathInformation e)
    {
        this._player.Logger.LogDebug("Offline player '{Name}' died. Killer: {KillerName}.", this._player.Name, e.KillerName);
        this._isDead = true;

        // Do not cancel the loop here: a bot needs to keep ticking so it can resume after respawning.
        // For a normal offline session the tick stops the session on respawn, which disposes (and cancels) this helper.
    }

    private async Task RunLoopAsync(CancellationToken cancellationToken)
    {
        // Randomize the loop phase, so hundreds of concurrently started players don't all tick on the
        // same 500ms boundary - smoother server load and less robotic synchrony between them.
        await Task.Delay(Rand.NextInt(0, 500), cancellationToken).ConfigureAwait(false);

        while (await this._timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await this.SafeTickAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task SafeTickAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.TickAsync(cancellationToken).ConfigureAwait(false);
            this._player.OnAiTickSucceeded();
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(ex, "Error in offline player helper tick for {AccountLoginName}.", this._player.AccountLoginName);
            this._player.OnAiTickFailed();
        }
    }

    private async ValueTask TickAsync(CancellationToken cancellationToken)
    {
        // Actions queued from outside the tick (e.g. skill learning on level-up) run here, serialized
        // with the combat handler - so nothing mutates the skill list while combat is enumerating it.
        await this._player.DrainPendingBotActionsAsync().ConfigureAwait(false);

        if (await this.HandleDeathAsync().ConfigureAwait(false))
        {
            return;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (this._player.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            return;
        }

        if (!await this._zenHandler.DeductZenAsync().ConfigureAwait(false))
        {
            if (this._player.Account?.LoginName is { } loginName)
            {
                await this._player.GameContext.OfflinePlayerManager.StopAsync(loginName).ConfigureAwait(false);
            }

            return;
        }

        await this._repairHandler.PerformRepairsAsync().ConfigureAwait(false);
        await this._petHandler.CheckPetDurabilityAsync().ConfigureAwait(false);

        if (this.IsOnSkillCooldown())
        {
            return;
        }

        // CMuHelper::Work() order: Buff → RecoverHealth → ObtainItem → Regroup → Attack.
        if (!await this._buffHandler.PerformBuffsAsync().ConfigureAwait(false))
        {
            return;
        }

        await this._healingHandler.PerformHealthRecoveryAsync().ConfigureAwait(false);
        await this._combatHandler.PerformDrainLifeRecoveryAsync().ConfigureAwait(false);
        await this._itemPickupHandler.PickupItemsAsync().ConfigureAwait(false);

        if (this._player.IsWalking)
        {
            return;
        }

        if (!await this._movementHandler.RegroupAsync().ConfigureAwait(false))
        {
            return;
        }

        await this._combatHandler.PerformAttackAsync().ConfigureAwait(false);
    }

    private async ValueTask<bool> HandleDeathAsync()
    {
        if (this._isDead)
        {
            if (!this._player.IsAlive)
            {
                // Player hasn't respawned yet, skip the tick and check again on the next interval.
                return true;
            }

            if (this._player.RespawnAndContinue)
            {
                // Bots keep playing: reset the death state and re-anchor the hunting origin to the respawn
                // position so the navigator picks a fresh hunting ground from where the bot came back to life.
                // A death by a player's hand may have left a pending revenge - arm it now (the navigator
                // then marches the bot back to its death site instead of picking a hunting ground).
                this._isDead = false;
                this._player.HuntingOrigin = this._player.Position;
                this._player.ArmRevengeAfterRespawn();
                this._player.Logger.LogInformation("Bot '{Name}' respawned; resuming.", this._player.Name);
                return false;
            }

            if (this._player.Account?.LoginName is { } loginName)
            {
                this._player.Logger.LogInformation("Offline player died and successfully respawned. Stopping session for {0}.", loginName);
                await this._player.GameContext.OfflinePlayerManager.StopAsync(loginName).ConfigureAwait(false);
            }

            return true;
        }

        return false;
    }

    private bool IsOnSkillCooldown()
    {
        if (this._combatHandler.SkillCooldownTicks > 0)
        {
            this._combatHandler.DecrementCooldown();
            return true;
        }

        return false;
    }
}
