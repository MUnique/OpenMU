// <copyright file="StartDevilSquareEventChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startds command.
/// </summary>
[Guid("3684DC79-D81E-4033-AB2C-537334CF0BB6")]
[PlugIn(nameof(StartDevilSquareEventChatCommandPlugIn), "Handles the chat command '/startds'. Starts the devil square event at the next possible time.")]
[ChatCommandHelp(Command, "Starts the devil square event at the next possible time.", CharacterStatus.GameMaster)]
public class StartDevilSquareEventChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/startds";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var devilSquare = player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.DevilSquare);
        devilSquare?.ForceStart();
    }
}