// <copyright file="OfflineLevelingIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views.MuHelper;

/// <summary>
/// Server-side AI that drives an <see cref="OfflineLevelingPlayer"/> ghost after the real
/// client disconnects. Mirrors the C++ <c>CMuHelper::Work()</c> loop including:
/// <list type="bullet">
///   <item>Basic / conditional / combo skill attack selection</item>
///   <item>Self-buff application (up to 3 configured buff skills)</item>
///   <item>Auto-heal / drain-life based on HP %</item>
///   <item>Return-to-origin regrouping</item>
///   <item>Item pickup (Zen, Jewels, Excellent, Ancient, and named extra items)</item>
///   <item>Skill and movement animations broadcast to nearby observers</item>
/// </list>
/// Party support is not implemented.
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

    private Timer? _aiTimer;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineLevelingIntelligence"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    public OfflineLevelingIntelligence(OfflineLevelingPlayer player)
    {
        this._player = player;
        var config = MuHelperPlayerConfiguration.TryDeserialize(player.SelectedCharacter?.MuHelperConfiguration);
        var originalPosition = player.Position;

        this._buffHandler = new BuffHandler(player, config);
        this._itemPickupHandler = new ItemPickupHandler(player, config);
        this._movementHandler = new MovementHandler(player, config, originalPosition, () => CombatHandler.CalculateHuntingRange(config));
        this._combatHandler = new CombatHandler(player, config, this._movementHandler, originalPosition);
        this._repairHandler = new RepairHandler(player, config);
        this._zenHandler = new ZenConsumptionHandler(player);

        if (config is null)
        {
            player.Logger.LogDebug("Offline leveling started for {0} without a valid MU Helper configuration.", player.Name);
        }
        else
        {
            player.Logger.LogDebug("Offline leveling configuration for {0}: RepairItem={1}, Zen={2}, AutoHeal={3}.", player.Name, config.RepairItem, config.PickZen, config.AutoHeal);
        }
    }

    /// <summary>Starts the 500 ms AI timer.</summary>
    public void Start()
    {
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Timer callback — exceptions are caught internally.")]
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
            this._player.Logger.LogError(ex, "Error in offline leveling AI tick for {Name}.", this._player.CharacterName);
        }
    }

    private async ValueTask TickAsync()
    {
        if (!this._player.IsAlive || this._player.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            return;
        }

        await this._zenHandler.DeductZenAsync().ConfigureAwait(false);

        await this._repairHandler.PerformRepairsAsync().ConfigureAwait(false);



        if (this._combatHandler.SkillCooldownTicks > 0)
        {
            this._combatHandler.DecrementCooldown();
            return;
        }

        // CMuHelper::Work() order: Buff → RecoverHealth → ObtainItem → Regroup → Attack
        if (!await this._buffHandler.PerformBuffsAsync().ConfigureAwait(false))
        {
            return;
        }

        await this._combatHandler.PerformHealthRecoveryAsync().ConfigureAwait(false);

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
}
