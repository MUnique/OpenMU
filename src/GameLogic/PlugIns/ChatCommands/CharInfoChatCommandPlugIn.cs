// <copyright file="CharInfoChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.Persistence.BasicModel;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles charinfo commands.
/// </summary>
[Guid("0C7162BC-C74E-4A65-82E3-12811E4BE170")]
[PlugIn("Char Info command", "Handles the chat command '/charinfo <char>'. Returns information about the character back to the requester.")]
[ChatCommandHelp(Command, "Returns information about the character back to the requester.", typeof(CharInfoChatCommandArgs), CharacterStatus.GameMaster)]
public class CharInfoChatCommandPlugIn : ChatCommandPlugInBase<CharInfoChatCommandArgs>
{
    private const string Command = "/charinfo";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, CharInfoChatCommandArgs arguments)
    {
        var player = this.GetPlayerByCharacterName(gameMaster, arguments.CharacterName ?? string.Empty);
        var account = player.Account;

        if (account == null)
        {
            return;
        }

        await this.ShowMessageToAsync(gameMaster, $"Account Name: {account.LoginName}").ConfigureAwait(false);

        await this.ShowAllLinesMessageToAsync(gameMaster, player.SelectedCharacter?.ToString()).ConfigureAwait(false);

        await this.ShowAllLinesMessageToAsync(gameMaster, player?.Attributes?.ToString()).ConfigureAwait(false);
    }

    private async ValueTask ShowAllLinesMessageToAsync(Player gameMaster, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        using var reader = new StringReader(message);

        while (true)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            if (line == null)
            {
                break;
            }

            await this.ShowMessageToAsync(gameMaster, line).ConfigureAwait(false);
        }
    }
}