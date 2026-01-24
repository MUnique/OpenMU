// <copyright file="ItemChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
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

            var item = CreateItem(gameMaster, itemDefinition!, arguments);
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

    private static Item CreateItem(Player gameMaster, DataModel.Configuration.Items.ItemDefinition itemDefinition, ItemChatCommandArgs arguments)
    {
        var item = new TemporaryItem();
        item.Definition = itemDefinition;
        item.Durability = item.IsStackable() ? 1 : item.Definition.Durability;
        item.HasSkill = item.Definition.Skill != null && arguments.Skill;
        item.Level = arguments.Level;
        item.SocketCount = item.Definition.MaximumSockets;

        AddOption(item, arguments);
        AddLuckOption(item, arguments);
        AddExcellentOptions(item, arguments);
        AddAncientBonusOption(item, arguments);

        return item;
    }

    private static void AddOption(TemporaryItem item, ItemChatCommandArgs arguments)
    {
        if (item.Definition != null && arguments.Opt > 0)
        {
            var allOptions = item.Definition.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.Option);
            IncreasableItemOption itemOption;

            if (item.Definition.Skill?.Number == 49) // Dinorant
            {
                if ((arguments.Opt & 1) > 0)
                {
                    itemOption = allOptions.First(o => o.PowerUpDefinition!.TargetAttribute == Stats.DamageReceiveDecrement);
                    var dinoOptionLink = new ItemOptionLink { ItemOption = itemOption, Level = 1 };
                    item.ItemOptions.Add(dinoOptionLink);
                }

                if ((arguments.Opt & 2) > 0)
                {
                    itemOption = allOptions.First(o => o.PowerUpDefinition!.TargetAttribute == Stats.MaximumAbility);
                    var dinoOptionLink = new ItemOptionLink { ItemOption = itemOption, Level = 2 };
                    item.ItemOptions.Add(dinoOptionLink);
                }

                if ((arguments.Opt & 4) > 0)
                {
                    itemOption = allOptions.First(o => o.PowerUpDefinition!.TargetAttribute == Stats.AttackSpeedAny);
                    var dinoOptionLink = new ItemOptionLink { ItemOption = itemOption, Level = 4 };
                    item.ItemOptions.Add(dinoOptionLink);
                }
            }
            else
            {
                itemOption = allOptions.First();
                var level = arguments.Opt;
                var optionLink = new ItemOptionLink { ItemOption = itemOption, Level = level };
                item.ItemOptions.Add(optionLink);
            }
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
        if (item.Definition != null && arguments.ExcellentNumber > 0)
        {
            var excellentOptions = item.Definition.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                .Where(o => ((1 << (o.Number - 1)) & arguments.ExcellentNumber) > 0)
                .ToList();

            ushort appliedOptions = 0;
            foreach (var excellentOption in excellentOptions)
            {
                var optionLink = new ItemOptionLink { ItemOption = excellentOption };
                item.ItemOptions.Add(optionLink);
                appliedOptions++;
            }

            // every excellent item has skill (if is in item definition)
            item.HasSkill = appliedOptions > 0 && item.Definition.Skill != null;
        }
    }

    private static void AddAncientBonusOption(TemporaryItem item, ItemChatCommandArgs arguments)
    {
        if (item.Definition != null && arguments.Ancient > 0
                                    && item.Definition.PossibleItemSetGroups.FirstOrDefault(g => g.Items.Any(i => i.ItemDefinition == item.Definition && i.AncientSetDiscriminator == arguments.Ancient)) is { } ancientSet
                                    && ancientSet.Items.FirstOrDefault(i => i.ItemDefinition == item.Definition) is { } itemOfItemSet)
        {
            var optionLink = new ItemOptionLink { ItemOption = itemOfItemSet.BonusOption, Level = arguments.AncientBonusLevel };
            item.ItemOptions.Add(optionLink);
            item.ItemSetGroups.Add(itemOfItemSet);
        }
    }
}