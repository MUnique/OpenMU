// <copyright file="SkinChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.Persistence.BasicModel;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles skin commands.
/// </summary>
[Guid("4735CC2C-9E5D-457A-92CB-9D765F74FDFB")]
[PlugIn("Skin chat command", "Handles the chat command '/skin <number>'. Applies a monster skin to the game masters character.")]
[ChatCommandHelp(Command, "Applies a monster skin to the game masters character.", typeof(SkinChatCommandArgs), CharacterStatus.GameMaster)]
public class SkinChatCommandPlugIn : ChatCommandPlugInBase<SkinChatCommandArgs>
{
    private const string Command = "/skin";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc/>
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, SkinChatCommandArgs arguments)
    {
        if (gameMaster?.Attributes is { } attributes
            && attributes.GetComposableAttribute(Stats.TransformationSkin) is { } attribute)
        {
            attribute.Elements.ToList().ForEach(attribute.RemoveElement);
            attribute.AddElement(attributes.CreateElement(new PowerUpDefinitionValue { AggregateType = AggregateType.AddRaw, Value = arguments.SkinNumber }, Stats.TransformationSkin));

            attributes[Stats.TransformationSkin] = arguments.SkinNumber;
        }
    }
}