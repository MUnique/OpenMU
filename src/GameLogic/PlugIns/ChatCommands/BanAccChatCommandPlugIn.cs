// <copyright file="BanAccChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles banacc commands.
/// </summary>
[Guid("EF869270-847E-48D5-9012-F5D111D9C8EB")]
[PlugIn]
[Display(Name = nameof(PlugInResources.BanAccChatCommandPlugIn_Name), Description = nameof(PlugInResources.BanAccChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, "Bans an account from the game.", typeof(BanAccChatCommandArgs), CharacterStatus.GameMaster)]
public class BanAccChatCommandPlugIn : ChatCommandPlugInBase<BanAccChatCommandArgs>
{
    private const string Command = "/banacc";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, BanAccChatCommandArgs arguments)
    {
        await this.ChangeAccountStateByLoginNameAsync(gameMaster, arguments.AccountName ?? string.Empty, AccountState.Banned).ConfigureAwait(false);

        await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.AccountHasBeenBanned), this.Key, arguments.AccountName).ConfigureAwait(false);
    }
}