// <copyright file="MoveMonsterChatCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Chat command to instantly move a monster to specific coordinates.
/// </summary>
[Guid("B3DE58F3-B604-4F59-9122-E686AD90BE7B")]
[PlugIn("Move remote monster chat command", "Handles the chat command '/movemonster <id> <x> <y>'. Moves a previously created monster which can be remote controlled by the GM.")]
[ChatCommandHelp(Command, "Moves a previously created monster which can be remote controlled by the game master.", typeof(MoveMonsterCommandArgs), CharacterStatus.GameMaster)]
internal class MoveMonsterChatCommand : ChatCommandPlugInBase<MoveMonsterCommandArgs>
{
    private const string Command = "/movemonster";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, MoveMonsterCommandArgs arguments)
    {
        var monster = gameMaster.ObservingBuckets.SelectMany(b => b).OfType<Monster>().FirstOrDefault(m => m.Id == arguments.Id);
        if (monster is null)
        {
            await this.ShowMessageToAsync(gameMaster, $"Monster with id {arguments.Id} not found.").ConfigureAwait(false);
            return;
        }

        await monster.MoveAsync(arguments.Coordinates).ConfigureAwait(false);
    }
}