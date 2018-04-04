// <copyright file="IItemSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;

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
    }

    /// <summary>
    /// This item serializer is used to serialize the item data to the data packets.
    /// At the moment, each item is serialized into a 12-byte long part of an array:
    /// Byte Order: ItemCode Options Dura Exe Ancient Kind 00 Socket1 Socket2 Socket3 Socket4 Socket5
    /// </summary>
    public class ItemSerializer : IItemSerializer
    {
        private const byte LuckFlag = 4;

        private const byte SkillFlag = 128;

        private const byte Option380Flag = 128;

        private const byte NoSocket = 0xFF;

        private const byte EmptySocket = 0xFE;

        private const int MaximumSockets = 5;

        private const byte MaximumSocketOptions = 50;

        /// <inheritdoc/>
        public int NeededSpace
        {
            get { return 12; }
        }

        /// <inheritdoc/>
        public void SerializeItem(byte[] array, int startIndex, Item item)
        {
            array[startIndex] = item.Definition.Number;
            ////Item Level:
            array[startIndex + 1] = (byte)((item.Level << 3) & 0x78);
            ////Item Option:
            ////It is splitted into 2 parts. Webzen... :-/
            var itemOptionLevel = item.ItemOptions.FirstOrDefault(o => o.ItemOption.OptionType == ItemOptionTypes.Option)?.Level ?? 0;
            array[startIndex + 1] += (byte)(itemOptionLevel & 3); // setting the first 2 bits
            array[startIndex + 3] = (byte)((itemOptionLevel & 4) << 4); // The highest bit is placed into the 2nd bit of the exc byte.

            array[startIndex + 2] = item.Durability;

            array[startIndex + 3] |= GetExcellentByte(item);
            if (item.ItemOptions.Any(o => o.ItemOption.OptionType == ItemOptionTypes.Level380Option))
            {
                array[startIndex + 3] |= Option380Flag;
            }

            if (item.ItemOptions.Any(o => o.ItemOption.OptionType == ItemOptionTypes.Luck))
            {
                array[startIndex + 1] |= LuckFlag;
            }

            if (item.HasSkill)
            {
                array[startIndex + 1] |= SkillFlag;
            }

            var ancientBonus = item.ItemOptions.FirstOrDefault(o => o.ItemOption.OptionType == ItemOptionTypes.AncientBonus);
            if (ancientBonus != null)
            {
                array[startIndex + 4] = ancientBonus.Level == 1 ? (byte)5 : ancientBonus.Level == 2 ? (byte)9 : (byte)0; // TODO: Check if this is right
            }

            array[startIndex + 5] = (byte)(item.Definition.Group << 4);
            array[startIndex + 6] = GetHarmonyByte(item);
            SetSocketBytes(array, startIndex + 7, item);
        }

        private static void SetSocketBytes(byte[] array, int startIndex, Item item)
        {
            for (int i = 0; i < MaximumSockets; i++)
            {
                array[startIndex + i] = NoSocket;
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
            var excellentOptions = item.ItemOptions.Where(o => o.ItemOption.OptionType == ItemOptionTypes.Excellent);

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
