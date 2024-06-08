// <copyright file="GameConfigurationHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.PlugIns;
using Nito.Disposables.Internals;

/// <summary>
/// Helper functions to access data of the <see cref="GameConfiguration"/> in a more generic way.
/// </summary>
public static class GameConfigurationHelper
{
    /// <summary>
    /// A dictionary which offers function for each possible configuration type
    /// included in a <see cref="GameConfiguration"/> composition root.
    /// </summary>
    public static readonly IReadOnlyDictionary<Type, Func<GameConfiguration, IEnumerable>> Enumerables = new Dictionary<Type, Func<GameConfiguration, IEnumerable>>
    {
        { typeof(GameConfiguration), c => Enumerable.Repeat(c, 1) },
        { typeof(ItemDefinition), c => c.Items },
        { typeof(ItemOptionDefinition), c => c.ItemOptions },
        {
            typeof(IncreasableItemOption), c => c.ItemOptions.SelectMany(o => o.PossibleOptions)
        },
        { typeof(ItemSetGroup), c => c.ItemSetGroups },
        { typeof(ItemOfItemSet), c => c.ItemSetGroups.SelectMany(o => o.Items) },
        { typeof(ItemLevelBonusTable), c => c.ItemLevelBonusTables },
        { typeof(LevelBonus), c => c.ItemLevelBonusTables.SelectMany(i => i.BonusPerLevel) },
        { typeof(ItemOptionCombinationBonus), c => c.ItemOptionCombinationBonuses },
        { typeof(CombinationBonusRequirement), c => c.ItemOptionCombinationBonuses.SelectMany(b => b.Requirements) },
        {
            typeof(PowerUpDefinition), c => c.ItemOptionCombinationBonuses.Select(i => i.Bonus).WhereNotNull()
                .Concat(c.ItemOptions.SelectMany(o => o.PossibleOptions.Select(p => p.PowerUpDefinition).WhereNotNull()))
                .Concat(c.ItemOptions.SelectMany(o => o.PossibleOptions.SelectMany(p => p.LevelDependentOptions.Select(l => l.PowerUpDefinition).WhereNotNull())))
                .Concat(c.MagicEffects.SelectMany(m => m.PowerUpDefinitions))
        },
        { typeof(ItemBasePowerUpDefinition), c => c.Items.SelectMany(i => i.BasePowerUpAttributes) },
        { typeof(ItemDropItemGroup), c => c.Items.SelectMany(i => i.DropItems) },
        { typeof(ItemOptionType), c => c.ItemOptionTypes },
        { typeof(ItemSlotType), c => c.ItemSlotTypes },
        { typeof(AttributeDefinition), c => c.Attributes },
        { typeof(StatAttributeDefinition), c => c.CharacterClasses.SelectMany(a => a.StatAttributes) },
        {
            typeof(AttributeRelationship), c => c.CharacterClasses.SelectMany(a => a.AttributeCombinations)
                .Concat(Enumerables![typeof(PowerUpDefinitionValue)](c).OfType<PowerUpDefinitionValue>().SelectMany(v => v.RelatedValues))
        },
        { typeof(ConstValueAttribute), c => c.CharacterClasses.SelectMany(a => a.BaseAttributeValues) },
        { typeof(SkillComboDefinition), c => c.CharacterClasses.Select(a => a.ComboDefinition).WhereNotNull() },
        { typeof(SkillComboStep), c => c.CharacterClasses.Select(a => a.ComboDefinition).WhereNotNull().SelectMany(d => d.Steps) },
        { typeof(DropItemGroup), c => c.DropItemGroups },
        { typeof(CharacterClass), c => c.CharacterClasses },
        { typeof(JewelMix), c => c.JewelMixes },
        { typeof(MagicEffectDefinition), c => c.MagicEffects },
        {
            typeof(PowerUpDefinitionValue), c => c.MagicEffects.Select(e => e.Duration).WhereNotNull()
                .Concat(Enumerables![typeof(PowerUpDefinition)](c).OfType<PowerUpDefinition>().Select(p => p.Boost).WhereNotNull())
        },
        { typeof(SimpleElement), c => Enumerables![typeof(PowerUpDefinitionValue)](c).OfType<PowerUpDefinitionValue>().Select(v => v.ConstantValue).WhereNotNull() },
        { typeof(MasterSkillRoot), c => c.MasterSkillRoots },
        { typeof(GameMapDefinition), c => c.Maps },
        { typeof(ExitGate), c => c.Maps.SelectMany(m => m.ExitGates) },
        { typeof(EnterGate), c => c.Maps.SelectMany(m => m.EnterGates) },
        { typeof(BattleZoneDefinition), c => c.Maps.Select(m => m.BattleZone).WhereNotNull() },
        {
            typeof(MonsterSpawnArea), c => c.Maps.SelectMany(m => m.MonsterSpawns)
                .Concat(c.MiniGameDefinitions.SelectMany(m => m.ChangeEvents).Select(e => e.SpawnArea).WhereNotNull())
        },
        {
            typeof(AttributeRequirement), c => c.Maps.SelectMany(m => m.MapRequirements)
                .Concat(c.Items.SelectMany(item => item.Requirements))
                .Concat(c.Skills.SelectMany(skill => skill.Requirements))
                .Concat(c.Skills.SelectMany(skill => skill.ConsumeRequirements))
        },
        { typeof(MonsterDefinition), c => c.Monsters },
        { typeof(ItemStorage), c => c.Monsters.Select(m => m.MerchantStore).WhereNotNull() },
        { typeof(ItemCrafting), c => c.Monsters.SelectMany(m => m.ItemCraftings) },
        { typeof(MonsterAttribute), c => c.Monsters.SelectMany(m => m.Attributes) },
        { typeof(QuestDefinition), c => c.Monsters.SelectMany(m => m.Quests) },
        { typeof(Skill), c => c.Skills },
        { typeof(WarpInfo), c => c.WarpList },
        { typeof(PlugInConfiguration), c => c.PlugInConfigurations },
        { typeof(MiniGameDefinition), c => c.MiniGameDefinitions },
        { typeof(MiniGameReward), c => c.MiniGameDefinitions.SelectMany(m => m.Rewards) },
        { typeof(MiniGameSpawnWave), c => c.MiniGameDefinitions.SelectMany(m => m.SpawnWaves) },
        { typeof(MiniGameChangeEvent), c => c.MiniGameDefinitions.SelectMany(m => m.ChangeEvents) },
        { typeof(MiniGameTerrainChange), c => c.MiniGameDefinitions.SelectMany(m => m.ChangeEvents).SelectMany(e => e.TerrainChanges) },
    }.AsReadOnly();

    /// <summary>
    /// Gets the objects of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the wanted objects.</typeparam>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The objects of the specified type.</returns>
    public static IEnumerable<T> GetObjectsOf<T>(this GameConfiguration gameConfiguration)
    {
        if (Enumerables.TryGetValue(typeof(T), out var enumerableGetter))
        {
            return enumerableGetter(gameConfiguration).OfType<T>();
        }

        return Enumerable.Empty<T>();
    }

    /// <summary>
    /// Gets the objects of the specified type.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="type">The type of the wanted objects.</param>
    /// <returns>The objects of the specified type.</returns>
    public static IEnumerable<object> GetObjectsOf(this GameConfiguration gameConfiguration, Type type)
    {
        if (Enumerables.TryGetValue(type, out var enumerableGetter))
        {
            return enumerableGetter(gameConfiguration).OfType<object>();
        }

        return Enumerable.Empty<object>();
    }

    /// <summary>
    /// Gets the instance of <paramref name="other"/> which is referenced within
    /// the <paramref name="gameConfiguration"/> based on their equality.
    /// </summary>
    /// <typeparam name="T">The type of object.</typeparam>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="other">The other instance of the same object, which may not
    /// be referenced by the <paramref name="gameConfiguration"/>.</param>
    /// <returns>
    /// The instance of <paramref name="other" /> which is referenced within the
    /// <paramref name="gameConfiguration" /> based on their equality.
    /// </returns>
    public static T? GetObjectOfConfig<T>(this GameConfiguration gameConfiguration, T? other)
        where T : class
    {
        if (other is null)
        {
            return null;
        }

        var result = gameConfiguration.GetObjectsOf<T>().FirstOrDefault(o => object.Equals(o, other));
        return result;
    }

    /// <summary>
    /// Gets the instance of <paramref name="other" /> which is referenced within
    /// the <paramref name="gameConfiguration" /> based on their equality.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="other">The other instance of the same object, which may not
    /// be referenced by the <paramref name="gameConfiguration" />.</param>
    /// <returns>
    /// The instance of <paramref name="other" /> which is referenced within the
    /// <paramref name="gameConfiguration" /> based on their equality.
    /// </returns>
    public static object? GetObjectOfConfig(this GameConfiguration gameConfiguration, object? other)
    {
        if (other is null)
        {
            return null;
        }

        var result = gameConfiguration.GetObjectsOf(other.GetType().BaseType!).FirstOrDefault(o => object.Equals(o, other))
            ?? gameConfiguration.GetObjectsOf(other.GetType()).FirstOrDefault(o => object.Equals(o, other));
        return result;
    }
}
