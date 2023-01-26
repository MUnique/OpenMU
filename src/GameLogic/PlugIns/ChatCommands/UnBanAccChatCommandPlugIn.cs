// <copyright file="UnBanAccChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles banacc commands.
/// </summary>
[Guid("FCBC9CC0-3C8F-45E2-96DF-9C55BE30C5D9")]
[PlugIn("Unban Account command", "Handles the chat command '/unbanacc <acc>'. Unbans an account from the game.")]
[ChatCommandHelp(Command, "Unbans an account from the game.", typeof(UnBanAccChatCommandArgs), CharacterStatus.GameMaster)]
public class UnBanAccChatCommandPlugIn : ChatCommandPlugInBase<UnBanAccChatCommandArgs>
{
    private const string Command = "/unbanacc";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, UnBanAccChatCommandArgs arguments)
    {
        await this.ChangeAccountStateByLoginNameAsync(gameMaster, arguments.AccountName ?? string.Empty, AccountState.Normal).ConfigureAwait(false);

        await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account {arguments.AccountName} has been unbanned.").ConfigureAwait(false);
    }
}