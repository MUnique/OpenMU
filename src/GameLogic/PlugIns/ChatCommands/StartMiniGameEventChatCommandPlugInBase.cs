// <copyright file="StartMiniGameEventChatCommandPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

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
        if (gameStarter.IsEventActive(player.GameContext))
        {
            await gameStarter.DisposeRunningGamesAsync(player.GameContext).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceRestartFormat), eventName).ConfigureAwait(false);
        }
        else
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceStartInitiatedFormat), eventName).ConfigureAwait(false);
        }

        gameStarter.ForceStart();
    }
}
