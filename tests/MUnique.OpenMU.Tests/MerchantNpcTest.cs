// <copyright file="MerchantNpcTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="MerchantNpc"/>.
/// </summary>
[TestFixture]
public class MerchantNpcTest
{
    /// <summary>
    /// An item of the store which finds no place, e.g. because the size of its definition grew,
    /// doesn't fail the creation of the merchant, and stays in the store.
    /// </summary>
    [Test]
    public void ItemWithoutPlaceDoesNotFailCreation()
    {
        var store = new BasicModel.ItemStorage();
        var fittingItem = new BasicModel.Item
        {
            ItemSlot = 0,
            Definition = new BasicModel.ItemDefinition { Width = 1, Height = 1 },
        };
        var tooWideItem = new BasicModel.Item
        {
            ItemSlot = 8,
            Definition = new BasicModel.ItemDefinition { Width = (byte)(InventoryConstants.RowSize + 1), Height = 1 },
        };
        store.Items.Add(fittingItem);
        store.Items.Add(tooWideItem);
        var merchantDefinition = new MonsterDefinition { MerchantStore = store };
        var map = new GameMap(new GameMapDefinition(), TimeSpan.FromSeconds(60), 8);

        MerchantNpc? merchant = null;
        Assert.That(() => merchant = new MerchantNpc(new MonsterSpawnArea(), merchantDefinition, map), Throws.Nothing);

        Assert.That(merchant!.MerchantStorage.Items, Is.EquivalentTo(new[] { fittingItem }));
        Assert.That(store.Items, Does.Contain(tooWideItem));
    }
}
