// <copyright file="AppearanceSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Collections.Generic;
    using System.Linq;

    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Default serializer for the appearance of a player.
    /// </summary>
    public class AppearanceSerializer : IAppearanceSerializer
    {
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
        public byte[] GetAppearanceData(IAppearanceData appearance)
        {
            return this.GetPreviewCharSet(appearance.CharacterClass, appearance.EquippedItems);
        }

        private byte[] GetPreviewCharSet(CharacterClass characterClass, IEnumerable<ItemAppearance> wearingItems)
        {
            byte[] preview = new byte[18];
            ItemAppearance[] itemArray = new ItemAppearance[12];
            for (byte i = 0; i < itemArray.Length; i++)
            {
                itemArray[i] = wearingItems.FirstOrDefault(item => item.ItemSlot == i);
            }

            preview[0] = (byte)(characterClass.Number << 3 & 0xF8);
            this.SetHand(preview, itemArray[InventoryConstants.LeftHandSlot], 1, 12);

            this.SetHand(preview, itemArray[InventoryConstants.RightHandSlot], 2, 13);

            this.SetArmorPiece(preview, itemArray[InventoryConstants.HelmSlot], 3, true, 0x80, 13, false);

            this.SetArmorPiece(preview, itemArray[InventoryConstants.ArmorSlot], 3, false, 0x40, 14, true);

            this.SetArmorPiece(preview, itemArray[InventoryConstants.PantsSlot], 4, true, 0x20, 14, false);

            this.SetArmorPiece(preview, itemArray[InventoryConstants.GlovesSlot], 4, false, 0x10, 15, true);

            this.SetArmorPiece(preview, itemArray[InventoryConstants.BootsSlot], 5, true, 0x08, 15, false);

            this.SetItemLevels(preview, itemArray);

            this.SetAncientSetCompleteness(preview, itemArray);

            this.AddWing(preview, itemArray[InventoryConstants.WingsSlot]);

            this.AddPet(preview, itemArray[InventoryConstants.PetSlot]);

            return preview;
        }

        private void SetAncientSetCompleteness(byte[] preview, ItemAppearance[] itemArray)
        {
            var isAncientSetComplete = false;

            // TODO
            if (isAncientSetComplete)
            {
                preview[11] |= 0x01;
            }
        }

        private void SetHand(byte[] preview, ItemAppearance item, int indexIndex, int groupIndex)
        {
            if (item == null)
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

        private void SetArmorPiece(byte[] preview, ItemAppearance item, int firstIndex, bool firstIndexHigh, byte secondIndexMask, int thirdIndex, bool thirdIndexHigh)
        {
            if (item == null)
            {
                // if the item is not equipped every index bit is set to 1
                preview[firstIndex] |= (byte)(0x0F << (firstIndexHigh ? 4 : 0));
                preview[9] |= secondIndexMask;
                preview[thirdIndex] |= (byte)(0x0F << (thirdIndexHigh ? 4 : 0));
            }
            else
            {
                // item id
                preview[firstIndex] |= (byte)((item.Definition.Number << (firstIndexHigh ? 4 : 0)) & (0x0F << (firstIndexHigh ? 4 : 0)));
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
                        preview[thirdIndex] |= byte2;
                    }
                }

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

        private void SetItemLevels(byte[] preview, ItemAppearance[] itemArray)
        {
            int levelindex = 0;
            for (int i = 0; i < 7; i++)
            {
                if (itemArray[i] != null)
                {
                    levelindex |= ((itemArray[i].Level - 1) / 2) << (i * 3);
                }
            }

            preview[6] = (byte)((levelindex >> 16) & 255);
            preview[7] = (byte)((levelindex >> 8) & 255);
            preview[8] = (byte)(levelindex & 255);
        }

        private void AddWing(byte[] preview, ItemAppearance wing)
        {
            if (wing == null)
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
            }

            switch ((WingIndex)wing.Definition.Number)
            {
                case WingIndex.WingsOfElf:
                    preview[9] |= 0x01;
                    break;
                case WingIndex.WingsOfHeaven:
                    preview[9] |= 0x02;
                    break;
                case WingIndex.WingsOfSatan:
                    preview[9] |= 0x03;
                    break;
                case WingIndex.WingsOfMistery:
                    preview[5] |= 0x04;
                    break;
                case WingIndex.WingsOfSpirit:
                    preview[9] |= 0x01;
                    break;
                case WingIndex.WingsOfSoul:
                    preview[9] |= 0x02;
                    break;
                case WingIndex.WingsOfDragon:
                    preview[9] |= 0x03;
                    break;
                case WingIndex.WingsOfDarkness:
                    preview[9] |= 0x04;
                    break;
                case WingIndex.CapeOfLord:
                    preview[9] |= 0x05;
                    break;
                case WingIndex.WingsOfDespair:
                    preview[9] |= 0x06;
                    break;
                case WingIndex.CapeOfFighter:
                    preview[9] |= 0x07;
                    break;
                case WingIndex.WingOfStorm:
                    preview[9] |= 0x01;
                    break;
                case WingIndex.WingOfEternal:
                    preview[9] |= 0x02;
                    break;
                case WingIndex.WingOfIllusion:
                    preview[9] |= 0x03;
                    break;
                case WingIndex.WingOfRuin:
                    preview[9] |= 0x04;
                    break;
                case WingIndex.CapeOfEmperor:
                    preview[9] |= 0x05;
                    break;
                case WingIndex.WingOfDimension:
                    preview[9] |= 0x06;
                    break;
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
            }
        }

        private void AddPet(byte[] preview, ItemAppearance pet)
        {
            if (pet == null)
            {
                preview[5] |= 0x03;
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
                    preview[5] = 0x03;
                    preview[12] |= 0x01;
                    break;
                case PetIndex.Fenrir:
                    preview[5] |= 0x03;
                    preview[10] &= 0xFE;
                    preview[12] &= 0xFE;
                    preview[12] |= 0x04;
                    preview[16] = 0x00;

                    // TODO: Red Fenrir: preview[16] |= 0x01;
                    // TODO: Blue Fenrir: preview[16] |= 0x02;
                    // TODO: Gold Fenrir: preview[17] |= 0x01;
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
}
