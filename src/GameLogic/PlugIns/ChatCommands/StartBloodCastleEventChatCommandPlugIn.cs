// <copyright file="StartBloodCastleEventChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startds command.
/// If an event is already running, it is stopped (all players are removed from it) and
/// a new event starts with fresh instances.
/// </summary>
[Guid("7177533A-F147-407E-97B0-C4D8E1AC1AF4")]
[PlugIn]
[Display(Name = nameof(PlugInResources.StartBloodCastleEventChatCommandPlugIn_Name), Description = nameof(PlugInResources.StartBloodCastleEventChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class StartBloodCastleEventChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/startbc";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var bloodCastle = player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.BloodCastle);
        if (bloodCastle is null)
        {
            return;
        }

        var eventName = player.GameContext.Configuration.MiniGameDefinitions.FirstOrDefault(d => d.Type == MiniGameType.BloodCastle)?.Name
            ?? MiniGameType.BloodCastle.ToString();
        if (bloodCastle.IsEventActive(player.GameContext))
        {
            await bloodCastle.DisposeRunningGamesAsync(player.GameContext).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceRestartFormat), eventName).ConfigureAwait(false);
        }
        else
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MiniGameForceStartInitiatedFormat), eventName).ConfigureAwait(false);
        }

        bloodCastle.ForceStart();
    }
}