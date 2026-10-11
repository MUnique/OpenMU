// <copyright file="AppearanceSerializer097.cs" company="MUnique">
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
/// Serializer for the appearance of a player, compatible with the client of version 0.97.
/// </summary>
/// <remarks>
/// Compared to version 0.95, it supports the second wings and armor items with a number of 16 and higher.
/// Because the second wings use the lower bits of index 9, the flag of the Dinorant moved to index 10.
/// Bit pattern:
/// Index   Bits
/// 0       cccc cpPP     c = character class, p = pose
/// 1       left hand item (number + group &lt;&lt; 5), 0xFF if empty
/// 2       right hand item (number + group &lt;&lt; 5), 0xFF if empty
/// 3       hhhh aaaa     h = helm number, a = armor number (lower 4 bits, 0xF if empty)
/// 4       pppp gggg     p = pants number, g = gloves number (lower 4 bits, 0xF if empty)
/// 5       bbbb wwPP     b = boots number (lower 4 bits, 0xF if empty), w = first wing (0b11 if none or second wing), P = pet (0b11 if none or Dinorant)
/// 6 to 8  item levels as glow levels, 3 bits for each of the 7 items
/// 9       HAPGB sss     5th bit of the numbers of helm, armor, pants, gloves, boots; s = second wing number - 2
/// 10      HAPGBLRD      excellent flags of helm, armor, pants, gloves, boots, left hand, right hand; D = Dinorant.
/// </remarks>
[Guid("8AFEAB19-F308-422D-8E11-8CCF0A44966B")]
[PlugIn]
[Display(Name = nameof(PlugInResources.AppearanceSerializer097_Name), Description = nameof(PlugInResources.AppearanceSerializer097_Description), ResourceType = typeof(PlugInResources))]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
public class AppearanceSerializer097 : IAppearanceSerializer
{
    private const byte NoArmorNumber = 0x1F;
    private const byte NoFirstWing = 0b11;
    private const byte NoPet = 0b11;
    private const byte DinorantNumber = 3;
    private const byte DinorantFlag = 0x01;
    private const byte FirstSecondWingNumber = 3;
    private const byte LastSecondWingNumber = 6;

    /// <summary>
    /// A cache which holds the results of the serializer.
    /// </summary>
    private static readonly ConcurrentDictionary<IAppearanceData, byte[]> Cache = new();

    /// <summary>
    /// The flags of the armor slots for the 5th bit of the item number at index 9, and their excellent flag at index 10.
    /// </summary>
    private static readonly (int Slot, int Index, bool HighNibble, byte Flag)[] ArmorSlots =
    {
        (InventoryConstants.HelmSlot, 3, true, 0x80),
        (InventoryConstants.ArmorSlot, 3, false, 0x40),
        (InventoryConstants.PantsSlot, 4, true, 0x20),
        (InventoryConstants.GlovesSlot, 4, false, 0x10),
        (InventoryConstants.BootsSlot, 5, true, 0x08),
    };

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
            WritePreviewCharSet(target, appearance);
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

    private static void WritePreviewCharSet(Span<byte> target, IAppearanceData appearanceData)
    {
        target.Slice(0, 11).Clear();
        var itemArray = new ItemAppearance?[InventoryConstants.EquippableSlotsCount];
        foreach (var item in appearanceData.EquippedItems)
        {
            if (item.ItemSlot < itemArray.Length)
            {
                itemArray[item.ItemSlot] = item;
            }
        }

        if (appearanceData.CharacterClass is not null)
        {
            target[0] = (byte)(appearanceData.CharacterClass.Number << 3);
        }

        target[0] |= (byte)appearanceData.Pose;

        SetHand(target, itemArray[InventoryConstants.LeftHandSlot], 1, 0x04);
        SetHand(target, itemArray[InventoryConstants.RightHandSlot], 2, 0x02);
        foreach (var (slot, index, highNibble, flag) in ArmorSlots)
        {
            SetArmorPiece(target, itemArray[slot], index, highNibble, flag);
        }

        SetWing(target, itemArray[InventoryConstants.WingsSlot]);
        SetPet(target, itemArray[InventoryConstants.PetSlot]);
        SetItemLevels(target, itemArray);
    }

    private static void SetHand(Span<byte> target, ItemAppearance? item, int index, byte excellentFlag)
    {
        if (item?.Definition is not { } definition)
        {
            target[index] = 0xFF;
            return;
        }

        target[index] = (byte)((definition.Number & 0x1F) | (definition.Group << 5));
        if (IsExcellent(item))
        {
            target[10] |= excellentFlag;
        }
    }

    private static void SetArmorPiece(Span<byte> target, ItemAppearance? item, int index, bool highNibble, byte flag)
    {
        var number = item?.Definition?.Number ?? NoArmorNumber;
        var lowerBits = (byte)(number & 0x0F);
        target[index] |= highNibble ? (byte)(lowerBits << 4) : lowerBits;
        if ((number & 0x10) != 0)
        {
            target[9] |= flag;
        }

        if (item is not null && IsExcellent(item))
        {
            target[10] |= flag;
        }
    }

    private static void SetWing(Span<byte> target, ItemAppearance? wing)
    {
        if (wing?.Definition is { Number: >= FirstSecondWingNumber and <= LastSecondWingNumber } secondWing)
        {
            target[5] |= NoFirstWing << 2;
            target[9] |= (byte)(secondWing.Number - (FirstSecondWingNumber - 1));
        }
        else if (wing?.Definition is { Number: < FirstSecondWingNumber } firstWing)
        {
            target[5] |= (byte)(firstWing.Number << 2);
        }
        else
        {
            target[5] |= NoFirstWing << 2;
        }
    }

    private static void SetPet(Span<byte> target, ItemAppearance? pet)
    {
        switch (pet?.Definition?.Number)
        {
            case { } number and < DinorantNumber:
                target[5] |= (byte)number;
                break;
            case DinorantNumber:
                target[5] |= NoPet;
                target[10] |= DinorantFlag;
                break;
            default:
                target[5] |= NoPet;
                break;
        }
    }

    private static void SetItemLevels(Span<byte> target, ItemAppearance?[] itemArray)
    {
        int levelIndex = 0;
        for (int i = 0; i < 7; i++)
        {
            if (itemArray[i] is { } item)
            {
                levelIndex |= item.GetGlowLevel() << (i * 3);
            }
        }

        target[6] = (byte)((levelIndex >> 16) & 0xFF);
        target[7] = (byte)((levelIndex >> 8) & 0xFF);
        target[8] = (byte)(levelIndex & 0xFF);
    }

    private static bool IsExcellent(ItemAppearance item)
    {
        return item.VisibleOptions.Contains(ItemOptionTypes.Excellent);
    }

    private void OnAppearanceOfAppearanceChanged(object? sender, EventArgs args) => this.InvalidateCache(sender as IAppearanceData ?? throw new ArgumentException($"sender must be of type {nameof(IAppearanceData)}"));
}
