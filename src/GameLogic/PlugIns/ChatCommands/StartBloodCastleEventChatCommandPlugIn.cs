// <copyright file="StartBloodCastleEventChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the startbc command.
/// If an event is already running, it is stopped (all players are removed from it) and
/// a new event starts with fresh instances.
/// </summary>
[Guid("7177533A-F147-407E-97B0-C4D8E1AC1AF4")]
[PlugIn]
[Display(Name = nameof(PlugInResources.StartBloodCastleEventChatCommandPlugIn_Name), Description = nameof(PlugInResources.StartBloodCastleEventChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class StartBloodCastleEventChatCommandPlugIn : StartMiniGameEventChatCommandPlugInBase
{
    private const string Command = "/startbc";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    protected override MiniGameType MiniGameType => MiniGameType.BloodCastle;
}
