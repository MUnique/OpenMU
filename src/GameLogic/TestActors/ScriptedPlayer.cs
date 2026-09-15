// <copyright file="ScriptedPlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A scripted actor: a connection-less player which enters the world as an existing account's
/// character and then does what a scenario tells it to.
/// </summary>
/// <remarks>
/// It derives from <see cref="Player"/> and NOT from
/// <see cref="MUnique.OpenMU.GameLogic.Offline.OfflinePlayer"/> on purpose: the bot code treats
/// offline players as one of its own (<c>BotSelfDefensePlugIn</c> only registers an aggressor which
/// is <c>not OfflinePlayer</c>, the mini-game handler skips offline party leaders, and the admin
/// panel lists them as offline accounts). An actor has to be a human stand-in, so it repeats the
/// ~15 lines of the offline login sequence instead of inheriting them.
/// </remarks>
public class ScriptedPlayer : Player
{
    private readonly ActorEventLog _eventLog = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptedPlayer"/> class.
    /// </summary>
    /// <param name="gameContext">The game context of the game server this actor plays on.</param>
    public ScriptedPlayer(IGameContext gameContext)
        : base(gameContext)
    {
    }

    /// <summary>
    /// Gets the event stream of this actor: everything the game would have shown to its client.
    /// </summary>
    public ActorEventLog EventLog => this._eventLog;

    /// <summary>
    /// Gets the login name of the account this actor animates.
    /// </summary>
    public string? AccountLoginName { get; private set; }

    /// <summary>
    /// Gets the intelligence which executes the actor's commands, once it entered the world.
    /// </summary>
    public ScriptedIntelligence? Intelligence { get; private set; }

    /// <summary>
    /// Logs the account's character in and enters the world, the way an offline player does it.
    /// </summary>
    /// <param name="loginName">The login name of an existing account.</param>
    /// <param name="characterSlot">The character slot to animate; <c>null</c> takes the lowest slot.</param>
    /// <returns><c>true</c> if the actor entered the world.</returns>
    public async ValueTask<bool> InitializeAsync(string loginName, byte? characterSlot = null)
    {
        try
        {
            // The actor's own persistence context, like a bot's: no entity of another player's
            // context is ever attached here.
            var account = await this.PersistenceContext.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
            if (account is null)
            {
                this.Logger.LogError("Actor account {LoginName} could not be loaded.", loginName);
                return false;
            }

            var character = characterSlot is { } slot
                ? account.Characters.FirstOrDefault(c => c.CharacterSlot == slot)
                : account.Characters.OrderBy(c => c.CharacterSlot).FirstOrDefault();
            if (character is null)
            {
                this.Logger.LogError("Actor account {LoginName} has no character in slot {Slot}.", loginName, characterSlot);
                return false;
            }

            return await this.EnterWorldAsync(account, character).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to initialize the actor for account {LoginName}.", loginName);
            return false;
        }
    }

    /// <summary>
    /// Enters the world with an already loaded account and character. This is the login sequence
    /// itself, split out so it can be driven with a prepared account (tests) as well as with one
    /// loaded from the database.
    /// </summary>
    /// <param name="account">The account to animate.</param>
    /// <param name="character">The character of that account to animate.</param>
    /// <returns><c>true</c> if the actor entered the world.</returns>
    public async ValueTask<bool> EnterWorldAsync(Account account, Character character)
    {
        this.Account = account;
        this.AccountLoginName = account.LoginName;

        await this.PlayerState.TryAdvanceToAsync(MUnique.OpenMU.GameLogic.PlayerState.LoginScreen).ConfigureAwait(false);
        await this.PlayerState.TryAdvanceToAsync(MUnique.OpenMU.GameLogic.PlayerState.Authenticated).ConfigureAwait(false);
        await this.PlayerState.TryAdvanceToAsync(MUnique.OpenMU.GameLogic.PlayerState.CharacterSelection).ConfigureAwait(false);

        await this.GameContext.AddPlayerAsync(this).ConfigureAwait(false);

        // Selecting the character is what enters the world: it runs OnPlayerEnteredWorldAsync,
        // which calls ClientReadyAfterMapChangeAsync itself. Calling it again (as the offline
        // player does) only earns an "already on map" warning in the log.
        await this.SetSelectedCharacterAsync(character).ConfigureAwait(false);

        if (this.PlayerState.CurrentState != MUnique.OpenMU.GameLogic.PlayerState.EnteredWorld)
        {
            this.Logger.LogError(
                "Actor {LoginName} did not enter the world; it is in state {State}.",
                this.AccountLoginName,
                this.PlayerState.CurrentState.Name);
            return false;
        }

        this.Intelligence = new ScriptedIntelligence(this);
        this.Intelligence.Start();

        this._eventLog.Append(
            "spawned",
            new ActorEventField("actor", this.AccountLoginName ?? string.Empty),
            new ActorEventField("character", this.Name),
            new ActorEventField("map", this.CurrentMap?.Definition.Name.ToString() ?? string.Empty),
            new ActorEventField("x", this.Position.X),
            new ActorEventField("y", this.Position.Y));

        this.Logger.LogInformation(
            "Actor {LoginName} entered the world as {Character} on {Map} at {Position}.",
            this.AccountLoginName,
            this.Name,
            this.CurrentMap?.Definition.Name,
            this.Position);

        return true;
    }

    /// <summary>
    /// Stops the actor: the normal logout path, which saves the character's progress and releases
    /// the account.
    /// </summary>
    /// <returns>The task.</returns>
    public async ValueTask StopAsync()
    {
        await this.DisconnectAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        => new RecordingViewPlugInContainer(this);

    /// <inheritdoc />
    protected override async ValueTask InternalDisconnectAsync()
    {
        if (this.Intelligence is { } intelligence)
        {
            this.Intelligence = null;
            await intelligence.DisposeAsync().ConfigureAwait(false);
        }

        await base.InternalDisconnectAsync().ConfigureAwait(false);
    }
}
