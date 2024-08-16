// <copyright file="ItemChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles gm item command.
/// </summary>
/// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
[Guid("ABFE2440-E765-4F17-A588-BD9AE3799887")]
[PlugIn("Item chat command", "Handles the chat command '/item <group> <number> <lvl?> <exc?> <sk?> <lu?> <opt?> <anc?> <ancBonuslvl?>'. Drops a specific item next to the character.")]
[ChatCommandHelp(Command, "Drops a specific item next to the character.", typeof(ItemChatCommandArgs), CharacterStatus.GameMaster)]
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
            var item = CreateItem(gameMaster, arguments);
            var dropCoordinates = gameMaster.CurrentMap.Terrain.GetRandomCoordinate(gameMaster.Position, 1);
            var droppedItem = new DroppedItem(item, dropCoordinates, gameMaster.CurrentMap, gameMaster);
            await gameMaster.CurrentMap.AddAsync(droppedItem).ConfigureAwait(false);
            await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] {item} created").ConfigureAwait(false);
        }
    }

    private static Item CreateItem(Player gameMaster, ItemChatCommandArgs arguments)
    {
        var item = new TemporaryItem();
        item.Definition = GetItemDefination(gameMaster, arguments);
        item.Durability = item.IsStackable() ? 1 : item.Definition.Durability;
        item.HasSkill = item.Definition.Skill != null && arguments.Skill;
        item.Level = GetItemLevel(item.Definition, arguments);
        item.SocketCount = item.Definition.MaximumSockets;

        AddOption(item, arguments);
        AddLuckOption(item, arguments);
        AddExcellentOptions(item, arguments);
        AddAncientBonusOption(item, arguments);

        return item;
    }

    private static ItemDefinition GetItemDefination(Player gameMaster, ItemChatCommandArgs arguments)
    {
        return gameMaster.GameContext.Configuration.Items
                   .FirstOrDefault(def => def.Group == arguments.Group && def.Number == arguments.Number)
               ?? throw new ArgumentException($"{arguments.Group} {arguments.Number} does not exist.");
    }

    private static byte GetItemLevel(ItemDefinition itemDefinition, ItemChatCommandArgs arguments)
    {
        if (arguments.Level > itemDefinition.MaximumItemLevel)
        {
            throw new ArgumentException($"Level cannot be greater than {itemDefinition.MaximumItemLevel}.");
        }

        return arguments.Level;
    }

    private static void AddOption(TemporaryItem item, ItemChatCommandArgs arguments)
    {
        if (item.Definition != null && arguments.Opt > default(byte))
        {
            var itemOption = item.Definition.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .First(o => o.OptionType == ItemOptionTypes.Option);

            var level = arguments.Opt;
            var optionLink = new ItemOptionLink { ItemOption = itemOption, Level = level };
            item.ItemOptions.Add(optionLink);
        }
    }

    private static void AddLuckOption(TemporaryItem item, ItemChatCommandArgs arguments)
    {
        if (item.Definition != null && arguments.Luck)
        {
            var optionLink = new ItemOptionLink
            {
                ItemOption = item.Definition.PossibleItemOptions
                    .SelectMany(o => o.PossibleOptions)
                    .First(o => o.OptionType == ItemOptionTypes.Luck),
            };

            item.ItemOptions.Add(optionLink);
        }
    }

    private static void AddExcellentOptions(TemporaryItem item, ItemChatCommandArgs arguments)
    {
        if (item.Definition != null && arguments.ExcellentNumber > default(byte))
        {
            var excellentOptions = item.Definition.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                .Where(o => ((1 << (o.Number - 1)) & arguments.ExcellentNumber) > default(byte))
                .ToList();

            ushort appliedOptions = default;
            foreach (var excellentOption in excellentOptions)
            {
                var optionLink = new ItemOptionLink { ItemOption = excellentOption };
                item.ItemOptions.Add(optionLink);
                appliedOptions++;
            }

            // every excellent item has skill (if is in item definition)
            item.HasSkill = appliedOptions > default(ushort) && item.Definition.Skill != null;
        }
    }

    private static void AddAncientBonusOption(TemporaryItem item, ItemChatCommandArgs arguments)
    {
        if (item.Definition != null && arguments.Ancient > default(byte)
                                    && item.Definition.PossibleItemSetGroups.FirstOrDefault(g => g.Items.Any(i => i.ItemDefinition == item.Definition && i.AncientSetDiscriminator == arguments.Ancient)) is { } ancientSet
                                    && ancientSet.Items.FirstOrDefault(i => i.ItemDefinition == item.Definition) is { } itemOfItemSet)
        {
            var optionLink = new ItemOptionLink { ItemOption = itemOfItemSet.BonusOption, Level = arguments.AncientBonusLevel };
            item.ItemOptions.Add(optionLink);
            item.ItemSetGroups.Add(itemOfItemSet);
        }
    }
}