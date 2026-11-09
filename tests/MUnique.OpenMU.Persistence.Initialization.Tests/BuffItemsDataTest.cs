// <copyright file="BuffItemsDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the buff items of the in-game shop (seals, scrolls, elixirs and auras) of the season 6 data.
/// </summary>
[TestFixture]
internal class BuffItemsDataTest
{
    private static readonly (byte Group, short Number)[] BuffItems =
    [
        (13, 43), (13, 44), (13, 45), (13, 62), (13, 63), (13, 81), (13, 82), (13, 93), (13, 94), (13, 104), (13, 105),
        (14, 72), (14, 73), (14, 74), (14, 75), (14, 76), (14, 77), (14, 78), (14, 79), (14, 80), (14, 81), (14, 82),
        (14, 97), (14, 98), (14, 140),
    ];

    /// <summary>
    /// Tests that a new season 6 database contains the buff items with their effects,
    /// and that the products of the in-game shop are linked to them.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsBuffItemsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertBuffItems(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the buff items and their effects to a database which was created before they existed,
    /// links the products of the in-game shop, and that applying it twice doesn't add anything twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsBuffItemsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        RemoveBuffItems(gameConfiguration);
        var effectCount = gameConfiguration.MagicEffects.Count;

        var update = new AddBuffItemsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsBuffItem), Is.EqualTo(BuffItems.Length));
        Assert.That(gameConfiguration.MagicEffects, Has.Count.EqualTo(effectCount + BuffItems.Length));
        AssertBuffItems(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update leaves existing buff items unchanged.
    /// </summary>
    [Test]
    public async Task UpdateKeepsExistingBuffItemsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var items = gameConfiguration.Items.Where(IsBuffItem).ToList();
        var effectCount = gameConfiguration.MagicEffects.Count;
        var seal = items.Single(i => i is { Group: 13, Number: 43 });
        seal.Durability = 5;

        await new AddBuffItemsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Where(IsBuffItem), Is.EquivalentTo(items));
        Assert.That(gameConfiguration.MagicEffects, Has.Count.EqualTo(effectCount));
        Assert.That(seal.Durability, Is.EqualTo(5));
    }

    /// <summary>
    /// Tests that the update doesn't assign the items to the products of a custom in-game shop script,
    /// because its product sequences may identify other products.
    /// </summary>
    [Test]
    public async Task UpdateDoesNotAssignItemsToCustomScriptAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        RemoveBuffItems(gameConfiguration);
        var configuration = gameConfiguration.CashShopConfiguration!;
        configuration.ScriptYearId++;

        await new AddBuffItemsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsBuffItem), Is.EqualTo(BuffItems.Length));
        Assert.That(configuration.Packages.SelectMany(p => p.Products), Has.None.Matches<CashShopProduct>(p => p.ItemDefinition is { } item && IsBuffItem(item)));
    }

    /// <summary>
    /// Tests that the update adds the protection talismans and the attributes of their effects to a database
    /// which was created before they existed, and that applying it twice doesn't add anything twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsProtectionTalismansAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var attributes = new[] { Stats.DeathPenaltyProtection, Stats.ItemDurabilityProtection };
        foreach (var talisman in gameConfiguration.Items.Where(i => i is { Group: 13, Number: 81 or 82 }).ToList())
        {
            gameConfiguration.Items.Remove(talisman);
            gameConfiguration.MagicEffects.Remove(talisman.ConsumeEffect!);
        }

        foreach (var attribute in gameConfiguration.Attributes.Where(attributes.Contains).ToList())
        {
            gameConfiguration.Attributes.Remove(attribute);
        }

        var effectCount = gameConfiguration.MagicEffects.Count;

        var update = new AddProtectionTalismansUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.MagicEffects, Has.Count.EqualTo(effectCount + 2));
        Assert.That(gameConfiguration.Attributes.Count(attributes.Contains), Is.EqualTo(2));
        AssertBuffItems(gameConfiguration);
    }

    private static void RemoveBuffItems(GameConfiguration gameConfiguration)
    {
        foreach (var item in gameConfiguration.Items.Where(IsBuffItem).ToList())
        {
            gameConfiguration.Items.Remove(item);
            gameConfiguration.MagicEffects.Remove(item.ConsumeEffect!);
        }

        foreach (var product in gameConfiguration.CashShopConfiguration!.Packages.SelectMany(p => p.Products).Where(p => p.ItemDefinition is { } item && IsBuffItem(item)))
        {
            product.ItemDefinition = null;
        }
    }

    private static void AssertBuffItems(GameConfiguration gameConfiguration)
    {
        Assert.Multiple(() =>
        {
            AssertEffect(gameConfiguration, 13, 43, "Seal of Ascension", 1, 1, 0x1D, 24, 1800, (Stats.ExperienceRate, 1.5f, AggregateType.Multiplicate));
            AssertEffect(gameConfiguration, 13, 44, "Seal of Wealth", 1, 1, 0x1E, 24, 1800, (Stats.ExperienceRate, 2f, AggregateType.Multiplicate));
            AssertEffect(gameConfiguration, 13, 45, "Seal of Sustenance", 1, 1, 0x1F, 24, 1800, (Stats.ExperienceRate, 0f, AggregateType.Multiplicate), (Stats.MasterExperienceRate, 0f, AggregateType.Multiplicate), (Stats.BonusExperienceRate, 0f, AggregateType.Multiplicate));
            AssertEffect(gameConfiguration, 13, 62, "Seal of Healing", 1, 1, 87, 24, 1800, (Stats.ExperienceRate, 1.5f, AggregateType.Multiplicate), (Stats.HealthRecoveryMultiplier, 0.05f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 13, 63, "Seal of Divinity", 1, 1, 88, 13, 1800, (Stats.ManaRecoveryMultiplier, 0.05f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 13, 81, "Talisman of Guardian", 1, 1, 0x63, 77, 1800, (Stats.DeathPenaltyProtection, 1f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 13, 82, "Talisman of Item Protection", 1, 1, 0x64, 78, 1800, (Stats.ItemDurabilityProtection, 1f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 13, 93, "Master Seal of Ascension", 1, 1, 101, 24, 1800, (Stats.MasterExperienceRate, 1.5f, AggregateType.Multiplicate));
            AssertEffect(gameConfiguration, 13, 94, "Master Seal of Wealth", 1, 1, 102, 24, 1800, (Stats.MasterExperienceRate, 2f, AggregateType.Multiplicate));
            AssertEffect(gameConfiguration, 14, 72, "Scroll of Quickness", 2, 2, 0x2C, 67, 1800, (Stats.AttackSpeedAny, 15f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 14, 73, "Scroll of Defense", 2, 2, 0x2D, 68, 1800, (Stats.DefenseFinal, 30f, AggregateType.AddFinal));
            AssertEffect(gameConfiguration, 14, 74, "Scroll of Wrath", 2, 2, 0x2E, 69, 1800, (Stats.PhysicalBaseDmg, 30f, AggregateType.AddFinal));
            AssertEffect(gameConfiguration, 14, 75, "Scroll of Wizardry", 2, 2, 0x2F, 70, 1800, (Stats.WizardryBaseDmg, 30f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 14, 76, "Scroll of Health", 2, 2, 0x30, 71, 1800, (Stats.MaximumHealth, 200f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 14, 77, "Scroll of Mana", 2, 2, 0x31, 72, 1800, (Stats.MaximumMana, 300f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 14, 78, "Elixir of Strength", 1, 2, 0x32, 26, 864000, (Stats.TotalStrength, 50f, AggregateType.AddFinal));
            AssertEffect(gameConfiguration, 14, 79, "Elixir of Agility", 1, 2, 0x33, 26, 864000, (Stats.TotalAgility, 50f, AggregateType.AddFinal));
            AssertEffect(gameConfiguration, 14, 80, "Elixir of Health", 1, 2, 0x34, 26, 864000, (Stats.TotalVitality, 50f, AggregateType.AddFinal));
            AssertEffect(gameConfiguration, 14, 81, "Elixir of Energy", 1, 2, 0x35, 26, 864000, (Stats.TotalEnergy, 50f, AggregateType.AddFinal));
            AssertEffect(gameConfiguration, 14, 82, "Elixir of Control", 1, 2, 0x36, 26, 864000, (Stats.TotalLeadership, 50f, AggregateType.AddFinal));
            AssertEffect(gameConfiguration, 14, 97, "Scroll of Battle", 2, 2, 89, 14, 1800, (Stats.CriticalDamageChance, 0.2f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 14, 98, "Scroll of Strength", 2, 2, 90, 14, 1800, (Stats.ExcellentDamageChance, 0.2f, AggregateType.AddRaw));
            AssertEffect(gameConfiguration, 14, 140, "Scroll of Healing", 2, 2, 121, 75, 1800, (Stats.HealthRecoveryMultiplier, 0.03f, AggregateType.AddRaw), (Stats.MaximumHealth, 100f, AggregateType.AddRaw));
            AssertPerLevelEffect(gameConfiguration, 105, "Max SD Boost Aura", 114, 59, Stats.MaximumShield, 10f);
            AssertPerLevelEffect(gameConfiguration, 104, "Max AG Boost Aura", 113, 58, Stats.MaximumAbility, 1f);

            foreach (var item in gameConfiguration.Items.Where(IsBuffItem))
            {
                Assert.That(item.IsTradable, Is.False, item.Name.ValueInNeutralLanguage);
                Assert.That(item.IsDroppable, Is.False, item.Name.ValueInNeutralLanguage);
                Assert.That(item.IsSellableToNpc, Is.False, item.Name.ValueInNeutralLanguage);
                Assert.That(item.IsPersonalStoreSellable, Is.False, item.Name.ValueInNeutralLanguage);
            }

            var seal = gameConfiguration.Items.Single(i => i is { Group: 13, Number: 43 });
            Assert.That(seal.IsStorable, Is.False);
            var products = gameConfiguration.CashShopConfiguration!.Packages.SelectMany(p => p.Products).ToList();
            Assert.That(products.Where(p => p.ProductSequence == 9).Select(p => p.ItemDefinition), Has.All.SameAs(seal));
            Assert.That(products.Where(p => p.ProductSequence == 161).Select(p => p.ItemDefinition), Has.All.SameAs(gameConfiguration.Items.Single(i => i is { Group: 14, Number: 72 })));
        });
    }

    private static void AssertEffect(GameConfiguration gameConfiguration, byte group, short number, string name, byte width, byte height, short effectNumber, byte subType, float durationSeconds, params (AttributeDefinition Attribute, float Value, AggregateType AggregateType)[] boosts)
    {
        var effect = AssertItemAndGetEffect(gameConfiguration, group, number, name, width, height, effectNumber, subType, durationSeconds);
        Assert.That(effect.PowerUpDefinitions, Has.Count.EqualTo(boosts.Length), name);
        foreach (var (attribute, value, aggregateType) in boosts)
        {
            var powerUp = effect.PowerUpDefinitions.Single(p => p.TargetAttribute == attribute);
            Assert.That(powerUp.Boost!.ConstantValue.Value, Is.EqualTo(value).Within(0.0001f), $"{name}: {attribute.Designation}");
            Assert.That(powerUp.Boost.ConstantValue.AggregateType, Is.EqualTo(aggregateType), $"{name}: {attribute.Designation}");
        }
    }

    private static void AssertPerLevelEffect(GameConfiguration gameConfiguration, short number, string name, short effectNumber, byte subType, AttributeDefinition attribute, float valuePerLevel)
    {
        var effect = AssertItemAndGetEffect(gameConfiguration, 13, number, name, 1, 1, effectNumber, subType, 1800);
        var relation = effect.PowerUpDefinitions.Single(p => p.TargetAttribute == attribute).Boost!.RelatedValues.Single();
        Assert.That(relation.InputAttribute, Is.EqualTo(Stats.Level), name);
        Assert.That(relation.InputOperator, Is.EqualTo(InputOperator.Multiply), name);
        Assert.That(relation.InputOperand, Is.EqualTo(valuePerLevel), name);
    }

    private static MagicEffectDefinition AssertItemAndGetEffect(GameConfiguration gameConfiguration, byte group, short number, string name, byte width, byte height, short effectNumber, byte subType, float durationSeconds)
    {
        var item = gameConfiguration.Items.Single(i => i.Group == group && i.Number == number);
        Assert.That(item.Name.ValueInNeutralLanguage, Is.EqualTo(name));
        Assert.That((item.Width, item.Height), Is.EqualTo((width, height)), name);
        Assert.That(item.Durability, Is.EqualTo(1), name);
        var effect = item.ConsumeEffect!;
        Assert.That(effect, Is.Not.Null, name);
        Assert.That(gameConfiguration.MagicEffects, Contains.Item(effect), name);
        Assert.That(effect.Number, Is.EqualTo(effectNumber), name);
        Assert.That(effect.SubType, Is.EqualTo(subType), name);
        Assert.That(effect.Duration!.ConstantValue.Value, Is.EqualTo(durationSeconds), name);
        Assert.That(effect.SendDuration, Is.True, name);
        return effect;
    }

    private static bool IsBuffItem(ItemDefinition item)
    {
        return BuffItems.Contains((item.Group, item.Number));
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
