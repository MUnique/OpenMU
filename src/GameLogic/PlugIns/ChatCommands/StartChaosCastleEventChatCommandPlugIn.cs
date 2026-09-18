// <copyright file="StartChaosCastleEventChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startcc command.
/// If an event is already running, it is stopped (all players are removed from it) and
/// a new event starts with fresh instances.
/// </summary>
[Guid("A990270E-B9C6-4445-BBA9-56367A90D31D")]
[PlugIn]
[Display(Name = nameof(PlugInResources.StartChaosCastleEventChatCommandPlugIn_Name), Description = nameof(PlugInResources.StartChaosCastleEventChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class StartChaosCastleEventChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/startcc";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var chaosCastle = player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.ChaosCastle);
        if (chaosCastle is null)
        {
            return;
        }

        var eventName = player.GameContext.Configuration.MiniGameDefinitions.FirstOrDefault(d => d.Type == MiniGameType.ChaosCastle)?.Name
            ?? MiniGameType.ChaosCastle.ToString();
        if (chaosCastle.IsEventActive(player.GameContext))
        {
            await chaosCastle.DisposeRunningGamesAsync(player.GameContext).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceRestartFormat), eventName).ConfigureAwait(false);
        }
        else
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceStartInitiatedFormat), eventName).ConfigureAwait(false);
        }

        chaosCastle.ForceStart();
    }
}
