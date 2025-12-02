// <copyright file="ItemSerializer.cs" company="MUnique">
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
/// At the moment, each item is serialized into a 12-byte long part of an array:
/// Byte Order: ItemCode Options Dura Exe Ancient Kind/380Opt HarmonyOpt Socket1 Socket2 Socket3 Socket4 Socket5.
/// </summary>
[Guid("3607902F-C7A8-40D0-823A-186F3BF630C7")]
[PlugIn("Item Serializer", "The default item serializer. It's most likely only correct for season 6.")]
[MinimumClient(5, 0, ClientLanguage.Invariant)]
public class ItemSerializer : IItemSerializer
{
    private const byte LuckFlag = 4;

    private const byte SkillFlag = 128;

    private const byte LevelMask = 0x78;

    private const byte GuardianOptionFlag = 0x08;

    private const byte AncientBonusLevelMask = 0b1100;
    private const byte AncientDiscriminatorMask = 0b0011;
    private const byte AncientMask = AncientBonusLevelMask | AncientDiscriminatorMask;

    /// <inheritdoc/>
    public int NeededSpace => 12;

    /// <inheritdoc/>
    public int SerializeItem(Span<byte> target, Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));
        target[0] = (byte)item.Definition.Number;

        var itemLevel = item.IsTrainablePet() ? 0 : item.Level;
        target[1] = (byte)((itemLevel << 3) & LevelMask);

        var itemOption = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option);
        if (itemOption != null)
        {
            var optionLevel = itemOption.Level;

            // A dinorant can normally have up to 2 options, all being coded in the item option level.
            // A one-option dino has level = 1, 2, or 4; a two-option has level = 3, 5, or 6.
            if (item.Definition.Skill?.Number == 49)
            {
                item.ItemOptions.Where(o => o.ItemOption?.OptionType == ItemOptionTypes.Option && o != itemOption)
                    .ForEach(o => optionLevel |= o.Level);
            }

            // The item option level is splitted into 2 parts. Webzen... :-/
            target[1] += (byte)(optionLevel & 3); // setting the first 2 bits
            target[3] = (byte)((optionLevel & 4) << 4); // The highest bit is placed into the 2nd bit of the exc byte (0x40).

            // Some items (wings) can have different options (3rd wings up to 3!)
            // Alternate options are set at array[startIndex + 3] |= 0x20 and 0x10
            if (itemOption.ItemOption?.Number > 0)
            {
                target[3] |= (byte)((itemOption.ItemOption.Number & 0b11) << 4);
            }
        }

        target[2] = item.Durability();

        target[3] |= GetExcellentByte(item);

        if ((item.Definition.Number & 0x100) == 0x100)
        {
            // Support for 512 items per Group
            target[3] |= 0x80;
        }

        target[3] |= GetFenrirByte(item);

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck))
        {
            target[1] |= LuckFlag;
        }

        if (item.HasSkill)
        {
            target[1] |= SkillFlag;
        }

        var ancientSet = item.ItemSetGroups.FirstOrDefault(set => set.AncientSetDiscriminator != 0);
        if (ancientSet != null)
        {
            target[4] |= (byte)(ancientSet.AncientSetDiscriminator & AncientDiscriminatorMask);

            // An ancient item may or may not have an ancient bonus option. Example without bonus: Gywen Pendant.
            var ancientBonus = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.AncientBonus);
            if (ancientBonus != null)
            {
                target[4] |= (byte)((ancientBonus.Level << 2) & AncientBonusLevelMask);
            }
        }

        target[5] = (byte)(item.Definition.Group << 4);
        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.GuardianOption))
        {
            target[5] |= GuardianOptionFlag;
        }

        target[6] = (byte)(GetHarmonyByte(item) | GetSocketBonusByte(item));
        SetSocketBytes(target.Slice(7, MaximumSockets), item);

        return this.NeededSpace;
    }

    /// <inheritdoc />
    public Item DeserializeItem(Span<byte> array, GameConfiguration gameConfiguration, IContext persistenceContext)
    {
        var itemNumber = array[0] + ((array[0] & 0x80) << 1);
        var itemGroup = (array[5] & 0xF0) >> 4;
        var definition = gameConfiguration.Items.FirstOrDefault(def => def.Number == itemNumber && def.Group == itemGroup)
                         ?? throw new ArgumentException($"Couldn't find the item definition for the given byte array. Extracted item number and group: {itemNumber}, {itemGroup}");

        var item = persistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Level = (byte)((array[1] & LevelMask) >> 3);
        item.Durability = array[2];

        if (item.Definition.PossibleItemOptions.Any(o =>
                o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Excellent)))
        {
            ReadExcellentOption(array[3], persistenceContext, item);
        }
        else if (item.Definition.PossibleItemOptions.Any(o =>
                     o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Wing)))
        {
            ReadWingOption(array[3], persistenceContext, item);
        }
        else
        {
            // set nothing.
        }

        ReadSkillFlag(array[1], item);
        ReadLuckOption(array[1], persistenceContext, item);
        ReadNormalOption(array, persistenceContext, item);
        ReadAncientOption(array[4], persistenceContext, item);
        ReadLevel380Option(array[5], persistenceContext, item);
        if (item.Definition.PossibleItemOptions.Any(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.BlackFenrir)))
        {
            ReadFenrirOptions(array[3], persistenceContext, item);
        }

        if (item.Definition.MaximumSockets == 0)
        {
            AddHarmonyOption(array[6], persistenceContext, item);
        }
        else
        {
            ReadSocketBonus(array[6], persistenceContext, item);
        }

        ReadSockets(array.Slice(7), persistenceContext, item);
        return item;
    }

    private static void ReadSkillFlag(byte optionByte, Item item)
    {
        if ((optionByte & SkillFlag) == 0)
        {
            return;
        }

        if (item.Definition!.Skill is null)
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

        AddLuckOption(persistenceContext, item);
    }

    private static void ReadWingOption(byte wingbyte, IContext persistenceContext, Item item)
    {
        var wingBits = wingbyte & 0x0F;
        ReadWingOptionBits(wingBits, persistenceContext, item);
    }

    private static void ReadExcellentOption(byte excByte, IContext persistenceContext, Item item)
    {
        var excellentBits = excByte & 0x3F;
        ReadExcellentOptionBits(excellentBits, persistenceContext, item);
    }

    private static void ReadNormalOption(Span<byte> array, IContext persistenceContext, Item item)
    {
        var optionLevel = (array[1] & 3) + ((array[3] >> 4) & 4);
        if (optionLevel == 0)
        {
            return;
        }

        var itemIsWing = item.Definition!.PossibleItemOptions.Any(o => o.PossibleOptions.Any(i => i.OptionType == ItemOptionTypes.Wing));
        var optionNumber = itemIsWing ? (array[3] >> 4) & 0b11 : 0;

        AddNormalOption(optionNumber, optionLevel, persistenceContext, item);
    }

    private static void ReadAncientOption(byte ancientByte, IContext persistenceContext, Item item)
    {
        if ((ancientByte & AncientMask) == 0)
        {
            return;
        }

        var bonusLevel = (ancientByte & AncientBonusLevelMask) >> 2;
        var setDiscriminator = ancientByte & AncientDiscriminatorMask;
        AddAncientOption(setDiscriminator, bonusLevel, persistenceContext, item);
    }

    private static void ReadLevel380Option(byte option380Byte, IContext persistenceContext, Item item)
    {
        if ((option380Byte & GuardianOptionFlag) == 0)
        {
            return;
        }

        AddLevel380Option(persistenceContext, item);
    }
}