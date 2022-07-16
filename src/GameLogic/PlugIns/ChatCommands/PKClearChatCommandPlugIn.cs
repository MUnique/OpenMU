// <copyright file="PKClearChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles pk clear commands.
/// </summary>
[Guid("EB97A8F6-F6BD-460A-BCBE-253BF679361A")]
[PlugIn("PK clear chat command", "Handles the chat command '/pkclear <char>'. Clears the player kill count.")]
[ChatCommandHelp(Command, "Clears the player kill count.", typeof(PkClearChatCommandArgs), CharacterStatus.GameMaster)]
public class PkClearChatCommandPlugIn : ChatCommandPlugInBase<PkClearChatCommandArgs>
{
    private const string Command = "/pkclear";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, PkClearChatCommandArgs arguments)
    {
        var targetPlayer = this.GetPlayerByCharacterName(gameMaster, arguments.CharacterName ?? string.Empty);

        targetPlayer.SelectedCharacter!.State = HeroState.Normal;
        targetPlayer.SelectedCharacter!.StateRemainingSeconds = 0;
        targetPlayer.SelectedCharacter!.PlayerKillCount = 0;
        await targetPlayer.ForEachWorldObserverAsync<IUpdateCharacterHeroStatePlugIn>(p => p.UpdateCharacterHeroStateAsync(targetPlayer), true).ConfigureAwait(false);

        if (!targetPlayer.Name.Equals(gameMaster.Name))
        {
            await this.ShowMessageToAsync(targetPlayer, $"Your player kills have been cleaned by the game master.").ConfigureAwait(false);
        }

        await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] {targetPlayer.Name} kills have been cleaned.").ConfigureAwait(false);
    }
}