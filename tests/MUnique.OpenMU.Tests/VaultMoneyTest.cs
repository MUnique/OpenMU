// <copyright file="VaultMoneyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.MessageHandler.Vault;
using MUnique.OpenMU.Network.Packets.ClientToServer;

/// <summary>
/// Tests for moving money between the inventory and the vault.
/// </summary>
[TestFixture]
public class VaultMoneyTest
{
    private const int VaultMoney = 1000;
    private const int PlayerMoney = 50;

    /// <summary>
    /// Tests that the packet handler ignores a deposit amount which doesn't fit into an <see cref="int"/>.
    /// </summary>
    [Test]
    public async Task HandlerIgnoresOversizedDepositAsync()
    {
        var player = await CreatePlayerWithVaultAsync().ConfigureAwait(false);
        player.IsVaultLocked = true;

        await HandleAsync(player, VaultMoveMoneyRequest.VaultMoneyMoveDirection.InventoryToVault, uint.MaxValue).ConfigureAwait(false);

        AssertBalances(player, VaultMoney, PlayerMoney);
    }

    /// <summary>
    /// Tests that the packet handler ignores a take amount which doesn't fit into an <see cref="int"/>.
    /// </summary>
    [Test]
    public async Task HandlerIgnoresOversizedTakeAsync()
    {
        var player = await CreatePlayerWithVaultAsync().ConfigureAwait(false);
        player.IsVaultLocked = false;

        await HandleAsync(player, VaultMoveMoneyRequest.VaultMoneyMoveDirection.VaultToInventory, uint.MaxValue).ConfigureAwait(false);

        AssertBalances(player, VaultMoney, PlayerMoney);
    }

    /// <summary>
    /// Tests that a deposit which would push the vault money beyond <see cref="int.MaxValue"/> is refused without losing money.
    /// </summary>
    [Test]
    public async Task DepositBeyondIntMaxValueIsRefusedAsync()
    {
        var player = await CreatePlayerWithVaultAsync().ConfigureAwait(false);
        player.Vault!.ItemStorage.Money = int.MaxValue - 10;

        Assert.That(player.TryDepositVaultMoney(PlayerMoney), Is.False);
        AssertBalances(player, int.MaxValue - 10, PlayerMoney);
    }

    /// <summary>
    /// Tests that a take which would push the inventory money beyond <see cref="int.MaxValue"/> is refused without losing money.
    /// </summary>
    [Test]
    public async Task TakeBeyondIntMaxValueIsRefusedAsync()
    {
        var player = await CreatePlayerWithVaultAsync().ConfigureAwait(false);
        player.IsVaultLocked = false;
        player.Money = int.MaxValue - 10;

        Assert.That(player.TryTakeVaultMoney(VaultMoney), Is.False);
        AssertBalances(player, VaultMoney, int.MaxValue - 10);
    }

    private static async ValueTask HandleAsync(Player player, VaultMoveMoneyRequest.VaultMoneyMoveDirection direction, uint amount)
    {
        var packet = new byte[VaultMoveMoneyRequest.Length];
        _ = new VaultMoveMoneyRequest(packet) { Direction = direction, Amount = amount };
        await new VaultMoneyHandlerPlugIn().HandlePacketAsync(player, packet).ConfigureAwait(false);
    }

    private static void AssertBalances(Player player, int vaultMoney, int playerMoney)
    {
        Assert.That(player.Vault!.ItemStorage.Money, Is.EqualTo(vaultMoney));
        Assert.That(player.Money, Is.EqualTo(playerMoney));
    }

    private static async ValueTask<Player> CreatePlayerWithVaultAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        player.GameContext.Configuration.MaximumVaultMoney = int.MaxValue;
        player.Money = PlayerMoney;

        var itemStorage = new Mock<ItemStorage>();
        itemStorage.SetupAllProperties();
        itemStorage.Setup(i => i.Items).Returns(new List<Item>());
        itemStorage.Object.Money = VaultMoney;
        player.Vault = new Storage(InventoryConstants.WarehouseSize, itemStorage.Object);
        return player;
    }
}
