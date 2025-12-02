// <copyright file="AppearanceSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Extended serializer for the appearance of a player.
/// </summary>
[Guid("A17EDFF7-236B-4EC9-899A-A6FC8BBA840C")]
[PlugIn("Extended appearance serializer", "Extended serializer for the appearance of a player.")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class AppearanceSerializerExtended : IAppearanceSerializer
{
    /// <summary>
    /// A cache which holds the results of the serializer.
    /// </summary>
    private static readonly ConcurrentDictionary<IAppearanceData, byte[]> Cache = new();

    /// <inheritdoc/>
    public int NeededSpace => 27;

    /// <inheritdoc/>
    public void InvalidateCache(IAppearanceData appearance)
    {
        Cache.TryRemove(appearance, out _);
        appearance.AppearanceChanged -= this.OnAppearanceOfAppearanceChanged;
    }

    /// <inheritdoc/>
    public void WriteAppearanceData(Span<byte> target, IAppearanceData appearance, bool useCache)
    {
        if (target.Length < this.NeededSpace)
        {
            throw new ArgumentException($"Target span too small. Actual size: {target.Length}; Required: {this.NeededSpace}.", nameof(target));
        }

        if (useCache && Cache.TryGetValue(appearance, out var cached))
        {
            cached.CopyTo(target);
        }
        else
        {
            this.WritePreviewCharSet(target, appearance);
            if (useCache)
            {
                var cacheEntry = target.Slice(0, this.NeededSpace).ToArray();
                if (Cache.TryAdd(appearance, cacheEntry))
                {
                    appearance.AppearanceChanged += this.OnAppearanceOfAppearanceChanged;
                }
            }
        }
    }

    private void OnAppearanceOfAppearanceChanged(object? sender, EventArgs args)
    {
        this.InvalidateCache(sender as IAppearanceData ?? throw new ArgumentException($"sender must be of type {nameof(IAppearanceData)}"));
    }

    private void WritePreviewCharSet(Span<byte> target, IAppearanceData appearanceData)
    {
        var itemArray = new ItemAppearance?[InventoryConstants.EquippableSlotsCount];
        for (byte i = 0; i < itemArray.Length; i++)
        {
            itemArray[i] = appearanceData.EquippedItems.FirstOrDefault(item => item.ItemSlot == i);
        }

        if (appearanceData.CharacterClass is not null)
        {
            target[0] = appearanceData.CharacterClass.Number;
        }

        target[1] = (byte)appearanceData.Pose;
        if (appearanceData.FullAncientSetEquipped)
        {
            target[1] |= 0x10;
        }

        if (appearanceData.CharacterStatus == CharacterStatus.GameMaster)
        {
            target[1] |= 0x20;
        }
        
        var items = target[2..];
        SetShinyItem(items[0..3], itemArray[InventoryConstants.LeftHandSlot]);
        SetShinyItem(items[3..6], itemArray[InventoryConstants.RightHandSlot]);
        SetShinyItem(items[6..9], itemArray[InventoryConstants.HelmSlot]);
        SetShinyItem(items[9..12], itemArray[InventoryConstants.ArmorSlot]);
        SetShinyItem(items[12..15], itemArray[InventoryConstants.PantsSlot]);
        SetShinyItem(items[15..18], itemArray[InventoryConstants.GlovesSlot]);
        SetShinyItem(items[18..21], itemArray[InventoryConstants.BootsSlot]);
        SetUnshinyItem(items[21..23], itemArray[InventoryConstants.WingsSlot]);
        SetUnshinyItem(items[23..25], itemArray[InventoryConstants.PetSlot]);

        var pet = itemArray[InventoryConstants.PetSlot];
        if (pet is not null)
        {
            if (pet.VisibleOptions.Contains(ItemOptionTypes.BlackFenrir))
            {
                items[23] |= 0b10;
            }

            if (pet.VisibleOptions.Contains(ItemOptionTypes.BlueFenrir))
            {
                items[23] |= 0b100;
            }

            if (pet.VisibleOptions.Contains(ItemOptionTypes.GoldFenrir))
            {
                items[23] |= 0b110;
            }
        }
        
    }

    private void SetUnshinyItem(Span<byte> data, ItemAppearance? item)
    {
        var unshinyItem = new UnshinyItem(data);
        if (item?.Definition is null)
        {
            unshinyItem.Group = 0xF;
            unshinyItem.Number = 0xFFF;
        }
        else
        {
            unshinyItem.Group = item.Definition.Group;
            unshinyItem.Number = (ushort)item.Definition.Number;
        }
    }

    private void SetShinyItem(Span<byte> data, ItemAppearance? item)
    {
        var shinyItem = new ShinyItem(data);
        if (item?.Definition is null)
        {
            shinyItem.Group = 0xF;
            shinyItem.Number = 0xFFF;
            shinyItem.GlowLevel = 0;
            shinyItem.IsExcellent = false;
            shinyItem.IsAncient = false;
        }
        else
        {
            shinyItem.Group = item.Definition.Group;
            shinyItem.Number = (ushort)item.Definition.Number;
            shinyItem.GlowLevel = item.GetGlowLevel();
            shinyItem.IsExcellent = this.IsExcellent(item);
            shinyItem.IsAncient = this.IsAncient(item);
        }
    }

    private bool IsExcellent(ItemAppearance item)
    {
        return item.VisibleOptions.Contains(ItemOptionTypes.Excellent);
    }

    private bool IsAncient(ItemAppearance item)
    {
        return item.VisibleOptions.Contains(ItemOptionTypes.AncientOption);
    }

    /// <summary>
    /// Armor/Weapon Item: (3 bytes)
    ///     Group:  4 bit
    ///     Number: 12 bit
    ///     Level:  4 bit
    ///     IsExc:  1 bit
    ///     IsAnc:  1 bit
    /// </summary>
    private readonly ref struct ShinyItem(Span<byte> data)
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

        /// <summary>
        /// Gets or sets the glow level of the item.
        /// </summary>
        public byte GlowLevel
        {
            get => (byte)((this._data[2] & 0xF0) >> 4);
            set
            {
                value = (byte)((value & 0xF) << 4);
                value |= (byte)(this._data[2] & 0xF);
                this._data[2] = value;
            }
        }

        public bool IsExcellent
        {
            get => (this._data[2] & 0x08) != 0;
            set
            {
                if (value)
                {
                    this._data[2] |= 0b00001000;
                }
                else
                {
                    this._data[2] &= 0b11110111;
                }
            }
        }

        public bool IsAncient
        {
            get => (this._data[2] & 0x04) != 0;
            set
            {
                if (value)
                {
                    this._data[2] |= 0b00000100;
                }
                else
                {
                    this._data[2] &= 0b11111011;
                }
            }
        }
    }


    /// <summary>
    /// Unshiny Item: (2 bytes)
    ///     Group:  4 bit
    ///     Number: 12 bit
    /// </summary>
    private readonly ref struct UnshinyItem(Span<byte> data)
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
                this._data[1] = (byte)(value & 0xFF);
                this._data[0] = (byte)((this._data[0] & 0xF0) | ((value >> 8) & 0xF));
            }
        }
    }
}