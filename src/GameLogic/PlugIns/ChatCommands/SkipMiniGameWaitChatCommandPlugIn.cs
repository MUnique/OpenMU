// <copyright file="SkipMiniGameWaitChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the skipwait command.
/// It skips the currently running timed wait of the mini game event the game master is
/// currently participating in, e.g. the entering phase, the countdown, or an event-specific
/// standby time. The skip applies to everyone in the event: all participants are informed
/// about the skip, and the entrance announcements stop by themselves, because they are
/// derived from the live game state on every periodic tick.
/// Only the wait which is running right now is skipped. If the event is already running
/// and just waits for kills, there is nothing to skip (use /killall instead); the command
/// then reports that instead of arming a skip which would leak into a later phase and
/// brick the event.
/// </summary>
[Guid("97C963ED-7CDC-4D46-90BF-26F6E45B83FF")]
[PlugIn]
[Display(Name = nameof(PlugInResources.SkipMiniGameWaitChatCommandPlugIn_Name), Description = nameof(PlugInResources.SkipMiniGameWaitChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class SkipMiniGameWaitChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/skipwait";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        if (player.CurrentMiniGame is not MiniGameContext miniGame)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SkipWaitNotInMiniGame)).ConfigureAwait(false);
            return;
        }

        if (!miniGame.SkipCurrentWait())
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SkipWaitNothingToSkip)).ConfigureAwait(false);
            return;
        }

        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SkipWaitRequested)).ConfigureAwait(false);
        await miniGame.AnnounceSkipAsync(player.Name).ConfigureAwait(false);
    }
}
