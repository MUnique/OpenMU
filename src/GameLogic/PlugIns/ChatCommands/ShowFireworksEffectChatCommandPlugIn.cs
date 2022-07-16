// <copyright file="ShowFireworksEffectChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles fireworks effect commands.
/// </summary>
[Guid("658F7F9D-B8FF-4D52-A835-5B3D658B6B9F")]
[PlugIn("Show fireworks effect chat command", "Handles the chat command '/fireworks <x> <y>'. Shows an fireworks effect at the specified coordinates.")]
[ChatCommandHelp(Command, "Shows an fireworks effect at the specified coordinates.", typeof(CoordinatesCommandArgs), CharacterStatus.GameMaster)]
public class ShowFireworksEffectChatCommandPlugIn : ChatCommandPlugInBase<CoordinatesCommandArgs>
{
    private const string Command = "/fireworks";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc/>
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, CoordinatesCommandArgs arguments)
    {
        var coordinates = arguments.X == 0 ? gameMaster.Position : new Point(arguments.X, arguments.Y);
        await gameMaster.ForEachWorldObserverAsync<IShowItemDropEffectPlugIn>(p => p.ShowEffectAsync(ItemDropEffect.Fireworks, coordinates), true).ConfigureAwait(false);
    }
}