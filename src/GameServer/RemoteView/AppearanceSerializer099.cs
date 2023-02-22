// <copyright file="AppearanceSerializer099.cs" company="MUnique">
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
/// Serializer for the appearance of a player, compatible with the client of version 0.95.
/// </summary>
[Guid("99616318-38BC-4C0A-A818-29D821B78DE2")]
[PlugIn(nameof(AppearanceSerializer095), "Serializer for the appearance of a player, compatible with the client of version 0.95")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
public class AppearanceSerializer095 : IAppearanceSerializer
{
    /// <summary>
    /// A cache which holds the results of the serializer.
    /// </summary>
    private static readonly ConcurrentDictionary<IAppearanceData, byte[]> Cache = new ();

    private enum Pets
    {
        Angel = 0,
        Imp = 1,
        Unicorn = 2,
        Dinorant = 3,
    }

    private enum WingIndex
    {
        WingsOfElf = 0,
        WingsOfHeaven = 1,
        WingsOfSatan = 2,
        None = 3,
    }

    /// <inheritdoc/>
    public int NeededSpace => 11;

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

    private void OnAppearanceOfAppearanceChanged(object? sender, EventArgs args) => this.InvalidateCache(sender as IAppearanceData ?? throw new ArgumentException($"sender must be of type {nameof(IAppearanceData)}"));

    /// <summary>
    /// Writes the preview character set.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="appearanceData">The appearance data.</param>
    /// <returns></returns>
    private void WritePreviewCharSet(Span<byte> target, IAppearanceData appearanceData)
    {
        ItemAppearance?[] itemArray = new ItemAppearance[InventoryConstants.EquippableSlotsCount];
        for (byte i = 0; i < itemArray.Length; i++)
        {
            itemArray[i] = appearanceData.EquippedItems.FirstOrDefault(item => item.ItemSlot == i && item.Definition?.Number < 16);
        }

        if (appearanceData.CharacterClass is not null)
        {
            target[0] = (byte)(appearanceData.CharacterClass.Number << 3);
            /*  00 Dark Wizard
                10 Soul Master
                20 Dark Knight
                30 Blade Knight
                40 Elf
                50 Muse Elf
                60 Magic Gladiator*/
        }

        target[0] |= (byte)appearanceData.Pose;
        this.SetHand(target, itemArray[InventoryConstants.LeftHandSlot], 1);

        this.SetHand(target, itemArray[InventoryConstants.RightHandSlot], 2);

        this.SetArmorPiece(target, itemArray[InventoryConstants.HelmSlot], 3, true, 0x80);

        this.SetArmorPiece(target, itemArray[InventoryConstants.ArmorSlot], 3, false, 0x40);

        this.SetArmorPiece(target, itemArray[InventoryConstants.PantsSlot], 4, true, 0x20);

        this.SetArmorPiece(target, itemArray[InventoryConstants.GlovesSlot], 4, false, 0x10);

        this.SetArmorPiece(target, itemArray[InventoryConstants.BootsSlot], 5, true, 0x08);

        target[5] |= (byte)((itemArray[InventoryConstants.WingsSlot]?.Definition?.Number & 0x03) << 2 ?? 0b1100);
        this.SetPet(target, itemArray[InventoryConstants.PetSlot]);
        // index9: upper 5 bits are the equipped flags of SetArmorPiece

        this.SetItemLevels(target, itemArray);
    }

    private void SetPet(Span<byte> preview, ItemAppearance? item)
    {
        var pet = (Pets?)item?.Definition?.Number;
        switch (pet)
        {
            case Pets.Angel:
            case Pets.Imp:
            case Pets.Unicorn:
                preview[5] |= (byte)pet;
                break;
            case Pets.Dinorant:
                preview[5] |= 0b11;
                preview[9] |= 0b100;
                break;
            default:
                preview[5] |= 0b11;
                break;
        }
    }

    private void SetHand(Span<byte> preview, ItemAppearance? item, int index)
    {
        if (item?.Definition is null)
        {
            preview[index] = 0xFF;
        }
        else
        {
            preview[index] = (byte)item.Definition.Number;
            preview[index] |= (byte)(item.Definition.Group << 5);
        }
    }

    private byte GetOrMaskForHighNibble(int value)
    {
        return (byte)((value << 4) & 0xF0);
    }

    private byte GetOrMaskForLowNibble(int value)
    {
        return (byte)(value & 0x0F);
    }

    private void SetArmorPiece(Span<byte> preview, ItemAppearance? item, int firstIndex, bool firstIndexHigh, byte secondIndexMask)
    {
        if (item?.Definition is null)
        {
            this.SetEmptyArmor(preview, firstIndex, firstIndexHigh, secondIndexMask);
        }
        else
        {
            // item id
            this.SetArmorItemIndex(preview, item, firstIndex, firstIndexHigh, secondIndexMask);

            // exc bit
            if (this.IsExcellent(item))
            {
                preview[10] |= secondIndexMask;
            }
        }
    }

    private void SetEmptyArmor(Span<byte> preview, int firstIndex, bool firstIndexHigh, byte secondIndexMask)
    {
        // if the item is not equipped every index bit is set to 1
        preview[firstIndex] |= firstIndexHigh ? this.GetOrMaskForHighNibble(0x0F) : this.GetOrMaskForLowNibble(0x0F);
        preview[9] |= secondIndexMask;
    }

    private void SetArmorItemIndex(Span<byte> preview, ItemAppearance item, int firstIndex, bool firstIndexHigh, byte secondIndexMask)
    {
        preview[firstIndex] |= firstIndexHigh ? this.GetOrMaskForHighNibble(item.Definition!.Number) : this.GetOrMaskForLowNibble(item.Definition!.Number);
        byte multi = (byte)(item.Definition.Number / 16);
        if (multi > 0)
        {
            byte bit1 = (byte)(multi % 2);
            if (bit1 == 1)
            {
                preview[9] |= secondIndexMask;
            }
        }
    }

    private bool IsExcellent(ItemAppearance item)
    {
        return item.VisibleOptions.Contains(ItemOptionTypes.Excellent);
    }

    private void SetItemLevels(Span<byte> preview, ItemAppearance?[] itemArray)
    {
        int levelIndex = 0;
        for (int i = 0; i < 7; i++)
        {
            if (itemArray[i] is not null)
            {
                levelIndex |= itemArray[i]!.GetGlowLevel() << (i * 3);
            }
        }

        preview[6] = (byte)((levelIndex >> 16) & 255);
        preview[7] = (byte)((levelIndex >> 8) & 255);
        preview[8] = (byte)(levelIndex & 255);
    }

    private void AddWing(Span<byte> preview, ItemAppearance? wing)
    {
        if (wing?.Definition is null)
        {
            preview[5] |= 0x0C;
            return;
        }

        preview[5] |= (byte)((wing?.Definition?.Number & 0x03) << 2 ?? 0b1100);
            
    }
}