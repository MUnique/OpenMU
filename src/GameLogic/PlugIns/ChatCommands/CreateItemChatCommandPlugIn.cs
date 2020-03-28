// <copyright file="CreateItemChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles item creation command.
    /// </summary>
    /// <remarks>
    /// This should be deactivated by default or limited to game masters.
    /// </remarks>
    [Guid("ABFE2440-E765-4F17-A588-BD9AE3799887")]
    [PlugIn("Create Item chat command", "Handles the chat command '/create'")]
    [ChatCommandHelp(Command, typeof(Arguments), MinimumStatus)]
    public class CreateItemChatCommandPlugIn : IChatCommandPlugIn
    {
        private const string Command = "/item";

        private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            var dropCoordinates = player.CurrentMap.Terrain.GetRandomDropCoordinate(player.Position, 1);
            try
            {
                var arguments = command.ParseArguments<Arguments>();
                var item = CreateItem(player, arguments);
                var droppedItem = new DroppedItem(item, dropCoordinates, player.CurrentMap, player);
                player.CurrentMap.Add(droppedItem);
                player.ShowMessage($"[GM][/item] {item} created");
            }
            catch (ArgumentException e)
            {
                player.ShowMessage(e.Message);
            }
        }

        private static Item CreateItem(Player player, Arguments arguments)
        {
            var item = new TemporaryItem();
            var itemDefinition = player.GameContext.Configuration.Items.FirstOrDefault(def => def.Group == arguments.Group && def.Number == arguments.Number);
            if (itemDefinition == null)
            {
                throw new ArgumentException($"[GM][/item] {arguments.Group} {arguments.Number} does not exists");
            }

            item.Definition = itemDefinition;
            item.Level = arguments.Level;
            item.Durability = itemDefinition.Durability;
            item.HasSkill = itemDefinition.Skill != null && arguments.Skill;

            if (arguments.Opt > 0)
            {
                var optionLink = new ItemOptionLink
                {
                    ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                        .First(o => o.OptionType == ItemOptionTypes.Option),
                    Level = arguments.Opt,
                };
                item.ItemOptions.Add(optionLink);
            }

            if (arguments.Luck)
            {
                var optionLink = new ItemOptionLink
                {
                    ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                        .First(o => o.OptionType == ItemOptionTypes.Luck),
                };
                item.ItemOptions.Add(optionLink);
            }

            if (arguments.Exc > 0)
            {
                var excellentOptions = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                    .Where(o => (o.Number & arguments.Exc) > 0);
                var appliedOptions = 0;

                excellentOptions.ForEach(option =>
                {
                    var optionLink = new ItemOptionLink
                    {
                        ItemOption = option,
                    };
                    item.ItemOptions.Add(optionLink);
                    appliedOptions++;
                });

                // every excellent item has skill (if is in item definition)
                if (appliedOptions > 0 && itemDefinition.Skill != null)
                {
                    item.HasSkill = true;
                }
            }

            return item;
        }

        private class Arguments : ArgumentsBase
        {
            [Argument("g")]
            public byte Group { get; set; }

            [Argument("n")]
            public short Number { get; set; }

            [Argument("l")]
            public byte Level { get; set; } = 1;

            [Argument("e")]
            public byte Exc { get; set; }

            [Argument("s")]
            public bool Skill { get; set; }

            [Argument("lu")]
            public bool Luck { get; set; }

            [Argument("o")]
            public byte Opt { get; set; }
        }
    }
}