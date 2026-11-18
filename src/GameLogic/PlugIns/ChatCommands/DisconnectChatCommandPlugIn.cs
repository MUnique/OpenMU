// <copyright file="DisconnectChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles disconnect commands.
/// </summary>
[Guid("B5E0F108-9E55-48F6-A7A8-220BFAEF2F3E")]
[PlugIn]
[Display(Name = nameof(PlugInResources.DisconnectChatCommandPlugIn_Name), Description = nameof(PlugInResources.DisconnectChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(DisconnectChatCommandArgs), CharacterStatus.GameMaster)]
public class DisconnectChatCommandPlugIn : ChatCommandPlugInBase<DisconnectChatCommandArgs>
{
    private const string Command = "/disconnect";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, DisconnectChatCommandArgs arguments)
    {
        var targetPlayer = await this.GetPlayerByCharacterNameAsync(player, arguments.CharacterName ?? string.Empty).ConfigureAwait(false);
        if (targetPlayer is null)
        {
            return;
        }

        await targetPlayer.DisconnectAsync().ConfigureAwait(false);

        if (!targetPlayer.Name.Equals(player.Name))
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CommandResultPlayerDisconnected), this.Key, targetPlayer.Name).ConfigureAwait(false);
        }
    }
}