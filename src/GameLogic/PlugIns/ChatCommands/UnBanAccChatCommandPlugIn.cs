// <copyright file="UnBanAccChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles unbanacc commands.
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
        if (string.IsNullOrEmpty(arguments.AccountName))
        {
            await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account name is required.").ConfigureAwait(false);
            return;
        }

        using var context = gameMaster.GameContext.PersistenceContextProvider.CreateNewPlayerContext(gameMaster.GameContext.Configuration);
        var account = await context.GetAccountByLoginNameAsync(arguments.AccountName ?? string.Empty).ConfigureAwait(false);

        if (account != null)
        {
            account.State = AccountState.Normal;
            await context.SaveChangesAsync().ConfigureAwait(false);

            await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account {arguments.AccountName} has been unbanned.").ConfigureAwait(false);
        }
        else
        {
            await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account {arguments.AccountName} not found.\"").ConfigureAwait(false);
        }
    }
}