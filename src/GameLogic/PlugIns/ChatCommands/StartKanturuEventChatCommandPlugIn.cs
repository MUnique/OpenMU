// <copyright file="StartKanturuEventChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startkanturu command.
/// If an event is already running, it is stopped (all players are removed from it) and
/// a new event starts with fresh instances.
/// </summary>
[Guid("D20A5A0E-993D-48BA-882B-9E7D54B31529")]
[PlugIn]
[Display(Name = nameof(PlugInResources.StartKanturuEventChatCommandPlugIn_Name), Description = nameof(PlugInResources.StartKanturuEventChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class StartKanturuEventChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/startkanturu";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var kanturu = player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.Kanturu);
        if (kanturu is null)
        {
            return;
        }

        var eventName = player.GameContext.Configuration.MiniGameDefinitions.FirstOrDefault(d => d.Type == MiniGameType.Kanturu)?.Name
            ?? MiniGameType.Kanturu.ToString();
        if (kanturu.IsEventActive(player.GameContext))
        {
            await kanturu.DisposeRunningGamesAsync(player.GameContext).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceRestartFormat), eventName).ConfigureAwait(false);
        }
        else
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceStartInitiatedFormat), eventName).ConfigureAwait(false);
        }

        kanturu.ForceStart();
    }
}
