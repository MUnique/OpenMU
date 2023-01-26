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
[PlugIn("Unban Character command", "Handles the chat command '/unbanchar <char>'. Unbans the account of a character from the game.")]
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
        await this.ChangeAccountStateByCharacterNameAsync(gameMaster, arguments.CharacterName ?? string.Empty, AccountState.Normal).ConfigureAwait(false);

        await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] Account from {arguments.CharacterName} has been unbanned.").ConfigureAwait(false);
    }
}