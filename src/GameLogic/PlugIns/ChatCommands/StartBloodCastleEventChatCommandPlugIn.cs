// <copyright file="StartBloodCastleEventChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startds command.
/// </summary>
[Guid("7177533A-F147-407E-97B0-C4D8E1AC1AF4")]
[PlugIn(nameof(StartBloodCastleEventChatCommandPlugIn), "Handles the chat command '/startbc'. Starts the blood castle event at the next possible time.")]
[ChatCommandHelp(Command, "Starts the blood castle event at the next possible time.", CharacterStatus.GameMaster)]
public class StartBloodCastleEventChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/startbc";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var bloodCastle = player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.BloodCastle);
        bloodCastle?.ForceStart();
    }
}