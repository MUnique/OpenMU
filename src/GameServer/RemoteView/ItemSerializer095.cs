// <copyright file="ItemSerializer095.cs" company="MUnique">
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

/// <summary>
/// This item serializer is used to serialize the item data to the data packets for version 0.95.
/// Each item is serialized into a 4-byte long part of an array.
/// </summary>
/// <remarks>
/// Bit pattern:
/// Index   Bits
/// 0       gggn nnnn
/// 1       slll lLoo
/// 2       Durability
/// 3       goxx xxxx
///
/// g = group
/// n = number
/// l = level
/// L = luck
/// s = skill
/// o = option level
/// x = exc option flags.
/// </remarks>
[Guid("4BD85C02-C43E-494D-B6B8-767ED94E09F0")]
[PlugIn(nameof(ItemSerializer095), "The item serializer for game client version 0.95")]
[MinimumClient(0, 95, ClientLanguage.Invariant)]
public class ItemSerializer095 : IItemSerializer
{
    private const byte LuckFlag = 4;

    private const byte SkillFlag = 128;

    private const byte LevelMask = 0x78;

    /// <inheritdoc/>
    public int NeededSpace => 4;

    /// <inheritdoc/>
    public int SerializeItem(Span<byte> target, Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        var itemType = (item.Definition.Number & 0x1F) | (item.Definition.Group << 5);
        target[0] = (byte)(itemType & 0xFF);
        if (itemType > 0xFF)
        {
            target[3] = 0x80;
        }

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

            target[1] |= (byte)(optionLevel & 3);
            target[3] |= (byte)((optionLevel & 4) << 4); // The highest bit is placed into the 2nd bit of the exc byte (0x40).
        }

        target[3] |= GetExcellentByte(item);

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck))
        {
            target[1] |= LuckFlag;
        }

        if (item.HasSkill)
        {
            target[1] |= SkillFlag;
        }

        target[2] = item.Durability();

        return this.NeededSpace;
    }

    /// <inheritdoc />
    public Item DeserializeItem(Span<byte> array, GameConfiguration gameConfiguration, IContext persistenceContext)
    {
        throw new NotImplementedException();
    }

    private static byte GetExcellentByte(Item item)
    {
        byte result = 0;
        var excellentOptions = item.ItemOptions.Where(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent);

        foreach (var option in excellentOptions)
        {
            result |= (byte)(1 << (option.ItemOption!.Number - 1));
        }

        return result;
    }
}