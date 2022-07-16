// <copyright file="ShowNpcIdsChatCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Chat command to request the ids of all NPCs in the range of the player.
/// </summary>
[Guid("498D0205-388F-410A-A7C7-49069A64D3A9")]
[PlugIn("Show NPC ids chat command", "Handles the chat command '/showids'.")]
[ChatCommandHelp(Command, "Shows the IDs of all NPCs in the view range of the game master.", CharacterStatus.GameMaster)]
internal sealed class ShowNpcIdsChatCommand : IChatCommandPlugIn
{
    private const string Command = "/showids";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc/>
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var monsters = player.ObservingBuckets.SelectMany(b => b).OfType<NonPlayerCharacter>().ToList();
        foreach (var monster in monsters)
        {
            await player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync($"{monster.Id}", monster)).ConfigureAwait(false);
        }
    }
}