// <copyright file="OfflinePlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An offline player that continues leveling after the real client disconnects.
/// </summary>
public class OfflinePlayer : Player
{
    private OfflinePlayerMuHelper? _intelligence;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflinePlayer"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    public OfflinePlayer(IGameContext gameContext)
        : base(gameContext)
    {
    }

    /// <summary>
    /// Gets the login name this offline player belongs to.
    /// </summary>
    public string? AccountLoginName => this.Account?.LoginName;

    /// <summary>
    /// Gets the start timestamp of the offline session.
    /// </summary>
    public DateTime StartTimestamp { get; internal set; }

    /// <summary>
    /// Gets or sets the position the intelligence hunts around. For a plain offline player this is
    /// the spawn position and never changes. Bots update it to roam between hunting grounds.
    /// </summary>
    public Point HuntingOrigin { get; set; }

    /// <summary>
    /// Gets a value indicating whether the player should keep playing after dying and respawning.
    /// A normal offline session ends on death; bots override this to keep running forever.
    /// </summary>
    public virtual bool RespawnAndContinue => false;

    /// <summary>
    /// Initializes the offline player from captured references.
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

            await this.AdvanceToCharacterSelectionStateAsync().ConfigureAwait(false);

            await this.SetupCharacterAsync(character).ConfigureAwait(false);

            await this.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

            this.HuntingOrigin = this.Position;

            this.StartIntelligence();

            this.Logger.LogDebug(
                "Offline player started for character {CharacterName} on map {Map} at {Position}.",
                character.Name,
                character.CurrentMap?.Name,
                this.Position);

            return true;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to initialize offline player for {player}.", this);
            return false;
        }
    }

    /// <summary>
    /// Stops the offline player and removes it from the world.
    /// </summary>
    public virtual async ValueTask StopAsync()
    {
        if (this._intelligence is { } intelligence)
        {
            await intelligence.DisposeAsync().ConfigureAwait(false);
            this._intelligence = null;
        }

        try
        {
            await this.SaveProgressAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to save progress of offline player {AccountLoginName}.", this.AccountLoginName);
        }

        await this.DisconnectAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        => new OfflineViewPlugInContainer(this);

    private async ValueTask AdvanceToCharacterSelectionStateAsync()
    {
        // Advance state to allow the intelligence to perform actions.
        await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.LoginScreen).ConfigureAwait(false);
        await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.Authenticated).ConfigureAwait(false);
        await this.PlayerState.TryAdvanceToAsync(GameLogic.PlayerState.CharacterSelection).ConfigureAwait(false);
    }

    private async ValueTask SetupCharacterAsync(Character character)
    {
        // Add to context and set character.
        await this.GameContext.AddPlayerAsync(this).ConfigureAwait(false);
        await this.SetSelectedCharacterAsync(character).ConfigureAwait(false);

        if (this.SelectedCharacter is { } selectedCharacter)
        {
            this.PersistenceContext.Attach(selectedCharacter);
        }
    }

    /// <summary>
    /// Starts the intelligence which drives this offline player. Overridden by bots to also run navigation.
    /// </summary>
    protected virtual void StartIntelligence()
    {
        this._intelligence = new OfflinePlayerMuHelper(this);
        this._intelligence.Start();
    }
}