// <copyright file="OfflineLevelingPlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;
using System.Threading.Tasks;

/// <summary>
/// An offline player that continues leveling after the real client disconnects.
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
    /// Gets the start timestamp of the offline leveling session.
    /// </summary>
    public DateTime StartTimestamp { get; private set; }

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

            this.StartIntelligence();

            this.Logger.LogDebug(
                "Offline leveling started for character {CharacterName} on map {Map} at {Position}.",
                character.Name,
                character.CurrentMap?.Name,
                this.Position);

            return true;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to initialize offline player for {Name}.", this.Name);
            return false;
        }
    }

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

    private void StartIntelligence()
    {
        this._intelligence = new OfflineLevelingIntelligence(this);
        this._intelligence.Start();
    }

    /// <summary>
    /// Stops the offline player and removes it from the world.
    /// </summary>
    public async ValueTask StopAsync()
    {
        this._intelligence?.Dispose();
        this._intelligence = null;
        try
        {
            await this.SaveProgressAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to save progress of offline leveling player {Name}.", this.Name);
        }

        await this.DisconnectAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        => new OfflineViewPlugInContainer(this);


}