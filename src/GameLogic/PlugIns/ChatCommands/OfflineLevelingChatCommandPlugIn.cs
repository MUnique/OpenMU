// <copyright file="OfflineLevelingChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.OfflineLeveling;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the <c>/offlevel</c> chat command.
/// <list type="bullet">
///   <item>Logs the login-server entry off so the real player can re-connect at any time.</item>
///   <item>Disconnects the real client connection.</item>
///   <item>Creates a silent ghost player on the current map using the character's MU Helper config.</item>
///   <item>On next login the ghost is automatically stopped before character selection.</item>
/// </list>
/// </summary>
[Guid("A1C4E7F2-3B8D-4A09-8E5C-2D6F0B3A7E14")]
[PlugIn]
[Display(
    Name = nameof(PlugInResources.OfflineLevelingChatCommandPlugIn_Name),
    Description = nameof(PlugInResources.OfflineLevelingChatCommandPlugIn_Description),
    ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.Normal)]
public sealed class OfflineLevelingChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/offlevel";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        if (player.SelectedCharacter is null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.OfflineLevelingNoCharacterSelected)).ConfigureAwait(false);
            return;
        }

        if (!player.IsAlive)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.OfflineLevelingMustBeAlive)).ConfigureAwait(false);
            return;
        }

        if (player.CurrentMap is null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.OfflineLevelingNotOnMap)).ConfigureAwait(false);
            return;
        }

        if (player.Attributes?[Stats.IsMuHelperActive] <= 0)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.OfflineLevelingMuHelperNotRunning)).ConfigureAwait(false);
            return;
        }

        var loginName = player.Account?.LoginName;
        if (loginName is null)
        {
            return;
        }

        var manager = player.GameContext.OfflineLevelingManager;

        if (manager.IsActive(loginName))
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.OfflineLevelingAlreadyActive)).ConfigureAwait(false);
            return;
        }

        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.OfflineLevelingStarted)).ConfigureAwait(false);

        if (!await manager.StartAsync(player, loginName).ConfigureAwait(false))
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.OfflineLevelingFailed)).ConfigureAwait(false);
        }
    }
}