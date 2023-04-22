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
/// Default serializer for the appearance of a player.
/// </summary>
[Guid("54847CAF-7827-48FB-BF53-AF458A694FAF")]
[PlugIn("Default appearance serializer", "Default serializer for the appearance of a player. It will most likely only work correctly in season 6.")]
[MinimumClient(5, 0, ClientLanguage.Invariant)]
public class AppearanceSerializer : IAppearanceSerializer
{
    /// <summary>
    /// A cache which holds the results of the serializer.
    /// </summary>
    private static readonly ConcurrentDictionary<IAppearanceData, byte[]> Cache = new ();

    private enum PetIndex
    {
        Angel = 0,
        Imp = 1,
        Unicorn = 2,
        Dinorant = 3,
        DarkHorse = 4,
        DarkRaven = 5,
        Fenrir = 37,
        Demon = 64,
        SpiritOfGuardian = 65,
        Rudolph = 66,
        Panda = 80,
        PetUnicorn = 106,
        Skeleton = 123,
    }

    private enum WingIndex
    {
        WingsOfElf = 0,
        WingsOfHeaven = 1,
        WingsOfSatan = 2,
        WingsOfMistery = 41,
        WingsOfSpirit = 3,
        WingsOfSoul = 4,
        WingsOfDragon = 5,
        WingsOfDarkness = 6,
        CapeOfLord = 30, // other group, but index not overlapping with other wings
        WingsOfDespair = 42,
        CapeOfFighter = 49,
        WingOfStorm = 36,
        WingOfEternal = 37,
        WingOfIllusion = 38,
        WingOfRuin = 39,
        CapeOfEmperor = 40,
        WingOfDimension = 43,
        CapeOfOverrule = 50,
        SmallCapeOfLord = 130,
        SmallWingsOfMistery = 131,
        SmallWingsOfElf = 132,
        SmallWingsOfHeaven = 133,
        SmallWingsOfSatan = 134,
        SmallCloakOfWarrior = 135,
    }

    /// <inheritdoc/>
    public int NeededSpace => 18;

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
        ItemAppearance?[] itemArray = new ItemAppearance[InventoryConstants.EquippableSlotsCount];
        for (byte i = 0; i < itemArray.Length; i++)
        {
            itemArray[i] = appearanceData.EquippedItems.FirstOrDefault(item => item.ItemSlot == i);
        }

        if (appearanceData.CharacterClass is not null)
        {
            target[0] = (byte)(appearanceData.CharacterClass.Number << 3 & 0xF8);
        }

        target[0] |= (byte)appearanceData.Pose;
        this.SetHand(target, itemArray[InventoryConstants.LeftHandSlot], 1, 12);

        this.SetHand(target, itemArray[InventoryConstants.RightHandSlot], 2, 13);

        this.SetArmorPiece(target, itemArray[InventoryConstants.HelmSlot], 3, true, 0x80, 13, false);

        this.SetArmorPiece(target, itemArray[InventoryConstants.ArmorSlot], 3, false, 0x40, 14, true);

        this.SetArmorPiece(target, itemArray[InventoryConstants.PantsSlot], 4, true, 0x20, 14, false);

        this.SetArmorPiece(target, itemArray[InventoryConstants.GlovesSlot], 4, false, 0x10, 15, true);

        this.SetArmorPiece(target, itemArray[InventoryConstants.BootsSlot], 5, true, 0x08, 15, false);

        this.SetItemLevels(target, itemArray);

        if (appearanceData.FullAncientSetEquipped)
        {
            target[11] |= 0x01;
        }

        this.AddWing(target, itemArray[InventoryConstants.WingsSlot]);

        this.AddPet(target, itemArray[InventoryConstants.PetSlot]);
    }

    private void SetHand(Span<byte> preview, ItemAppearance? item, int indexIndex, int groupIndex)
    {
        if (item?.Definition is null)
        {
            preview[indexIndex] = 0xFF;
            preview[groupIndex] |= 0xF0;
        }
        else
        {
            preview[indexIndex] = (byte)item.Definition.Number;
            preview[groupIndex] |= (byte)(item.Definition.Group << 5);
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

    private void SetEmptyArmor(Span<byte> preview, int firstIndex, bool firstIndexHigh, byte secondIndexMask, int thirdIndex, bool thirdIndexHigh)
    {
        // if the item is not equipped every index bit is set to 1
        preview[firstIndex] |= firstIndexHigh ? this.GetOrMaskForHighNibble(0x0F) : this.GetOrMaskForLowNibble(0x0F);
        preview[9] |= secondIndexMask;
        preview[thirdIndex] |= thirdIndexHigh ? this.GetOrMaskForHighNibble(0x0F) : this.GetOrMaskForLowNibble(0x0F);
    }

    private void SetArmorItemIndex(Span<byte> preview, ItemAppearance item, int firstIndex, bool firstIndexHigh, byte secondIndexMask, int thirdIndex, bool thirdIndexHigh)
    {
        preview[firstIndex] |= firstIndexHigh ? this.GetOrMaskForHighNibble(item.Definition!.Number) : this.GetOrMaskForLowNibble(item.Definition!.Number);
        byte multi = (byte)(item.Definition.Number / 16);
        if (multi > 0)
        {
            byte bit1 = (byte)(multi % 2);
            byte byte2 = (byte)(multi / 2);
            if (bit1 == 1)
            {
                preview[9] |= secondIndexMask;
            }

            if (byte2 > 0)
            {
                preview[thirdIndex] |= thirdIndexHigh ? this.GetOrMaskForHighNibble(byte2) : this.GetOrMaskForLowNibble(byte2);
            }
        }
    }

    private void SetArmorPiece(Span<byte> preview, ItemAppearance? item, int firstIndex, bool firstIndexHigh, byte secondIndexMask, int thirdIndex, bool thirdIndexHigh)
    {
        if (item?.Definition is null)
        {
            this.SetEmptyArmor(preview, firstIndex, firstIndexHigh, secondIndexMask, thirdIndex, thirdIndexHigh);
        }
        else
        {
            // item id
            this.SetArmorItemIndex(preview, item, firstIndex, firstIndexHigh, secondIndexMask, thirdIndex, thirdIndexHigh);

            // exc bit
            if (this.IsExcellent(item))
            {
                preview[10] |= secondIndexMask;
            }

            // ancient bit
            if (this.IsAncient(item))
            {
                preview[11] |= secondIndexMask;
            }
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

    private void AddWing(Span<byte> preview, ItemAppearance? wing)
    {
        if (wing?.Definition is null)
        {
            return;
        }

        switch ((WingIndex)wing.Definition.Number)
        {
            case WingIndex.WingsOfElf:
            case WingIndex.WingsOfHeaven:
            case WingIndex.WingsOfSatan:
            case WingIndex.WingsOfMistery:
                preview[5] |= 0x04;
                break;
            case WingIndex.WingsOfSpirit:
            case WingIndex.WingsOfSoul:
            case WingIndex.WingsOfDragon:
            case WingIndex.WingsOfDarkness:
            case WingIndex.CapeOfLord:
            case WingIndex.WingsOfDespair:
            case WingIndex.CapeOfFighter:
                preview[5] |= 0x08;
                break;
            case WingIndex.WingOfStorm:
            case WingIndex.WingOfEternal:
            case WingIndex.WingOfIllusion:
            case WingIndex.WingOfRuin:
            case WingIndex.CapeOfEmperor:
            case WingIndex.WingOfDimension:
            case WingIndex.CapeOfOverrule:
            case WingIndex.SmallCapeOfLord:
            case WingIndex.SmallWingsOfMistery:
            case WingIndex.SmallWingsOfElf:
            case WingIndex.SmallWingsOfHeaven:
            case WingIndex.SmallWingsOfSatan:
            case WingIndex.SmallCloakOfWarrior:
                preview[5] |= 0x0C;
                break;
            default:
                // nothing to do
                break;
        }

        switch ((WingIndex)wing.Definition.Number)
        {
            case WingIndex.WingsOfElf:
            case WingIndex.WingsOfSpirit:
            case WingIndex.WingOfStorm:
                preview[9] |= 0x01;
                break;
            case WingIndex.WingsOfHeaven:
            case WingIndex.WingsOfSoul:
            case WingIndex.WingOfEternal:
                preview[9] |= 0x02;
                break;
            case WingIndex.WingsOfSatan:
            case WingIndex.WingsOfDragon:
            case WingIndex.WingOfIllusion:
                preview[9] |= 0x03;
                break;
            case WingIndex.WingsOfMistery:
            case WingIndex.WingsOfDarkness:
            case WingIndex.WingOfRuin:
                preview[9] |= 0x04;
                break;
            case WingIndex.CapeOfLord:
            case WingIndex.CapeOfEmperor:
                preview[9] |= 0x05;
                break;
            case WingIndex.WingsOfDespair:
            case WingIndex.WingOfDimension:
                preview[9] |= 0x06;
                break;
            case WingIndex.CapeOfFighter:
            case WingIndex.CapeOfOverrule:
                preview[9] |= 0x07;
                break;
            case WingIndex.SmallCapeOfLord:
                preview[17] |= 0x20;
                break;
            case WingIndex.SmallWingsOfMistery:
                preview[17] |= 0x40;
                break;
            case WingIndex.SmallWingsOfElf:
                preview[17] |= 0x60;
                break;
            case WingIndex.SmallWingsOfHeaven:
                preview[17] |= 0x80;
                break;
            case WingIndex.SmallWingsOfSatan:
                preview[17] |= 0xA0;
                break;
            case WingIndex.SmallCloakOfWarrior:
                preview[17] |= 0xC0;
                break;
            default:
                // nothing to do
                break;
        }
    }

    private void AddPet(Span<byte> preview, ItemAppearance? pet)
    {
        if (pet?.Definition is null)
        {
            preview[5] |= 0b0000_0011;
            return;
        }

        switch ((PetIndex)pet.Definition.Number)
        {
            case PetIndex.Angel:
            case PetIndex.Imp:
            case PetIndex.Unicorn:
                preview[5] |= (byte)pet.Definition.Number;
                break;
            case PetIndex.Dinorant:
                preview[5] |= 0x03;
                preview[10] |= 0x01;
                break;
            case PetIndex.DarkHorse:
                preview[5] |= 0x03;
                preview[12] |= 0x01;
                break;
            case PetIndex.Fenrir:
                preview[5] |= 0x03;
                preview[10] &= 0xFE;
                preview[12] &= 0xFE;
                preview[12] |= 0x04;
                preview[16] = 0x00;

                if (pet.VisibleOptions.Contains(ItemOptionTypes.BlackFenrir))
                {
                    preview[16] |= 0x01;
                }

                if (pet.VisibleOptions.Contains(ItemOptionTypes.BlueFenrir))
                {
                    preview[16] |= 0x02;
                }

                if (pet.VisibleOptions.Contains(ItemOptionTypes.GoldFenrir))
                {
                    preview[17] |= 0x01;
                }

                break;
            default:
                preview[5] |= 0x03;
                break;
        }

        switch ((PetIndex)pet.Definition.Number)
        {
            case PetIndex.Panda:
                preview[16] |= 0xE0;
                break;
            case PetIndex.PetUnicorn:
                preview[16] |= 0xA0;
                break;
            case PetIndex.Skeleton:
                preview[16] |= 0x60;
                break;
            case PetIndex.Rudolph:
                preview[16] |= 0x80;
                break;
            case PetIndex.SpiritOfGuardian:
                preview[16] |= 0x40;
                break;
            case PetIndex.Demon:
                preview[16] |= 0x20;
                break;
            default:
                // no further flag required.
                break;
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
}