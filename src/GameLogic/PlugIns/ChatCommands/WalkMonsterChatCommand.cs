// <copyright file="WalkMonsterChatCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Chat command to let a monster walk to specific coordinates.
/// </summary>
[Guid("1852FED5-8184-431E-8C5F-5131356D348F")]
[PlugIn("Walk remote monster chat command", "Handles the chat command '/walkmonster <id> <x> <y>'. Walks a previously created monster which can be remote controlled by the GM.")]
[ChatCommandHelp(Command, "Walks a previously created monster which can be remote controlled by the game master.", typeof(MoveMonsterCommandArgs), CharacterStatus.GameMaster)]
internal class WalkMonsterChatCommand : ChatCommandPlugInBase<MoveMonsterCommandArgs>
{
    private const string Command = "/walkmonster";

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

        await monster.WalkToAsync(arguments.Coordinates).ConfigureAwait(false);
    }
}