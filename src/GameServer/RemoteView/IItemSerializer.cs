// <copyright file="IItemSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Serializes the items into a byte array.
    /// </summary>
    public interface IItemSerializer
    {
        /// <summary>
        /// Gets the needed space for a serialized item.
        /// </summary>
        int NeededSpace { get; }

        /// <summary>
        /// Serializes the item into a byte array at the specified index.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="item">The item.</param>
        void SerializeItem(byte[] array, int startIndex, Item item);

        /// <summary>
        /// Deserializes the byte array into a new item instance.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="gameConfiguration">The game configuration. Required to determine the item definition.</param>
        /// <param name="persistenceContext">The persistence context. Required to create new objects.</param>
        /// <returns>The created item instance.</returns>
        Item DeserializeItem(byte[] array, int startIndex, GameConfiguration gameConfiguration, IContext persistenceContext);
    }

    /// <summary>
    /// This item serializer is used to serialize the item data to the data packets.
    /// At the moment, each item is serialized into a 12-byte long part of an array:
    /// Byte Order: ItemCode Options Dura Exe Ancient Kind/380Opt HarmonyOpt Socket1 Socket2 Socket3 Socket4 Socket5
    /// </summary>
    public class ItemSerializer : IItemSerializer
    {
        private const byte LuckFlag = 4;

        private const byte SkillFlag = 128;

        private const byte LevelMask = 0x78;

        private const byte Option380Flag = 0x08;

        private const byte NoSocket = 0xFF;

        private const byte EmptySocket = 0xFE;

        private const int MaximumSockets = 5;

        private const byte AncientBonusLevelMask = 0b1100;
        private const byte AncientDiscriminatorMask = 0b0011;
        private const byte AncientMask = AncientBonusLevelMask | AncientDiscriminatorMask;

        private const byte MaximumSocketOptions = 50;

        /// <inheritdoc/>
        public int NeededSpace
        {
            get { return 12; }
        }

        /// <inheritdoc/>
        public void SerializeItem(byte[] array, int startIndex, Item item)
        {
            array[startIndex] = (byte)item.Definition.Number;
            array[startIndex + 1] = (byte)((item.Level << 3) & LevelMask);

            var itemOption = item.ItemOptions.FirstOrDefault(o => o.ItemOption.OptionType == ItemOptionTypes.Option);
            if (itemOption != null)
            {
                // The item option level is splitted into 2 parts. Webzen... :-/
                array[startIndex + 1] += (byte)(itemOption.Level & 3); // setting the first 2 bits
                array[startIndex + 3] = (byte)((itemOption.Level & 4) << 4); // The highest bit is placed into the 2nd bit of the exc byte (0x40).

                // Some items (wings) can have different options (3rd wings up to 3!)
                // Alternate options are set at array[startIndex + 3] |= 0x20 and 0x10
                if (itemOption.ItemOption.Number != 0)
                {
                    array[startIndex + 3] |= (byte)((itemOption.ItemOption.Number & 0b11) << 4);
                }
            }

            array[startIndex + 2] = item.Durability;

            array[startIndex + 3] |= GetExcellentByte(item);

            if ((item.Definition.Number & 0x100) == 0x100)
            {
                // Support for 512 items per Group
                array[startIndex + 3] |= 0x80;
            }

            if (item.ItemOptions.Any(o => o.ItemOption.OptionType == ItemOptionTypes.Luck))
            {
                array[startIndex + 1] |= LuckFlag;
            }

            if (item.HasSkill)
            {
                array[startIndex + 1] |= SkillFlag;
            }

            var ancientSet = item.ItemSetGroups.FirstOrDefault(set => set.AncientSetDiscriminator != 0);
            if (ancientSet != null)
            {
                array[startIndex + 4] |= (byte)(ancientSet.AncientSetDiscriminator & AncientDiscriminatorMask);

                // an ancient item always has an ancient bonus option (e.g. 5 Vitality). If that's not the case, we should throw an exception.
                var ancientBonus =
                    item.ItemOptions.FirstOrDefault(o => o.ItemOption.OptionType == ItemOptionTypes.AncientBonus)
                    ?? throw new ArgumentException($"Item '{item}' belongs to an ancient set but doesn't have an ancient bonus option");

                array[startIndex + 4] |= (byte) ((ancientBonus.Level << 2) & AncientBonusLevelMask);
            }

            array[startIndex + 5] = (byte)(item.Definition.Group << 4);
            if (item.ItemOptions.Any(o => o.ItemOption.OptionType == ItemOptionTypes.Level380Option))
            {
                array[startIndex + 5] |= Option380Flag;
            }

            array[startIndex + 6] = GetHarmonyByte(item);
            SetSocketBytes(array, startIndex + 7, item);
        }

        /// <inheritdoc />
        public Item DeserializeItem(byte[] array, int startIndex, GameConfiguration gameConfiguration, IContext persistenceContext)
        {
            var itemNumber = array[startIndex] + ((array[startIndex] & 0x80) << 1);
            var itemGroup = (array[startIndex + 5] & 0xF0) >> 4;
            var definition = gameConfiguration.Items.FirstOrDefault(def => def.Number == itemNumber && def.Group == itemGroup)
                ?? throw new ArgumentException($"Couldn't find the item definition for the given byte array. Extracted item number and group: {itemNumber}, {itemGroup}");

            var item = persistenceContext.CreateNew<Item>();
            item.Definition = definition;
            item.Level = (byte)((array[startIndex + 1] & LevelMask) >> 3);
            item.Durability = array[startIndex + 2];

            if (item.Definition.PossibleItemOptions.Any(o =>
                o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Excellent)))
            {
                ReadExcellentOption(array[startIndex + 3], persistenceContext, item);
            }
            else if (item.Definition.PossibleItemOptions.Any(o =>
                o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Wing)))
            {
                ReadWingOption(array[startIndex + 3], persistenceContext, item);
            }

            ReadSkillFlag(array[startIndex + 1], item);
            ReadLuckOption(array[startIndex + 1], persistenceContext, item);
            ReadNormalOption(array, startIndex, persistenceContext, item);
            ReadAncientOption(array[startIndex + 4], persistenceContext, item);
            ReadLevel380Option(array[startIndex + 5], persistenceContext, item);
            ReadHarmonyOption(array[startIndex + 6], persistenceContext, item);
            ReadSockets(array.Skip(startIndex + 7), persistenceContext, item);
            return item;
        }

        private static void ReadSkillFlag(byte optionByte, Item item)
        {
            if ((optionByte & SkillFlag) == 0)
            {
                return;
            }

            if (item.Definition.Skill == null)
            {
                throw new ArgumentException($"The skill flag was set, but a skill is not defined for the specified item ({item.Definition.Number}, {item.Definition.Group})");
            }

            item.HasSkill = true;
        }

        private static void ReadLuckOption(byte optionByte, IContext persistenceContext, Item item)
        {
            if ((optionByte & LuckFlag) == 0)
            {
                return;
            }

            var luckOption = item.Definition.PossibleItemOptions
                                 .SelectMany(o => o.PossibleOptions.Where(i => i.OptionType == ItemOptionTypes.Luck))
                                 .FirstOrDefault()
                             ?? throw new ArgumentException($"The luck flag was set, but luck option is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
            var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = luckOption;
            item.ItemOptions.Add(optionLink);
        }

        private static void ReadWingOption(byte wingbyte, IContext persistenceContext, Item item)
        {
            var wingBits = wingbyte & 0x0F;
            var wingOptionDefinition = item.Definition.PossibleItemOptions.First(o =>
                o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Wing));
            foreach (var wingOption in wingOptionDefinition.PossibleOptions)
            {
                var optionMask = (byte)(1 << (wingOption.Number - 1));
                if ((wingBits & optionMask) == optionMask)
                {
                    var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
                    optionLink.ItemOption = wingOption;
                    item.ItemOptions.Add(optionLink);
                }
            }
        }

        private static void ReadExcellentOption(byte excByte, IContext persistenceContext, Item item)
        {
            var excellentBits = excByte & 0x3F;
            var excellentOptionDefinition = item.Definition.PossibleItemOptions.First(o =>
                o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Excellent));
            foreach (var excellentOption in excellentOptionDefinition.PossibleOptions)
            {
                var optionMask = (byte)(1 << (excellentOption.Number - 1));
                if ((excellentBits & optionMask) == optionMask)
                {
                    var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
                    optionLink.ItemOption = excellentOption;
                    item.ItemOptions.Add(optionLink);
                }
            }
        }

        private static void ReadNormalOption(byte[] array, int startIndex, IContext persistenceContext, Item item)
        {
            var optionLevel = (array[startIndex + 1] & 3) + ((array[startIndex + 3] >> 4) & 4);
            if (optionLevel == 0)
            {
                return;
            }

            var itemIsWing = item.Definition.PossibleItemOptions.Any(o => o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Wing));
            var optionNumber = itemIsWing ? (array[startIndex + 3] >> 4) & 0b11 : 0;
            var option = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions.Where(i => i.OptionType == ItemOptionTypes.Option && i.Number == optionNumber))
                .FirstOrDefault()
                ?? throw new ArgumentException($"The option with level {optionLevel} and number {optionNumber} is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
            var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = option;
            optionLink.Level = optionLevel;
            item.ItemOptions.Add(optionLink);
        }

        private static void ReadAncientOption(byte ancientByte, IContext persistenceContext, Item item)
        {
            
            if ((ancientByte & AncientMask) == 0)
            {
                return;
            }

            var bonusLevel = (ancientByte & AncientBonusLevelMask) >> 2;
            var setDiscriminator = ancientByte & AncientDiscriminatorMask;
            var ancientSets = item.Definition.PossibleItemSetGroups
                .Where(set => set.AncientSetDiscriminator == setDiscriminator && set.Options.Any(o => o.OptionType == ItemOptionTypes.AncientBonus)).ToList();
            if (ancientSets.Count > 1)
            {
                throw new ArgumentException($"Ambigious ancient set discriminator: {ancientSets.Count} sets with discriminator {setDiscriminator} found for item definition ({item.Definition.Number}, {item.Definition.Group}).");
            }

            var ancientSet = ancientSets.FirstOrDefault()
                             ?? throw new ArgumentException($"Couldn't find ancient set (discriminator {setDiscriminator}) for item ({item.Definition.Number}, {item.Definition.Group}).");
            item.ItemSetGroups.Add(ancientSet);
            var ancientBonus = ancientSet.Options.First(o => o.OptionType == ItemOptionTypes.AncientBonus);
            var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = ancientBonus;
            optionLink.Level = bonusLevel;
        }

        private static void ReadLevel380Option(byte option380Byte, IContext persistenceContext, Item item)
        {
            if ((option380Byte & Option380Flag) == 0)
            {
                return;
            }

            var option380 = item.Definition.PossibleItemOptions
                                .SelectMany(o => o.PossibleOptions.Where(i => i.OptionType == ItemOptionTypes.Level380Option))
                                .FirstOrDefault()
                            ?? throw new ArgumentException($"The lvl380 option flag was set, but the option is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
            var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = option380;
            item.ItemOptions.Add(optionLink);
        }

        private static void ReadSockets(IEnumerable<byte> socketBytes, IContext persistenceContext, Item item)
        {
            if (item.Definition.MaximumSockets == 0)
            {
                return;
            }

            var numberOfSockets = 0;
            foreach (var socketByte in socketBytes.Take(item.Definition.MaximumSockets))
            {
                if (socketByte == NoSocket)
                {
                    continue;
                }

                numberOfSockets++;
                if (socketByte == EmptySocket)
                {
                    continue;
                }

                var socketOption = item.Definition.PossibleItemOptions
                                       .SelectMany(o => o.PossibleOptions.Where(p => p.OptionType == ItemOptionTypes.SocketOption && p.Number == socketByte))
                                       .FirstOrDefault()
                                   ?? throw new ArgumentException($"The socket option {socketByte} was set, but the option is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
                var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = socketOption;
                item.ItemOptions.Add(optionLink);
            }

            item.SocketCount = numberOfSockets;
        }

        private static void ReadHarmonyOption(byte harmonyByte, IContext persistenceContext, Item item)
        {
            if (harmonyByte == 0)
            {
                return;
            }

            var level = harmonyByte & 0x0F;
            var optionNumber = (harmonyByte & 0xF0) >> 4;
            var harmonyOption = item.Definition.PossibleItemOptions
                                    .SelectMany(o => o.PossibleOptions.Where(p =>
                                        p.OptionType == ItemOptionTypes.HarmonyOption && p.Number == optionNumber))
                                    .FirstOrDefault()
                                ?? throw new ArgumentException(
                                    $"The harmony option {optionNumber} was set, but the option is not defined as possible option in the item definition ({item.Definition.Number}, {item.Definition.Group}).");
            var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = harmonyOption;
            optionLink.Level = level;
            item.ItemOptions.Add(optionLink);
        }

        private static void SetSocketBytes(byte[] array, int startIndex, Item item)
        {
            for (int i = 0; i < MaximumSockets; i++)
            {
                array[startIndex + i] = i < item.SocketCount ? EmptySocket : NoSocket;
            }

            var socketOptions = item.ItemOptions.Where(o => o.ItemOption.OptionType == ItemOptionTypes.SocketOption).Select(o => o.ItemOption.Number);
            int j = 0;
            foreach (int number in socketOptions)
            {
                array[startIndex + j] = (byte)number;
            }
        }

        private static byte GetHarmonyByte(Item item)
        {
            byte result = 0;
            var harmonyOption = item.ItemOptions.FirstOrDefault(o => o.ItemOption.OptionType == ItemOptionTypes.HarmonyOption);
            if (harmonyOption != null)
            {
                result = (byte)(harmonyOption.ItemOption.Number << 4);
                result |= (byte)harmonyOption.Level;
            }

            return result;
        }

        private static byte GetExcellentByte(Item item)
        {
            byte result = 0;
            var excellentOptions = item.ItemOptions.Where(o => o.ItemOption.OptionType == ItemOptionTypes.Excellent || o.ItemOption.OptionType == ItemOptionTypes.Wing);

            foreach (var option in excellentOptions)
            {
                result |= (byte)(1 << (option.ItemOption.Number - 1));
            }

            return result;
        }
    }

    /*
    public enum ExcArmorOptions
    {
        Zen = 1,
        DefenseRate = 2,
        Reflect = 4,
        DamageDecrease = 8,
        Mana = 16,
        HP = 32
    }

    public enum ExcWeaponOptions
    {
        Mana8 = 1,
        Life8 = 2,
        Speed7 = 4,
        Dmg2Percent = 8,
        DmgLvl20 = 16,
        ExcDmg = 32
    }

    public enum Lvl380Options
    {
        None = 0,
        AttackSuccessRate = 1,
        AddDamage = 2,
        DefenseSuccessRate = 3,
        Defense = 4,
        MaxHPinc = 5,
        MaxSDinc = 6,
        SDrecover = 7,
        SDrecoverRateInc = 8
    }
    */
}
