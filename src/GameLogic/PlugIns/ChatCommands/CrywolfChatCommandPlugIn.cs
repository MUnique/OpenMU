// <copyright file="CrywolfChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command for game masters, which lets the crywolf event proceed: it starts the event
/// when it's not running, and during the battle Balgass appears if he didn't yet. Otherwise, the current state ends.
/// </summary>
[Guid("2A7D4F95-8C31-4E6B-9A05-6F2E1B8D3C47")]
[PlugIn]
[Display(Name = nameof(PlugInResources.CrywolfChatCommandPlugIn_Name), Description = nameof(PlugInResources.CrywolfChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class CrywolfChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/crywolf";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        if (CrywolfPlugIn.GetContext(player.GameContext) is not { } context)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfNotActive)).ConfigureAwait(false);
            return;
        }

        context.SkipWaitingTime();
        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CrywolfProceeds), context.State).ConfigureAwait(false);
    }
}
