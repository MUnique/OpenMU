// <copyright file="OfflineLevelingIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using System.Threading;

/// <summary>
/// Server-side AI that drives an <see cref="OfflineLevelingPlayer"/> after the real
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
public sealed class OfflineLevelingIntelligence : IDisposable
{
    private readonly OfflineLevelingPlayer _player;

    private readonly CombatHandler _combatHandler;
    private readonly BuffHandler _buffHandler;
    private readonly ItemPickupHandler _itemPickupHandler;
    private readonly MovementHandler _movementHandler;
    private readonly RepairHandler _repairHandler;
    private readonly ZenConsumptionHandler _zenHandler;
    private readonly HealingHandler _healingHandler;
    private readonly PetHandler _petHandler;

    private Timer? _aiTimer;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineLevelingIntelligence"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    public OfflineLevelingIntelligence(OfflineLevelingPlayer player)
    {
        this._player = player;
        var originalPosition = player.Position;
        var config = player.MuHelperSettings;

        this._buffHandler = new BuffHandler(player, config);
        this._healingHandler = new HealingHandler(player, config);
        this._itemPickupHandler = new ItemPickupHandler(player, config);
        this._movementHandler = new MovementHandler(player, config, originalPosition);
        this._combatHandler = new CombatHandler(player, config, this._movementHandler, this._buffHandler, originalPosition);
        this._repairHandler = new RepairHandler(player, config);
        this._zenHandler = new ZenConsumptionHandler(player);
        this._petHandler = new PetHandler(player, config);

        if (config is null)
        {
            this._player.Logger.LogDebug("Offline leveling started for {CharacterName} without a valid MU Helper configuration.", this._player.Name);
        }
        else
        {
            this._player.Logger.LogDebug("Offline leveling configuration for {CharacterName}: MuHelperSettings={Settings}.", this._player.Name, config);
        }
    }

    /// <summary>Starts the 500 ms AI timer.</summary>
    public void Start()
    {
        _ = this._petHandler.InitializeAsync();

        this._aiTimer ??= new Timer(
            state => _ = this.SafeTickAsync(),
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromMilliseconds(500));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        this._disposed = true;
        this._aiTimer?.Dispose();
        this._aiTimer = null;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100", Justification = "Timer callback — exceptions are caught internally.")]
    private async Task SafeTickAsync()
    {
        try
        {
            await this.TickAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(ex, "Error in offline leveling AI tick for {AccountLoginName}.", this._player.AccountLoginName);
        }
    }

    private async ValueTask TickAsync()
    {
        if (!this.CanPerformActions())
        {
            return;
        }

        await this._zenHandler.DeductZenAsync().ConfigureAwait(false);
        await this._repairHandler.PerformRepairsAsync().ConfigureAwait(false);
        await this._petHandler.CheckPetDurabilityAsync().ConfigureAwait(false);

        if (this.IsOnSkillCooldown())
        {
            return;
        }

        // CMuHelper::Work() order: Buff → RecoverHealth → ObtainItem → Regroup → Attack
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

    private bool CanPerformActions()
    {
        return this._player.IsAlive && this._player.PlayerState.CurrentState == PlayerState.EnteredWorld;
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