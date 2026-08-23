// <copyright file="StartIllusionTempleEventChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startcc command.
/// </summary>
[Guid("A990270E-B9C6-4445-BBA9-56367A90D42D")]
[PlugIn]
[Display(Name = nameof(PlugInResources.StartIllusionTempleEventChatCommandPlugIn_Name), Description = nameof(PlugInResources.StartIllusionTempleEventChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class StartIllusionTempleEventChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/startit";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var illusionTemple = player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.IllusionTemple);
        illusionTemple?.ForceStart();
    }
}