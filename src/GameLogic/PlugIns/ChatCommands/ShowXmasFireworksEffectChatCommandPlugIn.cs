// <copyright file="ShowXmasFireworksEffectChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles xmas fireworks effect commands.
/// </summary>
[Guid("0E23E4CE-6E7B-4F29-92D8-04A1335EC722")]
[PlugIn("Show xmas fireworks effect chat command", "Handles the chat command '/xmasfireworks <x> <y>'. Shows an christmas fireworks effect at the specified coordinates.")]
[ChatCommandHelp(Command, "Shows an christmas fireworks effect at the specified coordinates.", typeof(CoordinatesCommandArgs), CharacterStatus.GameMaster)]
public class ShowXmasFireworksEffectChatCommandPlugIn : ChatCommandPlugInBase<CoordinatesCommandArgs>
{
    private const string Command = "/xmasfireworks";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc/>
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, CoordinatesCommandArgs arguments)
    {
        var coordinates = arguments.X == 0 ? gameMaster.Position : new Point(arguments.X, arguments.Y);
        await gameMaster.ForEachWorldObserverAsync<IShowItemDropEffectPlugIn>(p => p.ShowEffectAsync(ItemDropEffect.ChristmasFireworks, coordinates), true).ConfigureAwait(false);
    }
}