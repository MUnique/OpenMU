// <copyright file="AppearanceSerializer075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Serializer for the appearance of a player, compatible with the client of version 0.75.
/// </summary>
[Guid("D20EEBFA-12C1-4A86-B202-63121EB2A95B")]
[PlugIn("Appearance Serializer 0.75", "Serializer for the appearance of a player, compatible with the client of version 0.75.")]
[MinimumClient(0, 75, ClientLanguage.Invariant)]
public class AppearanceSerializer075 : IAppearanceSerializer
{
    /// <summary>
    /// A cache which holds the results of the serializer.
    /// </summary>
    private static readonly ConcurrentDictionary<IAppearanceData, byte[]> Cache = new();

    /// <inheritdoc/>
    public int NeededSpace => 9;

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

    private void WritePreviewCharSet(Span<byte> target, IAppearanceData appearanceData)
    {
        ItemAppearance?[] itemArray = new ItemAppearance[InventoryConstants.BootsSlot + 1];
        for (byte i = 0; i < itemArray.Length; i++)
        {
            itemArray[i] = appearanceData.EquippedItems.FirstOrDefault(item => item.ItemSlot == i && item.Definition?.Number < 16);
        }

        if (appearanceData.CharacterClass is not null)
        {
            target[0] = (byte)(appearanceData.CharacterClass.Number << 3);
        }

        target[0] |= (byte)appearanceData.Pose;
        this.SetHand(target, itemArray[InventoryConstants.LeftHandSlot], 1);

        this.SetHand(target, itemArray[InventoryConstants.RightHandSlot], 2);

        this.SetArmorPiece(target, itemArray[InventoryConstants.HelmSlot], 3, true);

        this.SetArmorPiece(target, itemArray[InventoryConstants.ArmorSlot], 3, false);

        this.SetArmorPiece(target, itemArray[InventoryConstants.PantsSlot], 4, true);

        this.SetArmorPiece(target, itemArray[InventoryConstants.GlovesSlot], 4, false);

        this.SetArmorPiece(target, itemArray[InventoryConstants.BootsSlot], 5, true);
        var wing = appearanceData.EquippedItems.FirstOrDefault(item => item.ItemSlot == InventoryConstants.WingsSlot && item.Definition?.Number < 3);
        var pet = appearanceData.EquippedItems.FirstOrDefault(item => item.ItemSlot == InventoryConstants.PetSlot && item.Definition?.Number < 3);
        target[5] |= (byte)((wing?.Definition?.Number & 0x03) << 2 ?? 0b1100);
        target[5] |= (byte)(pet?.Definition?.Number & 0x03 ?? 0b0011);

        this.SetItemLevels(target, itemArray);
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
            preview[index] |= (byte)(item.Definition.Group << 4);
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

    private void SetArmorPiece(Span<byte> preview, ItemAppearance? item, int index, bool highNibble)
    {
        if (item?.Definition is null)
        {
            // if the item is not equipped every index bit is set to 1
            preview[index] |= highNibble ? this.GetOrMaskForHighNibble(0x0F) : this.GetOrMaskForLowNibble(0x0F);
        }
        else
        {
            var number = item.Definition.Number;
            if (number >= 10)
            {
                // Elf items start again at 0. What a stupid logic, as there are only 15 sets anyway.
                number -= 10;
            }

            preview[index] |= highNibble ? this.GetOrMaskForHighNibble(number) : this.GetOrMaskForLowNibble(number);
        }
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
}