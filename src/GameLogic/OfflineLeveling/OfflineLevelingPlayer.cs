// <copyright file="OfflineLevelingPlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A ghost player that stays in the world after the real client disconnects,
/// continuing to level up using the character's saved MU Helper configuration.
/// All view plugin calls are silently discarded — no network traffic is produced.
/// </summary>
public sealed class OfflineLevelingPlayer : Player
{
    private OfflineLevelingIntelligence? _intelligence;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineLevelingPlayer"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    public OfflineLevelingPlayer(IGameContext gameContext)
        : base(gameContext)
    {
    }

    /// <summary>
    /// Gets the login name of the account this offline player belongs to,
    /// used to match it when the real player logs back in.
    /// </summary>
    public string? AccountLoginName => this.Account?.LoginName;

    /// <summary>
    /// Gets the character name used to match on re-login.
    /// </summary>
    public string? CharacterName => this.SelectedCharacter?.Name;

    /// <summary>
    /// Initializes the offline leveling ghost from pre-captured account and character
    /// references. Must be called AFTER the real player has fully disconnected so that
    /// the name slot in <see cref="GameContext.PlayersByCharacterName"/> is free.
    /// </summary>
    /// <param name="account">The account the character belongs to.</param>
    /// <param name="character">The character to continue leveling.</param>
    /// <returns><c>true</c> when the ghost was successfully started.</returns>
    public async ValueTask<bool> InitializeAsync(Account account, Character character)
    {
        try
        {
            this.Account = account;

            // Attach the account (and all reachable entities including the character)
            // to the ghost's own persistence context so that SaveChangesAsync persists
            // XP, level-ups, and any other in-memory changes accumulated during offline leveling.
            this.PersistenceContext.Attach(account);

            // Must follow the full valid transition path: Initial → LoginScreen → Authenticated → CharacterSelection.
            // Skipping LoginScreen leaves the state at Initial, so TickAsync's EnteredWorld guard
            // blocks every tick and the ghost never attacks.
            await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.LoginScreen).ConfigureAwait(false);
            await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.Authenticated).ConfigureAwait(false);
            await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.CharacterSelection).ConfigureAwait(false);

            // AddPlayerAsync must come BEFORE SetSelectedCharacterAsync because it subscribes
            // the PlayerEnteredWorld event, which SetSelectedCharacterAsync fires.
            // Without this order, the ghost never gets added to PlayersByCharacterName.
            await this.GameContext.AddPlayerAsync(this).ConfigureAwait(false);

            // SetSelectedCharacterAsync → OnPlayerEnteredWorldAsync → ClientReadyAfterMapChangeAsync
            // which handles: setting CurrentMap, advancing state to EnteredWorld, and map.AddAsync.
            // We must NOT call those steps ourselves — doing so would add the ghost to the map twice.
            await this.SetSelectedCharacterAsync(character).ConfigureAwait(false);

            this._intelligence = new OfflineLevelingIntelligence(this);
            this._intelligence.Start();

            this.Logger.LogInformation(
                "Offline leveling started for character {CharacterName} on map {Map} at {Position}.",
                character.Name,
                character.CurrentMap?.Name,
                this.Position);

            return true;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to initialize offline leveling player.");
            return false;
        }
    }

    /// <summary>
    /// Stops the AI loop and removes the ghost from the world.
    /// Called when the real player logs back in.
    /// </summary>
    public async ValueTask StopAsync()
    {
        this._intelligence?.Dispose();
        this._intelligence = null;

        await this.DisconnectAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        => new NullViewPlugInContainer();
}