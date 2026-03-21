// <copyright file="PetHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Pet;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Handles pet behavior initialization and management for the offline leveling player.
/// </summary>
internal sealed class PetHandler
{
    private readonly OfflineLevelingPlayer _player;
    private readonly IMuHelperSettings? _config;
    private readonly IPetCommandManager? _petCommandManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetHandler"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    /// <param name="petCommandManager">Optional pet command manager for testing.</param>
    public PetHandler(OfflineLevelingPlayer player, IMuHelperSettings? config, IPetCommandManager? petCommandManager = null)
    {
        this._player = player;
        this._config = config;
        this._petCommandManager = petCommandManager;
    }

    private IPetCommandManager? PetCommandManager => this._petCommandManager ?? this._player.PetCommandManager;

    /// <summary>
    /// Initializes the dark raven behavior if configured.
    /// The raven runs its own internal attack loop independently of the player's tick.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        try
        {
            await this.InitializeDarkRavenAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(ex, "Error initializing pet for character {CharacterName}.", this._player.Name);
        }
    }

    /// <summary>
    /// Checks the pet durability and sets the pet to idle if it has run out.
    /// </summary>
    public async ValueTask CheckPetDurabilityAsync()
    {
        if (this.PetCommandManager is null)
        {
            return;
        }

        try
        {
            if (this._player.Inventory?.GetItem(InventoryConstants.PetSlot) is { Durability: 0 })
            {
                await this.PetCommandManager.SetBehaviourAsync(PetBehaviour.Idle, null).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(ex, "Error checking pet durability for {AccountLoginName}.", this._player.AccountLoginName);
        }
    }

    private async ValueTask InitializeDarkRavenAsync()
    {
        if (this._config is not { UseDarkRaven: true } || this.PetCommandManager is not { } petCommandManager)
        {
            return;
        }

        var behaviour = this._config.DarkRavenMode switch
        {
            1 => PetBehaviour.AttackRandom,
            2 => PetBehaviour.AttackWithOwner,
            _ => PetBehaviour.Idle,
        };

        await petCommandManager.SetBehaviourAsync(behaviour, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Stops the pet behavior.
    /// </summary>
    public async ValueTask StopAsync()
    {
        if (this.PetCommandManager is { } petCommandManager)
        {
            try
            {
                await petCommandManager.SetBehaviourAsync(PetBehaviour.Idle, null).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._player.Logger.LogError(ex, "Error stopping pet for {AccountLoginName}.", this._player.AccountLoginName);
            }
        }
    }


}