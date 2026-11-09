// <copyright file="LuckyCoinActionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.LuckyCoins;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Tests for the <see cref="LuckyCoinActions"/>.
/// </summary>
[TestFixture]
public class LuckyCoinActionsTest
{
    private readonly LuckyCoinActions _actions = new();

    /// <summary>
    /// A registration uses up one coin of a stack and counts it at the character.
    /// </summary>
    [Test]
    public async Task RegisterUsesUpOneCoinAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var coins = await AddCoinsAsync(player, 3).ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<ILuckyCoinViewPlugIn>()!);

        await this._actions.RegisterAsync(player).ConfigureAwait(false);
        await this._actions.RegisterAsync(player).ConfigureAwait(false);

        Assert.That(coins.Durability, Is.EqualTo(1));
        view.Verify(v => v.ShowRegistrationResultAsync(LuckyCoinRegistrationResult.Registered, 1), Times.Once);
        view.Verify(v => v.ShowRegistrationResultAsync(LuckyCoinRegistrationResult.Registered, 2), Times.Once);
        Assert.That(player.SelectedCharacter!.Attributes.Single(a => a.Definition!.Id == Stats.RegisteredLuckyCoins.Id).Value, Is.EqualTo(2));

        await this._actions.ShowRegisteredCountAsync(player).ConfigureAwait(false);
        view.Verify(v => v.ShowRegisteredCountAsync(2), Times.Once);
    }

    /// <summary>
    /// Without a coin, nothing is registered.
    /// </summary>
    [Test]
    public async Task RegisterWithoutCoinFailsAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<ILuckyCoinViewPlugIn>()!);

        await this._actions.RegisterAsync(player).ConfigureAwait(false);

        view.Verify(v => v.ShowRegistrationResultAsync(LuckyCoinRegistrationResult.NotEnoughItems, 0), Times.Once);
    }

    /// <summary>
    /// Nothing happens when the player doesn't talk to Delgado.
    /// </summary>
    [Test]
    public async Task RequiresOpenedDelgadoDialogAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var coins = await AddCoinsAsync(player, 3).ConfigureAwait(false);
        player.OpenedNpc = null;
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<ILuckyCoinViewPlugIn>()!);

        await this._actions.RegisterAsync(player).ConfigureAwait(false);
        await this._actions.ExchangeAsync(player, 10).ConfigureAwait(false);

        Assert.That(coins.Durability, Is.EqualTo(3));
        view.VerifyNoOtherCalls();
    }

    /// <summary>
    /// An exchange takes the coins from several stacks and adds the reward to the inventory.
    /// </summary>
    [Test]
    public async Task ExchangeGivesRewardAsync()
    {
        var player = await CreatePlayerAsync(new LuckyCoinReward { CoinCount = 10, ItemGroup = 14, ItemNumber = 13, ItemLevel = 0 }).ConfigureAwait(false);
        player.GameContext.Configuration.Items.Add(new ItemDefinition { Group = 14, Number = 13, Width = 1, Height = 1, Durability = 1 });
        var firstStack = await AddCoinsAsync(player, 6).ConfigureAwait(false);
        var secondStack = await AddCoinsAsync(player, 6).ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<ILuckyCoinViewPlugIn>()!);

        await this._actions.ExchangeAsync(player, 10).ConfigureAwait(false);

        view.Verify(v => v.ShowExchangeResultAsync(LuckyCoinExchangeResult.Exchanged), Times.Once);
        Assert.That(player.Inventory!.Items.Count(item => item.Definition is { Group: 14, Number: 13 }), Is.EqualTo(1));
        Assert.That(player.Inventory.Items.Where(item => item.Definition is { Group: 14, Number: 100 }).Sum(item => item.Durability), Is.EqualTo(2));
        Assert.That(new[] { firstStack, secondStack }.Count(stack => player.Inventory.Items.Contains(stack)), Is.EqualTo(1));
    }

    /// <summary>
    /// An exchange fails without enough coins, without a reward for the count of coins, and without space for the reward.
    /// The coins are kept then.
    /// </summary>
    [Test]
    public async Task ExchangeRequirementsAsync()
    {
        var player = await CreatePlayerAsync(new LuckyCoinReward { CoinCount = 10, ItemGroup = 14, ItemNumber = 13, ItemLevel = 0 }).ConfigureAwait(false);
        player.GameContext.Configuration.Items.Add(new ItemDefinition { Group = 14, Number = 13, Width = 10, Height = 10, Durability = 1 });
        var coins = await AddCoinsAsync(player, 9).ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<ILuckyCoinViewPlugIn>()!);

        await this._actions.ExchangeAsync(player, 10).ConfigureAwait(false);
        view.Verify(v => v.ShowExchangeResultAsync(LuckyCoinExchangeResult.NotEnoughItems), Times.Once);

        coins.Durability = 30;
        await this._actions.ExchangeAsync(player, 20).ConfigureAwait(false);
        view.Verify(v => v.ShowExchangeResultAsync(LuckyCoinExchangeResult.NotEnoughItems), Times.Exactly(2));

        await this._actions.ExchangeAsync(player, 10).ConfigureAwait(false);
        view.Verify(v => v.ShowExchangeResultAsync(LuckyCoinExchangeResult.InventoryFull), Times.Once);
        Assert.That(coins.Durability, Is.EqualTo(30));
    }

    private static async ValueTask<Player> CreatePlayerAsync(params LuckyCoinReward[] rewards)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.FeaturePlugIns.AddPlugIn(new LuckyCoinFeaturePlugIn { Configuration = new LuckyCoinConfiguration { Rewards = rewards.ToList() } }, true);
        player.OpenedNpc = new NonPlayerCharacter(null!, new MonsterDefinition { Number = 478, NpcWindow = NpcWindow.DelgadoLuckyCoinRegistration }, null!);
        return player;
    }

    private static async ValueTask<Item> AddCoinsAsync(Player player, int count)
    {
        var definition = player.GameContext.Configuration.Items.FirstOrDefault(item => item is { Group: 14, Number: 100 });
        if (definition is null)
        {
            definition = new ItemDefinition { Group = 14, Number = 100, Width = 1, Height = 1, Durability = 255 };
            player.GameContext.Configuration.Items.Add(definition);
        }

        var coins = player.PersistenceContext.CreateNew<Item>();
        coins.Definition = definition;
        coins.Durability = count;
        Assert.That(await player.Inventory!.AddItemAsync(coins).ConfigureAwait(false), Is.True);
        return coins;
    }
}
