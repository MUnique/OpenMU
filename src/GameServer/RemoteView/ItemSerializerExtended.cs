// <copyright file="ItemSerializerExtended.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

using static ItemSerializerHelper;

/// <summary>
/// This item serializer is used to serialize the item data to the data packets.
/// At the moment, each item is serialized into a dynamical length 5 to 15-byte
/// long part of an array.
/// </summary>
[Guid("9EBB4761-93D4-49DE-AC53-BD8744315439")]
[PlugIn("Item Serializer", "The extended item serializer. It's most likely only correct for season 6.")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class ItemSerializerExtended : IItemSerializer
{
    [Flags]
    private enum OptionFlags : byte
    {
        None = 0x00,
        HasOption = 0x01,
        HasLuck = 0x02,
        HasSkill = 0x04,
        HasExcellent = 0x08,
        HasAncient = 0x10,
        HasHarmony = 0x20,
        HasGuardian = 0x40,
        HasSockets = 0x80,
    }

    /// <inheritdoc/>
    public int NeededSpace => 15;

    /// <inheritdoc/>
    public int SerializeItem(Span<byte> target, Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));
        var targetStruct = new ItemStruct(target);
        targetStruct.Group = (byte)item.Definition.Group;
        targetStruct.Number = (ushort)item.Definition.Number;
        targetStruct.Level = item.IsTrainablePet() ? (byte)0 : (byte)item.Level;
        targetStruct.Durability = item.Durability();
        targetStruct.Options = this.GetOptionFlags(item);

        if (item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option) is { } itemOption)
        {
            targetStruct.OptionLevel = (byte)(itemOption.Level & 0xF);

            // Some items (wings) can have different options (3rd wings up to 3!)
            targetStruct.OptionType = (byte)((itemOption.ItemOption?.Number ?? 0) & 0xF);
        }

        if (targetStruct.Options.HasFlag(OptionFlags.HasExcellent))
        {
            targetStruct.Excellent = GetExcellentByte(item);
            targetStruct.Excellent |= GetFenrirByte(item);
        }

        if (targetStruct.Options.HasFlag(OptionFlags.HasAncient)
            && item.ItemSetGroups.FirstOrDefault(set => set.AncientSetDiscriminator != 0) is { } ancientSet)
        {
            targetStruct.AncientDiscriminator = (byte)ancientSet.AncientSetDiscriminator;

            // An ancient item may or may not have an ancient bonus option. Example without bonus: Gywen Pendant.
            if (item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.AncientBonus) is { } ancientBonus)
            {
                targetStruct.AncientBonusLevel = (byte)ancientBonus.Level;
            }
        }

        if (targetStruct.Options.HasFlag(OptionFlags.HasHarmony))
        {
            targetStruct.Harmony = GetHarmonyByte(item);
        }

        if (targetStruct.Options.HasFlag(OptionFlags.HasSockets))
        {
            targetStruct.SocketCount = (byte)item.SocketCount;
            targetStruct.SocketBonus = GetSocketBonusByte(item);
            SetSocketBytes(targetStruct.Sockets, item);
        }

        return targetStruct.Length;
    }

    /// <inheritdoc />
    public Item DeserializeItem(Span<byte> array, GameConfiguration gameConfiguration, IContext persistenceContext)
    {
        var itemStruct = new ItemStruct(array);
        var itemGroup = itemStruct.Group;
        var itemNumber = itemStruct.Number;
        var definition = gameConfiguration.Items.FirstOrDefault(def => def.Number == itemNumber && def.Group == itemGroup)
                         ?? throw new ArgumentException($"Couldn't find the item definition for the given byte array. Extracted item number and group: {itemNumber}, {itemGroup}");

        var item = persistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Level = itemStruct.Level;
        item.Durability = itemStruct.Durability;
        item.HasSkill = itemStruct.Options.HasFlag(OptionFlags.HasSkill) && item.Definition?.Skill is not null;

        if (itemStruct.Options.HasFlag(OptionFlags.HasOption))
        {
            AddNormalOption(itemStruct.OptionType, itemStruct.OptionLevel, persistenceContext, item);
        }

        if (itemStruct.Options.HasFlag(OptionFlags.HasLuck))
        {
            AddLuckOption(persistenceContext, item);
        }

        if (itemStruct.Options.HasFlag(OptionFlags.HasExcellent))
        {
            if (item.IsWing())
            {
                ReadWingOptionBits(itemStruct.Excellent, persistenceContext, item);
            }
            else
            {
                ReadExcellentOptionBits(itemStruct.Excellent, persistenceContext, item);
            }
        }

        if (itemStruct.Options.HasFlag(OptionFlags.HasAncient))
        {
            AddAncientOption(itemStruct.AncientDiscriminator, itemStruct.AncientBonusLevel, persistenceContext, item);
        }

        if (itemStruct.Options.HasFlag(OptionFlags.HasHarmony))
        {
            AddHarmonyOption(itemStruct.Harmony, persistenceContext, item);
        }

        if (itemStruct.Options.HasFlag(OptionFlags.HasGuardian))
        {
            AddLevel380Option(persistenceContext, item);
        }

        if (itemStruct.Options.HasFlag(OptionFlags.HasSockets))
        {
            ReadSocketBonus(itemStruct.SocketBonus, persistenceContext, item);
            ReadSockets(itemStruct.Sockets, persistenceContext, item);
        }

        return item;
    }

    private OptionFlags GetOptionFlags(Item item)
    {
        OptionFlags result = default;
        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck))
        {
            result |= OptionFlags.HasLuck;
        }

        if (item.HasSkill)
        {
            result |= OptionFlags.HasSkill;
        }

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Option))
        {
            result |= OptionFlags.HasOption;
        }

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent
                                      || o.ItemOption?.OptionType == ItemOptionTypes.Wing
                                      || o.ItemOption?.OptionType == ItemOptionTypes.BlackFenrir
                                      || o.ItemOption?.OptionType == ItemOptionTypes.BlueFenrir
                                      || o.ItemOption?.OptionType == ItemOptionTypes.GoldFenrir))
        {
            result |= OptionFlags.HasExcellent;
        }

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.HarmonyOption))
        {
            result |= OptionFlags.HasHarmony;
        }

        if (item.ItemSetGroups.Any(set => set.AncientSetDiscriminator != 0))
        {
            result |= OptionFlags.HasAncient;
        }

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.GuardianOption))
        {
            result |= OptionFlags.HasGuardian;
        }

        if (item.SocketCount > 0)
        {
            result |= OptionFlags.HasSockets;
        }

        return result;
    }

    /// <summary>
    /// Layout:
    ///   Group:  4 bit
    ///   Number: 12 bit
    ///   Level:  8 bit
    ///   Dura:   8 bit
    ///   OptFlags: 8 bit
    ///     HasOpt
    ///     HasLuck
    ///     HasSkill
    ///     HasExc
    ///     HasAnc
    ///     HasGuardian
    ///     HasHarmony
    ///     HasSockets
    ///   Optional, depending on Flags:
    ///     Opt_Lvl 4 bit
    ///     Opt_Typ 4 bit
    ///     Exc:    8 bit
    ///     Anc_Dis 4 bit
    ///     Anc_Bon 4 bit
    ///     Harmony 8 bit
    ///     Soc_Bon 4 bit
    ///     Soc_Cnt 4 bit
    ///     Sockets n * 8 bit
    ///
    ///  Total: 5 ~ 15 bytes.
    /// </summary>
    private readonly ref struct ItemStruct(Span<byte> data)
    {
        private readonly Span<byte> _data = data;

        public byte Group
        {
            get => (byte)((this._data[0] >> 4) & 0xF);
            set
            {
                value <<= 4;
                value |= (byte)(this._data[0] & 0xF);
                this._data[0] = value;
            }
        }

        public ushort Number
        {
            get => (ushort)(((this._data[0] & 0xF) << 8) + this._data[1]);
            set
            {
                // Higher 4 bits of the first byte for the higher bits of the value
                this._data[0] = (byte)((this._data[0] & 0xF0) | (((value & 0x0F00) >> 8) & 0xF));

                // The lower bits in the second byte
                this._data[1] = (byte)(value & 0xFF);
            }
        }

        public byte Level
        {
            get => this._data[2];
            set => this._data[2] = value;
        }

        public byte Durability
        {
            get => this._data[3];
            set => this._data[3] = value;
        }

        public OptionFlags Options
        {
            get => (OptionFlags)this._data[4];
            set => this._data[4] = (byte)value;
        }

        public byte OptionByte
        {
            get => this.Options.HasFlag(OptionFlags.HasOption) ? this._data[5] : default;
            set => this._data[5] = value;
        }

        public byte OptionLevel
        {
            get => this.Options.HasFlag(OptionFlags.HasOption) ? (byte)(this._data[5] & 0xF) : default;
            set => this._data[5] = (byte)((this._data[5] & 0xF0) | (value & 0xF));
        }

        public byte OptionType
        {
            get => this.Options.HasFlag(OptionFlags.HasOption) ? (byte)((this._data[5] & 0xF0) >> 4) : default;
            set
            {
                value = (byte)((value & 0xF) << 4);
                value |= (byte)(this._data[5] & 0xF);
                this._data[5] = value;
            }
        }

        public byte Excellent
        {
            get => this.Options.HasFlag(OptionFlags.HasExcellent) ? this._data[this.ExcellentIndex] : default;
            set => this._data[this.ExcellentIndex] = value;
        }

        public byte AncientByte
        {
            get => this.Options.HasFlag(OptionFlags.HasAncient) ? this._data[this.AncientIndex] : default;
            set => this._data[this.AncientIndex] = value;
        }

        public byte AncientDiscriminator
        {
            get => this.Options.HasFlag(OptionFlags.HasAncient) ? (byte)(this._data[this.AncientIndex] & 0xF) : default;
            set => this._data[this.AncientIndex] = (byte)((this._data[this.AncientIndex] & 0xF0) | (value & 0xF));
        }

        public byte AncientBonusLevel
        {
            get => this.Options.HasFlag(OptionFlags.HasAncient) ? (byte)((this._data[this.AncientIndex] & 0xF0) >> 4) : default;
            set
            {
                value = (byte)((value & 0xF) << 4);
                value |= (byte)(this._data[this.AncientIndex] & 0xF);
                this._data[this.AncientIndex] = value;
            }
        }

        public byte Harmony
        {
            get => this.Options.HasFlag(OptionFlags.HasHarmony) ? this._data[this.HarmonyIndex] : default;
            set => this._data[this.HarmonyIndex] = value;
        }

        public byte SocketBonus
        {
            get => this.Options.HasFlag(OptionFlags.HasSockets) ? (byte)((this._data[this.SocketStartIndex] & 0xF0) >> 4) : default;
            set
            {
                value = (byte)((value & 0xF) << 4);
                value |= (byte)(this._data[this.SocketStartIndex] & 0xF);
                this._data[this.SocketStartIndex] = value;
            }
        }

        public byte SocketCount
        {
            get => this.Options.HasFlag(OptionFlags.HasSockets) ? (byte)(this._data[this.SocketStartIndex] & 0xF) : default;
            set => this._data[this.SocketStartIndex] = (byte)((this._data[this.SocketStartIndex] & 0xF0) | (value & 0xF));
        }

        public Span<byte> Sockets => this.Options.HasFlag(OptionFlags.HasSockets)
            ? this._data.Slice(this.SocketStartIndex + 1, this.SocketCount)
            : [];

        public int Length
        {
            get
            {
                int size = 5;
                if (this.Options.HasFlag(OptionFlags.HasOption))
                {
                    size++;
                }

                if (this.Options.HasFlag(OptionFlags.HasExcellent))
                {
                    size++;
                }

                if (this.Options.HasFlag(OptionFlags.HasAncient))
                {
                    size++;
                }

                if (this.Options.HasFlag(OptionFlags.HasHarmony))
                {
                    size++;
                }

                if (this.Options.HasFlag(OptionFlags.HasSockets))
                {
                    size++;
                    size += this.SocketCount;
                }

                return size;
            }
        }

        private int AdditionalOptionIndex => this.Options.HasFlag(OptionFlags.HasOption) ? 5 : 4;

        private int ExcellentIndex => this.Options.HasFlag(OptionFlags.HasExcellent) ? this.AdditionalOptionIndex + 1 : this.AdditionalOptionIndex;

        private int AncientIndex => this.Options.HasFlag(OptionFlags.HasAncient) ? this.ExcellentIndex + 1 : this.ExcellentIndex;

        private int HarmonyIndex => this.Options.HasFlag(OptionFlags.HasHarmony) ? this.AncientIndex + 1 : this.AncientIndex;

        private int SocketStartIndex => this.Options.HasFlag(OptionFlags.HasSockets) ? this.HarmonyIndex + 1 : this.HarmonyIndex;
    }
}