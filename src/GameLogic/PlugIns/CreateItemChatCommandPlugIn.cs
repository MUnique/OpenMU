// <copyright file="CreateItemChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles item creation command.
    /// </summary>
    /// <remarks>
    /// This should be deactivated by default or limited to game masters.
    /// </remarks>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.IChatCommandPlugIn" />
    [Guid("ABFE2440-E765-4F17-A588-BD9AE3799887")]
    [PlugIn("Create Item chat command", "Handles the chat command '/create'")]
    public class CreateItemChatCommandPlugIn : IChatCommandPlugIn
    {
        private const string CommandKey = "/item";

        private const string Usage = "[GM][/item] usage /item {Group} {Number} {Level} {Exc} {Skill} {Luck} {Opt}";

        /// <summary>
        /// arguments
        /// </summary>
        private struct Arguments : IEquatable<Arguments>
        {
            public byte Group { get; set; }

            public short Number { get; set; }

            public byte Level { get; set; }

            public byte Exc { get; set; }

            public bool Skill { get; set; }

            public bool Luck { get; set; }

            public byte Opt { get; set; }

            // Not implemented yet
            public bool Equals(Arguments other)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public string Key => CommandKey;

        /// <summary>
        /// Gets Min character status to use command
        /// </summary>
        public int MinStatusRequirement => (byte)CharacterStatus.GameMaster;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            var dropCoordinates = player.CurrentMap.Terrain.GetRandomDropCoordinate(player.Position, 1);
            try
            {
                var arguments = ParseArguments(command.Split(' ').Skip(1).ToList());
                var item = CreateItem(player, arguments);
                var droppedItem = new DroppedItem(item, dropCoordinates, player.CurrentMap, player);
                player.CurrentMap.Add(droppedItem);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage($"[GM][/item] {item} created", MessageType.BlueNormal);
            }
            catch (ArgumentException e)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(e.Message, MessageType.BlueNormal);
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

        private static Arguments ParseArguments(List<string> argumentsList)
        {
            if (argumentsList.ElementAtOrDefault(0) == null || argumentsList.ElementAtOrDefault(1) == null)
            {
                throw new ArgumentException(Usage);
            }

            return new Arguments
            {
                Group = byte.Parse(argumentsList.ElementAt(0)),
                Number = short.Parse(argumentsList.ElementAt(1)),
                Level = byte.Parse(argumentsList.ElementAtOrDefault(2) ?? "0"),
                Exc = byte.Parse(argumentsList.ElementAtOrDefault(3) ?? "0"),
                Skill = (argumentsList.ElementAtOrDefault(4) ?? "0") == "1",
                Luck = (argumentsList.ElementAtOrDefault(5) ?? "0") == "1",
                Opt = byte.Parse(argumentsList.ElementAtOrDefault(6) ?? "0"),
            };
        }
    }
}