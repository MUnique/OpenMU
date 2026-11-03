// <copyright file="BuffItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using System.Linq.Expressions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initializer for the buff items of the cash shop: seals, scrolls, elixirs and auras.
/// When a player uses one, its <see cref="ItemDefinition.ConsumeEffect"/> is applied for a limited time.
/// </summary>
/// <remarks>
/// The effect values, durations, buff numbers and item sizes are the ones which the game client defines
/// for these items, so that the client shows the same values as the server applies.
/// Existing items aren't changed, so it can be used by an update, too.
/// The rules which block trading and selling them are applied by the <see cref="ItemRules"/>.
/// </remarks>
internal class BuffItems : InitializerBase
{
    /// <summary>
    /// The sub type of the effects of the seals which increase or block the gained experience.
    /// Only one of them can be active at the same time.
    /// </summary>
    private const byte ExperienceSealSubType = 24;

    /// <summary>
    /// The sub type of the effect of the seal of divinity.
    /// </summary>
    private const byte RecoverySealSubType = 13;

    /// <summary>
    /// The sub type of the effects of the elixirs. Only one of them can be active at the same time.
    /// </summary>
    private const byte ElixirSubType = 26;

    /// <summary>
    /// The sub type of the effects of the scrolls of battle and strength.
    /// </summary>
    private const byte DamageRateScrollSubType = 14;

    /// <summary>
    /// The defense which the game client shows for the scroll of defense.
    /// </summary>
    /// <remarks>
    /// The client adds it to the shown defense, which is the base defense. The final defense is half of the
    /// base defense, so the final defense increases by half of this value, like for the Jack O'Lantern Cry.
    /// </remarks>
    private const float ScrollOfDefenseBaseDefense = 60f;

    private static readonly TimeSpan SealDuration = TimeSpan.FromMinutes(30);

    private static readonly TimeSpan ScrollDuration = TimeSpan.FromMinutes(30);

    private static readonly TimeSpan AuraDuration = TimeSpan.FromMinutes(30);

    private static readonly TimeSpan ElixirDuration = TimeSpan.FromDays(10);

    /// <summary>
    /// Initializes a new instance of the <see cref="BuffItems"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BuffItems(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateItems();
    }

    /// <summary>
    /// Creates the buff items which don't exist yet, together with their effects.
    /// </summary>
    /// <returns>The created items.</returns>
    internal IReadOnlyList<ItemDefinition> CreateItems()
    {
        var items = new List<ItemDefinition?>
        {
            // Seals. Their experience rates only apply to the normal or to the master experience.
            // The seals which change the experience share a sub type, so they don't stack.
            this.CreateItem(13, 43, 1, 1, () => ItemNames.SealOfAscension, MagicEffectNumber.SealAscension, ExperienceSealSubType, SealDuration, false, Boost.Multiply(Stats.ExperienceRate, 1.5f)),
            this.CreateItem(13, 44, 1, 1, () => ItemNames.SealOfWealth, MagicEffectNumber.SealWealth, ExperienceSealSubType, SealDuration, false, Boost.Multiply(Stats.ExperienceRate, 2f)),
            this.CreateItem(13, 45, 1, 1, () => ItemNames.SealOfSustenance, MagicEffectNumber.SealSustenance, ExperienceSealSubType, SealDuration, false, Boost.Multiply(Stats.ExperienceRate, 0f), Boost.Multiply(Stats.MasterExperienceRate, 0f), Boost.Multiply(Stats.BonusExperienceRate, 0f)),
            this.CreateItem(13, 62, 1, 1, () => ItemNames.SealOfHealing, MagicEffectNumber.SealOfHealing, ExperienceSealSubType, SealDuration, false, Boost.Multiply(Stats.ExperienceRate, 1.5f), Boost.Add(Stats.HealthRecoveryMultiplier, 0.05f)),
            this.CreateItem(13, 63, 1, 1, () => ItemNames.SealOfDivinity, MagicEffectNumber.SealOfDivinity, RecoverySealSubType, SealDuration, false, Boost.Add(Stats.ManaRecoveryMultiplier, 0.05f)),
            this.CreateItem(13, 93, 1, 1, () => ItemNames.MasterSealOfAscension, MagicEffectNumber.MasterSealOfAscension, ExperienceSealSubType, SealDuration, false, Boost.Multiply(Stats.MasterExperienceRate, 1.5f)),
            this.CreateItem(13, 94, 1, 1, () => ItemNames.MasterSealOfWealth, MagicEffectNumber.MasterSealOfWealth, ExperienceSealSubType, SealDuration, false, Boost.Multiply(Stats.MasterExperienceRate, 2f)),

            // Auras, which increase the maximum by the character level.
            this.CreateItem(13, 104, 1, 1, () => ItemNames.MaxAgBoostAura, MagicEffectNumber.MaxAbilityBoostAura, 58, AuraDuration, false, Boost.PerLevel(Stats.MaximumAbility, 1f)),
            this.CreateItem(13, 105, 1, 1, () => ItemNames.MaxSdBoostAura, MagicEffectNumber.MaxShieldBoostAura, 59, AuraDuration, false, Boost.PerLevel(Stats.MaximumShield, 10f)),

            // Scrolls.
            this.CreateItem(14, 72, 2, 2, () => ItemNames.ScrollOfQuickness, MagicEffectNumber.ScrollOfQuickness, 67, ScrollDuration, true, Boost.Add(Stats.AttackSpeedAny, 15f)),
            this.CreateItem(14, 73, 2, 2, () => ItemNames.ScrollOfDefense, MagicEffectNumber.ScrollOfDefense, 68, ScrollDuration, true, Boost.AddFinal(Stats.DefenseFinal, ScrollOfDefenseBaseDefense / 2)),
            this.CreateItem(14, 74, 2, 2, () => ItemNames.ScrollOfWrath, MagicEffectNumber.ScrollOfWrath, 69, ScrollDuration, true, Boost.AddFinal(Stats.PhysicalBaseDmg, 30f)),
            this.CreateItem(14, 75, 2, 2, () => ItemNames.ScrollOfWizardry, MagicEffectNumber.ScrollOfWizardry, 70, ScrollDuration, true, Boost.Add(Stats.WizardryBaseDmg, 30f)),
            this.CreateItem(14, 76, 2, 2, () => ItemNames.ScrollOfHealth, MagicEffectNumber.ScrollOfHealth, 71, ScrollDuration, true, Boost.Add(Stats.MaximumHealth, 200f)),
            this.CreateItem(14, 77, 2, 2, () => ItemNames.ScrollOfMana, MagicEffectNumber.ScrollOfMana, 72, ScrollDuration, true, Boost.Add(Stats.MaximumMana, 300f)),
            this.CreateItem(14, 97, 2, 2, () => ItemNames.ScrollOfBattle, MagicEffectNumber.ScrollOfBattle, DamageRateScrollSubType, ScrollDuration, true, Boost.Add(Stats.CriticalDamageChance, 0.2f)),
            this.CreateItem(14, 98, 2, 2, () => ItemNames.ScrollOfStrength, MagicEffectNumber.ScrollOfStrength, DamageRateScrollSubType, ScrollDuration, true, Boost.Add(Stats.ExcellentDamageChance, 0.2f)),
            this.CreateItem(14, 140, 2, 2, () => ItemNames.ScrollOfHealing, MagicEffectNumber.ScrollOfHealing, 75, ScrollDuration, true, Boost.Add(Stats.HealthRecoveryMultiplier, 0.03f), Boost.Add(Stats.MaximumHealth, 100f)),

            // Elixirs.
            this.CreateItem(14, 78, 1, 2, () => ItemNames.ElixirOfStrength, MagicEffectNumber.ElixirOfStrength, ElixirSubType, ElixirDuration, false, Boost.AddFinal(Stats.TotalStrength, 50f)),
            this.CreateItem(14, 79, 1, 2, () => ItemNames.ElixirOfAgility, MagicEffectNumber.ElixirOfAgility, ElixirSubType, ElixirDuration, false, Boost.AddFinal(Stats.TotalAgility, 50f)),
            this.CreateItem(14, 80, 1, 2, () => ItemNames.ElixirOfHealth, MagicEffectNumber.ElixirOfVitality, ElixirSubType, ElixirDuration, false, Boost.AddFinal(Stats.TotalVitality, 50f)),
            this.CreateItem(14, 81, 1, 2, () => ItemNames.ElixirOfEnergy, MagicEffectNumber.ElixirOfEnergy, ElixirSubType, ElixirDuration, false, Boost.AddFinal(Stats.TotalEnergy, 50f)),
            this.CreateItem(14, 82, 1, 2, () => ItemNames.ElixirOfControl, MagicEffectNumber.ElixirOfLeadership, ElixirSubType, ElixirDuration, false, Boost.AddFinal(Stats.TotalLeadership, 50f)),
        };

        return items.OfType<ItemDefinition>().ToList();
    }

    private ItemDefinition? CreateItem(byte group, short number, byte width, byte height, Expression<Func<string>> name, MagicEffectNumber effectNumber, byte subType, TimeSpan duration, bool stopByDeath, params Boost[] boosts)
    {
        if (this.GameConfiguration.Items.Any(item => item.Group == group && item.Number == number))
        {
            return null;
        }

        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = group;
        item.Number = number;
        item.Name = LocalizedString.FromResource(name);
        item.Width = width;
        item.Height = height;
        item.Durability = 1;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);

        item.ConsumeEffect = this.CreateEffect(item, effectNumber, subType, duration, stopByDeath, boosts);
        return item;
    }

    private MagicEffectDefinition CreateEffect(ItemDefinition item, MagicEffectNumber effectNumber, byte subType, TimeSpan duration, bool stopByDeath, Boost[] boosts)
    {
        if (this.GameConfiguration.MagicEffects.FirstOrDefault(e => e.Number == (short)effectNumber) is { } existingEffect)
        {
            return existingEffect;
        }

        var effect = this.Context.CreateNew<MagicEffectDefinition>();
        effect.SetGuid(item.Number, (short)effectNumber);
        this.GameConfiguration.MagicEffects.Add(effect);
        effect.Name = item.Name;
        effect.Number = (short)effectNumber;
        effect.SubType = subType;
        effect.InformObservers = false;
        effect.SendDuration = true;
        effect.StopByDeath = stopByDeath;
        effect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        effect.Duration.ConstantValue.Value = (float)duration.TotalSeconds;

        foreach (var boost in boosts)
        {
            var powerUp = this.Context.CreateNew<PowerUpDefinition>();
            effect.PowerUpDefinitions.Add(powerUp);
            powerUp.TargetAttribute = boost.Target.GetPersistent(this.GameConfiguration);
            powerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            powerUp.Boost.ConstantValue.AggregateType = boost.AggregateType;
            if (boost.IsPerLevel)
            {
                var perLevel = this.Context.CreateNew<AttributeRelationship>();
                perLevel.InputAttribute = Stats.Level.GetPersistent(this.GameConfiguration);
                perLevel.InputOperator = InputOperator.Multiply;
                perLevel.InputOperand = boost.Value;
                perLevel.AggregateType = boost.AggregateType;
                powerUp.Boost.RelatedValues.Add(perLevel);
            }
            else
            {
                powerUp.Boost.ConstantValue.Value = boost.Value;
            }
        }

        return effect;
    }

    /// <summary>
    /// A boost of an effect.
    /// </summary>
    /// <param name="Target">The target attribute.</param>
    /// <param name="Value">The value, or the value per character level.</param>
    /// <param name="AggregateType">The aggregate type.</param>
    /// <param name="IsPerLevel">If set to <c>true</c>, the value is multiplied with the character level.</param>
    private sealed record Boost(AttributeDefinition Target, float Value, AggregateType AggregateType, bool IsPerLevel)
    {
        public static Boost Add(AttributeDefinition target, float value) => new(target, value, AggregateType.AddRaw, false);

        public static Boost AddFinal(AttributeDefinition target, float value) => new(target, value, AggregateType.AddFinal, false);

        public static Boost Multiply(AttributeDefinition target, float value) => new(target, value, AggregateType.Multiplicate, false);

        public static Boost PerLevel(AttributeDefinition target, float valuePerLevel) => new(target, valuePerLevel, AggregateType.AddRaw, true);
    }
}
