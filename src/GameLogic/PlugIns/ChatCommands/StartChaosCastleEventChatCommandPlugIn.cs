// <copyright file="StartChaosCastleEventChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startcc command.
/// If an event is already running, it is stopped (all players are removed from it) and
/// a new event starts with fresh instances.
/// </summary>
[Guid("A990270E-B9C6-4445-BBA9-56367A90D31D")]
[PlugIn]
[Display(Name = nameof(PlugInResources.StartChaosCastleEventChatCommandPlugIn_Name), Description = nameof(PlugInResources.StartChaosCastleEventChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class StartChaosCastleEventChatCommandPlugIn : StartMiniGameEventChatCommandPlugInBase
{
    private const string Command = "/startcc";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    protected override MiniGameType MiniGameType => MiniGameType.ChaosCastle;
}
