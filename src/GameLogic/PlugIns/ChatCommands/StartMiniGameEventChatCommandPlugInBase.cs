// <copyright file="StartMiniGameEventChatCommandPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Base class for the game master chat commands which (re-)start a mini game event.
/// If an event is already running, it is stopped (all players are removed from it) and
/// a new event starts with fresh instances.
/// </summary>
public abstract class StartMiniGameEventChatCommandPlugInBase : IChatCommandPlugIn
{
    /// <inheritdoc />
    public abstract string Key { get; }

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <summary>
    /// Gets the type of the mini game which is started by this command.
    /// </summary>
    protected abstract MiniGameType MiniGameType { get; }

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var gameStarter = player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(this.MiniGameType);
        if (gameStarter is null)
        {
            return;
        }

        var eventName = player.GameContext.Configuration.MiniGameDefinitions.FirstOrDefault(d => d.Type == this.MiniGameType)?.Name
            ?? this.MiniGameType.ToString();
        if (IsDisabledOnServer(player.GameContext, gameStarter))
        {
            player.Logger.LogInformation("Game master {player} tried to force-start {event} on a game server where it is disabled.", player, eventName);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceStartUnavailableFormat), eventName).ConfigureAwait(false);
            return;
        }

        if (gameStarter.IsEventActive(player.GameContext))
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceRestartFormat), eventName).ConfigureAwait(false);
        }
        else
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceStartInitiatedFormat), eventName).ConfigureAwait(false);
        }

        // Always dispose first, even without a running game: it's a no-op then, but
        // clears stale state which would block the forced start.
        await gameStarter.DisposeRunningGamesAsync(player.GameContext).ConfigureAwait(false);

        gameStarter.ForceStart();
    }

    /// <summary>
    /// Determines whether the event is disabled on the game master's server.
    /// A disabled event is never force-started: the force would be consumed without
    /// starting anything, while the disposal above would still have removed open
    /// games and the game master would be told the event starts here.
    /// </summary>
    /// <param name="gameContext">The game context of the game master.</param>
    /// <param name="gameStarter">The start plug-in of the event.</param>
    /// <returns><c>true</c> if the event doesn't run on the server; otherwise, <c>false</c>.</returns>
    internal static bool IsDisabledOnServer(IGameContext gameContext, IPeriodicMiniGameStartPlugIn gameStarter)
    {
        // Goes through the plug-in instead of reading its configuration: the concrete
        // configuration type varies per event, and the configuration interface is
        // invariant, so matching a specific configuration type never fires.
        return !gameStarter.IsEnabledOn(gameContext);
    }
}
