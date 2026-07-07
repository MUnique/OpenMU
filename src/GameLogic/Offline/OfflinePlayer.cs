// <copyright file="OfflinePlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An offline player that continues leveling after the real client disconnects.
/// </summary>
public sealed class OfflinePlayer : Player
{
    private OfflinePlayerMuHelper? _intelligence;
    private Task? _intelligenceDisposeTask;

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
    /// Initializes the offline player by loading the account fresh from the database.
    /// </summary>
    /// <param name="loginName">The account login name.</param>
    /// <param name="characterName">The character name to continue with.</param>
    /// <returns><c>true</c> if successfully started.</returns>
    public async ValueTask<bool> InitializeAsync(string loginName, string characterName)
    {
        try
        {
            this.StartTimestamp = DateTime.UtcNow;

            var account = await this.PersistenceContext.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
            if (account is null)
            {
                this.Logger.LogError("Failed to load account {LoginName} for offline session.", loginName);
                return false;
            }

            var character = account.Characters?.FirstOrDefault(c => c.Name == characterName);
            if (character is null)
            {
                this.Logger.LogError("Character {CharacterName} not found in account {LoginName}.", characterName, loginName);
                return false;
            }

            this.Account = account;

            await this.AdvanceToCharacterSelectionStateAsync().ConfigureAwait(false);

            await this.SetupCharacterAsync(character).ConfigureAwait(false);

            await this.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

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
            this.Logger.LogError(ex, "Failed to initialize offline player for {LoginName}.", loginName);
            return false;
        }
    }

    /// <summary>
    /// Stops the offline player and removes it from the world.
    /// </summary>
    public async ValueTask StopAsync()
    {
        await this.DisconnectAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask InternalDisconnectAsync()
    {
        if (this._intelligence is { } intelligence)
        {
            this._intelligence = null;
            this._intelligenceDisposeTask = Task.Run(async () =>
            {
                try
                {
                    await intelligence.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Error disposing intelligence for offline player {AccountLoginName}.", this.AccountLoginName);
                }
            });
        }

        await base.InternalDisconnectAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        if (this._intelligenceDisposeTask is { } disposeTask)
        {
            await disposeTask.ConfigureAwait(false);
        }

        await base.DisposeAsyncCore().ConfigureAwait(false);
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
        await this.GameContext.AddPlayerAsync(this).ConfigureAwait(false);
        await this.SetSelectedCharacterAsync(character).ConfigureAwait(false);
    }

    private void StartIntelligence()
    {
        this._intelligence = new OfflinePlayerMuHelper(this);
        this._intelligence.Start();
    }
}