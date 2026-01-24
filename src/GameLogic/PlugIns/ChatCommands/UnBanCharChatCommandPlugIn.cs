// <copyright file="UnBanCharChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles unbanchar commands.
/// </summary>
[Guid("2830B01B-57A4-4925-AB6B-242C242B96C9")]
[PlugIn]
[Display(Name = nameof(PlugInResources.UnBanCharChatCommandPlugIn_Name), Description = nameof(PlugInResources.UnBanCharChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, "Unbans the account of a character from the game.", typeof(UnBanCharChatCommandArgs), CharacterStatus.GameMaster)]
public class UnBanCharChatCommandPlugIn : ChatCommandPlugInBase<BanCharChatCommandArgs>
{
    private const string Command = "/unbanchar";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, BanCharChatCommandArgs arguments)
    {
        if (!await this.TryChangeAccountStateByCharacterNameAsync(gameMaster, arguments.CharacterName ?? string.Empty, AccountState.Normal).ConfigureAwait(false))
        {
            return;
        }

        await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.UnbanAccountOfCharacterResult), this.Key, arguments.CharacterName).ConfigureAwait(false);
    }
}