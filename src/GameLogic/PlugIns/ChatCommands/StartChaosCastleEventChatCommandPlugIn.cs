// <copyright file="StartChaosCastleEventChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startcc command.
/// </summary>
[Guid("A990270E-B9C6-4445-BBA9-56367A90D31D")]
[PlugIn(nameof(StartChaosCastleEventChatCommandPlugIn), "Handles the chat command '/startcc'. Starts the chaos castle event at the next possible time.")]
[ChatCommandHelp(Command, "Starts the chaos castle event at the next possible time.", CharacterStatus.GameMaster)]
public class StartChaosCastleEventChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/startcc";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var chaosCastle = player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.ChaosCastle);
        chaosCastle?.ForceStart();
    }
}