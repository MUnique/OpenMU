// <copyright file="GladiatorsHonorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.GameLogic.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests the plugins of the Gladiator's Honor: the <see cref="GladiatorsHonorConsumeHandlerPlugIn"/>
/// and the <see cref="VulcanusExperiencePlugIn"/>.
/// </summary>
[TestFixture]
public class GladiatorsHonorTest
{
    private const byte ItemSlot = 12;

    private const float ExperienceRate = 0.2f;

    /// <summary>
    /// Tests that the item can't be used outside of Vulcanus, so it isn't used up.
    /// </summary>
    [Test]
    public async Task CanNotBeUsedOutsideOfVulcanusAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var item = await AddItemAsync(player).ConfigureAwait(false);

        var consumed = await new GladiatorsHonorConsumeHandlerPlugIn().ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.False);
        Assert.That(player.Inventory!.GetItem(ItemSlot), Is.SameAs(item));
        Assert.That(player.Attributes![Stats.VulcanusExperienceRate], Is.Zero);
    }

    /// <summary>
    /// Tests that the item applies its effect in the configured map and is used up.
    /// </summary>
    [Test]
    public async Task AppliesEffectInVulcanusAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var item = await AddItemAsync(player).ConfigureAwait(false);
        var plugIn = new GladiatorsHonorConsumeHandlerPlugIn
        {
            Configuration = new VulcanusMapConfiguration { MapNumber = (short)player.CurrentMap!.MapId },
        };

        var consumed = await plugIn.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(item.Durability, Is.Zero);
        Assert.That(player.Attributes![Stats.VulcanusExperienceRate], Is.EqualTo(ExperienceRate).Within(0.001f));
    }

    /// <summary>
    /// Tests that the experience is increased by the rate in Vulcanus.
    /// </summary>
    [Test]
    public async Task IncreasesExperienceInVulcanusAsync()
    {
        var player = await CreatePlayerWithEffectAsync().ConfigureAwait(false);
        var plugIn = new VulcanusExperiencePlugIn
        {
            Configuration = new VulcanusMapConfiguration { MapNumber = (short)player.CurrentMap!.MapId },
        };
        var args = new ExperienceCalculationArgs(Mock.Of<IAttackable>(), false, 1000);

        await plugIn.CalculateExperienceAsync(player, args).ConfigureAwait(false);

        Assert.That(args.Experience, Is.EqualTo(1200).Within(0.01));
    }

    /// <summary>
    /// Tests that the experience isn't increased outside of Vulcanus.
    /// </summary>
    [Test]
    public async Task DoesNotIncreaseExperienceOutsideOfVulcanusAsync()
    {
        var player = await CreatePlayerWithEffectAsync().ConfigureAwait(false);
        var args = new ExperienceCalculationArgs(Mock.Of<IAttackable>(), false, 1000);

        await new VulcanusExperiencePlugIn().CalculateExperienceAsync(player, args).ConfigureAwait(false);

        Assert.That(args.Experience, Is.EqualTo(1000));
    }

    private static async ValueTask<Player> CreatePlayerWithEffectAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var item = await AddItemAsync(player).ConfigureAwait(false);
        var plugIn = new GladiatorsHonorConsumeHandlerPlugIn
        {
            Configuration = new VulcanusMapConfiguration { MapNumber = (short)player.CurrentMap!.MapId },
        };
        Assert.That(await plugIn.ConsumeItemAsync(player, item, null, FruitUsage.Undefined).ConfigureAwait(false), Is.True);
        return player;
    }

    private static async ValueTask<Item> AddItemAsync(Player player)
    {
        var effect = new BasicModel.MagicEffectDefinition
        {
            Number = 0x67,
            SubType = 79,
            Duration = new BasicModel.PowerUpDefinitionValue { ConstantValue = { Value = 1800 } },
        };
        effect.PowerUpDefinitions.Add(new BasicModel.PowerUpDefinition
        {
            TargetAttribute = Stats.VulcanusExperienceRate,
            Boost = new BasicModel.PowerUpDefinitionValue { ConstantValue = { Value = ExperienceRate } },
        });
        var definition = new BasicModel.ItemDefinition { Group = 13, Number = 95, Width = 1, Height = 1, Durability = 1, ConsumeEffect = effect };
        var item = new BasicModel.Item { Definition = definition, Durability = 1 };
        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        return item;
    }
}
