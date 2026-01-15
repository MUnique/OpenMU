// <copyright file="RemoveNpcChatCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Chat command to remove a npc with a specific id.
/// </summary>
[Guid("34FAAD0A-FCA4-42E2-8F37-CEF48783BD78")]
[PlugIn]
[Display(Name = nameof(PlugInResources.RemoveNpcChatCommand_Name), Description = nameof(PlugInResources.RemoveNpcChatCommand_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, "Removes a NPC with the specified id.", typeof(IdCommandArgs), CharacterStatus.GameMaster)]
internal class RemoveNpcChatCommand : ChatCommandPlugInBase<IdCommandArgs>
{
    private const string Command = "/removenpc";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, IdCommandArgs arguments)
    {
        var monster = gameMaster.ObservingBuckets.SelectMany(b => b).OfType<NonPlayerCharacter>().FirstOrDefault(m => m.Id == arguments.Id);
        if (monster is null)
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NpcNotFoundById), arguments.Id).ConfigureAwait(false);
            return;
        }

        monster.Dispose();
        await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.NpcRemovedById), arguments.Id).ConfigureAwait(false);
    }
}