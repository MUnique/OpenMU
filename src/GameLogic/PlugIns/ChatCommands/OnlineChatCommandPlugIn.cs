// <copyright file="OnlineChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles online commands.
/// </summary>
[Guid("6693ABA3-7B35-4800-815B-096F3420E998")]
[PlugIn("Online chat command", "Handles the chat command '/online'. Gets the count of game masters and players online.")]
[ChatCommandHelp(Command, "Gets the online count of game masters and players.", typeof(EmptyChatCommandArgs), CharacterStatus.GameMaster)]
public class OnlineChatCommandPlugIn : ChatCommandPlugInBase<EmptyChatCommandArgs>, IChatCommandPlugIn
{
    private const string Command = "/online";

    /// <inheritdoc/>
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc/>
    protected override async ValueTask DoHandleCommandAsync(Player gameMasterPlayer, EmptyChatCommandArgs arguments)
    {
        var totalCharactersCount = 0;
        var totalGameMastersCount = 0;
        await gameMasterPlayer.GameContext.ForEachPlayerAsync(player =>
        {
            switch (player.SelectedCharacter?.CharacterStatus)
            {
                case CharacterStatus.Normal:
                    Interlocked.Increment(ref totalCharactersCount);
                    break;
                case CharacterStatus.GameMaster:
                    Interlocked.Increment(ref totalGameMastersCount);
                    break;
            }

            return Task.CompletedTask;
        }).ConfigureAwait(false);

        await this.ShowMessageToAsync(gameMasterPlayer, $"[{this.Key}] {totalGameMastersCount} GM(s) and {totalCharactersCount} player(s) online").ConfigureAwait(false);
    }
}