// <copyright file="ItemChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Administration;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles gm item command.
/// </summary>
/// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
[Guid("ABFE2440-E765-4F17-A588-BD9AE3799887")]
[PlugIn]
[Display(Name = nameof(PlugInResources.ItemChatCommandPlugIn_Name), Description = nameof(PlugInResources.ItemChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(ItemChatCommandArgs), CharacterStatus.GameMaster)]
public class ItemChatCommandPlugIn : ChatCommandPlugInBase<ItemChatCommandArgs>
{
    private const string Command = "/item";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, ItemChatCommandArgs arguments)
    {
        if (gameMaster.CurrentMap != null)
        {
            var (isValid, itemDefinition) = await TryParseArgumentsAsync(gameMaster, arguments).ConfigureAwait(false);
            if (!isValid)
            {
                return;
            }

            var item = GameMasterItemFactory.CreateItem(itemDefinition!, new ItemSpecification(arguments.Group, arguments.Number, arguments.Level, arguments.Skill, arguments.Luck, arguments.Opt, arguments.ExcellentNumber, arguments.Ancient, arguments.AncientBonusLevel));
            var dropCoordinates = gameMaster.CurrentMap.Terrain.GetRandomCoordinate(gameMaster.Position, 1);
            var droppedItem = new DroppedItem(item, dropCoordinates, gameMaster.CurrentMap, gameMaster);
            await gameMaster.CurrentMap.AddAsync(droppedItem).ConfigureAwait(false);
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemCreatedResult), this.Key, item).ConfigureAwait(false);
        }
    }

    private static async ValueTask<(bool Success, ItemDefinition? Definition)> TryParseArgumentsAsync(Player gameMaster, ItemChatCommandArgs arguments)
    {
        var itemDefinition = gameMaster.GameContext.Configuration.Items
            .FirstOrDefault(def => def.Group == arguments.Group && def.Number == arguments.Number);
        if (itemDefinition is null)
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemGroupNumberNotExists), arguments.Group, arguments.Number).ConfigureAwait(false);
            return (false, null);
        }

        if (arguments.Level > itemDefinition.MaximumItemLevel)
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemLevelExceeded), itemDefinition.MaximumItemLevel).ConfigureAwait(false);
            return (false, null);
        }

        return (true, itemDefinition);
    }
}
