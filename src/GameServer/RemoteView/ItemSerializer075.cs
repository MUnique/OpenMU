// <copyright file="ItemSerializer075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This item serializer is used to serialize the item data to the data packets for version 0.75.
/// Each item is serialized into a 3-byte long part of an array.
/// </summary>
[Guid("A97F30CF-A189-43A2-9271-D3E5A24CC3FD")]
[PlugIn("Item Serializer 0.75", "The item serializer for game client version 0.75")]
[MinimumClient(0, 75, ClientLanguage.Invariant)]
public class ItemSerializer075 : IItemSerializer
{
    private const byte LuckFlag = 4;

    private const byte SkillFlag = 128;

    private const byte LevelMask = 0x78;

    /// <inheritdoc/>
    public int NeededSpace => 3;

    /// <inheritdoc/>
    public int SerializeItem(Span<byte> target, Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));
        target[0] = (byte)(item.Definition.Number & 0x0F);
        target[0] |= (byte)(item.Definition.Group << 4);

        target[1] = (byte)((item.Level << 3) & LevelMask);

        var itemOption = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option);
        if (itemOption != null)
        {
            target[1] |= (byte)(itemOption.Level & 3);
        }

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
}