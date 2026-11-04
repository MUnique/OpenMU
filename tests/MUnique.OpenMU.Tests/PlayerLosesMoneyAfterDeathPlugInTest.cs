// <copyright file="PlayerLosesMoneyAfterDeathPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Vault;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="PlayerLosesMoneyAfterDeathPlugIn"/>.
/// </summary>
[TestFixture]
public class PlayerLosesMoneyAfterDeathPlugInTest
{
    private const int VaultMoney = 1000;
    private const int VaultLossPercentage = 10;

    /// <summary>
    /// A death removes the vault loss from the account vault, even when the vault is closed and locked,
    /// and the lost zen doesn't end up in the inventory.
    /// </summary>
    [Test]
    public async Task DeathRemovesVaultMoneyWhileVaultIsClosedAndLockedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Account = new BasicModel.Account
        {
            VaultPassword = "1234",
            Vault = new BasicModel.ItemStorage { Money = VaultMoney },
        };
        player.Money = 0;
        Assert.That(player.Vault, Is.Null);
        Assert.That(player.IsVaultLocked, Is.True);

        var plugIn = new PlayerLosesMoneyAfterDeathPlugIn
        {
            Configuration = new() { LossesPerLevel = [new(0, false, 0, VaultLossPercentage)] },
        };

        await plugIn.AttackableGotKilledAsync(player, await CreateMonsterAsync().ConfigureAwait(false)).ConfigureAwait(false);

        Assert.That(player.Account.Vault!.Money, Is.EqualTo(VaultMoney - (VaultMoney * VaultLossPercentage / 100)));
        Assert.That(player.Money, Is.Zero);
    }

    /// <summary>
    /// A death with the account vault open refreshes the vault money in the open window.
    /// </summary>
    [Test]
    public async Task DeathRefreshesOpenVaultMoneyAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Account = new BasicModel.Account { Vault = new BasicModel.ItemStorage { Money = VaultMoney } };
        player.Vault = new Storage(120, player.Account.Vault!);
        var plugIn = new PlayerLosesMoneyAfterDeathPlugIn
        {
            Configuration = new() { LossesPerLevel = [new(0, false, 0, VaultLossPercentage)] },
        };

        await plugIn.AttackableGotKilledAsync(player, await CreateMonsterAsync().ConfigureAwait(false)).ConfigureAwait(false);

        Assert.That(player.Vault.ItemStorage.Money, Is.EqualTo(VaultMoney - (VaultMoney * VaultLossPercentage / 100)));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateVaultMoneyPlugIn>()!).Verify(p => p.UpdateVaultMoneyAsync(true), Times.Once);
    }

    private static async ValueTask<Monster> CreateMonsterAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var map = await gameContext.GetMapAsync(0).ConfigureAwait(false);
        var definition = new BasicModel.MonsterDefinition { Id = Guid.NewGuid(), ObjectKind = NpcObjectKind.Monster };
        var spawnArea = new BasicModel.MonsterSpawnArea { MonsterDefinition = definition, GameMap = map!.Definition, Quantity = 1 };
        return new Monster(spawnArea, definition, map, NullDropGenerator.Instance, new Mock<INpcIntelligence>().Object, gameContext.PlugInManager, gameContext.PathFinderPool);
    }
}
