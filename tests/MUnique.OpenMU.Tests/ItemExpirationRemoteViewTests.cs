// <copyright file="ItemExpirationRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameServer.RemoteView.Inventory;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the packets of the <see cref="ItemExpirationViewPlugIn"/>.
/// </summary>
[TestFixture]
public class ItemExpirationRemoteViewTests
{
    private static readonly DateTime ExpiresAt = new(2026, 11, 20, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Tests that the list sends the count (D2 11) and one period item info (D2 12) per time-limited item, with the
    /// slot at offset 6 and the expiration in Unix seconds at offset 8, as the client reads them.
    /// </summary>
    [Test]
    public async Task ListOfExpirationsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new ItemExpirationViewPlugIn(player);
        var pet = new BasicModel.Item { ItemSlot = 8, ExpiresAt = ExpiresAt };
        var permanent = new BasicModel.Item { ItemSlot = 12 };
        var ring = new BasicModel.Item { ItemSlot = 20, ExpiresAt = ExpiresAt.AddDays(1) };

        await view.ShowExpirationsAsync(new Item[] { pet, permanent, ring }).ConfigureAwait(false);

        var seconds = (uint)new DateTimeOffset(ExpiresAt).ToUnixTimeSeconds();
        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(4 + 12 + 12));
        Assert.That(data[..4], Is.EqualTo(new byte[] { 0xC1, 4, 0xD2, 0x11 }));
        AssertPeriodItemInfo(data[4..16], 8, seconds);
        AssertPeriodItemInfo(data[16..], 20, seconds + 86400);
    }

    /// <summary>
    /// Tests that nothing is sent when no item is time-limited, neither in the list nor for a single item.
    /// </summary>
    [Test]
    public async Task NothingForPermanentItemsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new ItemExpirationViewPlugIn(player);
        var permanent = new BasicModel.Item { ItemSlot = 12 };

        await view.ShowExpirationsAsync(new Item[] { permanent }).ConfigureAwait(false);
        await view.ShowExpirationAsync(permanent).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.Empty);
    }

    /// <summary>
    /// Tests that a single time-limited item gets its period item info.
    /// </summary>
    [Test]
    public async Task SingleExpirationAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new ItemExpirationViewPlugIn(player);

        await view.ShowExpirationAsync(new BasicModel.Item { ItemSlot = 9, ExpiresAt = ExpiresAt }).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(12));
        AssertPeriodItemInfo(data, 9, (uint)new DateTimeOffset(ExpiresAt).ToUnixTimeSeconds());
    }

    /// <summary>
    /// Asserts the header and the fields of a period item info. The bytes 4 and 5 are padding of the client structure.
    /// </summary>
    private static void AssertPeriodItemInfo(byte[] data, ushort slot, uint seconds)
    {
        Assert.That(data[..4], Is.EqualTo(new byte[] { 0xC1, 12, 0xD2, 0x12 }));
        Assert.That(BitConverter.ToUInt16(data, 6), Is.EqualTo(slot));
        Assert.That(BitConverter.ToUInt32(data, 8), Is.EqualTo(seconds));
    }
}
