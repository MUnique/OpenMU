// <copyright file="AddMissingMerchantStoresPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update gives a merchant store to a merchant NPC which has a merchant window
/// but no items assigned, so talking to it opens an empty (non-working) shop.
/// </summary>
/// <remarks>
/// Affected NPC: Christine the General Goods Merchant (545) in the Loren Market. She is given a
/// clone of the general-goods (potion girl) store carried by Thompson the Merchant (231).
/// Two other NPCs in that window state are intentionally left out: Moss The Merchant (492) is the
/// gambler and Market Union Member Julia (547) is a warp NPC, so a general-goods store would not
/// fit either of them.
/// </remarks>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("f78d6e1d-1cb5-45f7-912d-54b2cb1220eb")]
public class AddMissingMerchantStoresPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add missing merchant stores";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Gives a general-goods store to Christine the General Goods Merchant (545), which had a shop window but no items, cloned from Thompson the Merchant (231).";

    /// <summary>
    /// The number of the NPC whose store is cloned for the empty merchants.
    /// </summary>
    private const short StoreSourceNpcNumber = 231; // Thompson the Merchant - carries the general-goods (potion girl) store

    /// <summary>
    /// The numbers of the NPCs which have a merchant window but no store.
    /// </summary>
    private static readonly short[] EmptyMerchantNpcNumbers = [545];

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddMissingMerchantStores;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 06, 17, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var storeSource = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == StoreSourceNpcNumber);
        if (storeSource?.MerchantStore is null)
        {
            return default;
        }

        foreach (var number in EmptyMerchantNpcNumbers)
        {
            var npc = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == number);
            if (npc is null || npc.MerchantStore is not null)
            {
                continue;
            }

            npc.MerchantStore = storeSource.MerchantStore.Clone(gameConfiguration);
            npc.MerchantStore.SetGuid(npc.Number);
        }

        return default;
    }
}
