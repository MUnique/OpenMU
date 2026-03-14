// <copyright file="OfflineLevelingPlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A ghost player that continues leveling after the real client disconnects.
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
    /// Gets the login name of the account.
    /// </summary>
    public string? AccountLoginName => this.Account?.LoginName;

    /// <summary>
    /// Gets the character name.
    /// </summary>
    public string? CharacterName => this.SelectedCharacter?.Name;

    /// <summary>
    /// Gets the start timestamp of the offline leveling session.
    /// </summary>
    public DateTime StartTimestamp { get; private set; }

    /// <summary>
    /// Initializes the ghost player from captured references.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="character">The character.</param>
    /// <returns><c>true</c> if successfully started.</returns>
    public async ValueTask<bool> InitializeAsync(Account account, Character character)
    {
        try
        {
            this.StartTimestamp = DateTime.UtcNow;
            this.Account = account;
            this.PersistenceContext.Attach(account);

            // Advance state to allow the intelligence to perform actions.
            await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.LoginScreen).ConfigureAwait(false);
            await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.Authenticated).ConfigureAwait(false);
            await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.CharacterSelection).ConfigureAwait(false);

            // Add to context and set character.
            await this.GameContext.AddPlayerAsync(this).ConfigureAwait(false);
            await this.SetSelectedCharacterAsync(character).ConfigureAwait(false);

            if (this.SelectedCharacter is { } selectedCharacter)
            {
                this.PersistenceContext.Attach(selectedCharacter);
            }

            this._intelligence = new OfflineLevelingIntelligence(this);
            this._intelligence.Start();

            this.Logger.LogDebug(
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
    /// Stops the ghost player and removes it from the world.
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