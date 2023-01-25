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
[PlugIn("Ban Account command", "Handles the chat command '/banacc <acc>'. Bans an account from the game.")]
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
        if (string.IsNullOrEmpty(arguments.AccountName))
        {
            await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account name is required.").ConfigureAwait(false);
            return;
        }

        using var context = gameMaster.GameContext.PersistenceContextProvider.CreateNewPlayerContext(gameMaster.GameContext.Configuration);
        var account = await context.GetAccountByLoginNameAsync(arguments.AccountName ?? string.Empty).ConfigureAwait(false);

        if (account != null)
        {
            foreach (var character in account.Characters)
            {
                var player = gameMaster.GameContext.GetPlayerByCharacterName(character.Name ?? string.Empty);

                // disconect to change account
                if (player != null)
                {
                    await player.DisconnectAsync().ConfigureAwait(false);
                    break;
                }
            }

            account.State = AccountState.Banned;
            await context.SaveChangesAsync().ConfigureAwait(false);

            await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account {arguments.AccountName} has been banned.").ConfigureAwait(false);
        }
        else
        {
            await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account {arguments.AccountName} not found.\"").ConfigureAwait(false);
        }
    }
}