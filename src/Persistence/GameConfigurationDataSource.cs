// <copyright file="GameConfigurationDataSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Provider which provides the latest <see cref="GameConfiguration"/> and
/// it's child data.
/// </summary>
public sealed class GameConfigurationDataSource : DataSourceBase<GameConfiguration>
{
    private static readonly IReadOnlyDictionary<Type, Func<GameConfiguration, IEnumerable>> Enumerables = new Dictionary<Type, Func<GameConfiguration, IEnumerable>>
    {
        { typeof(GameConfiguration), c => Enumerable.Repeat(c, 1) },
        { typeof(ItemDefinition), c => c.Items },
        { typeof(ItemOptionDefinition), c => c.ItemOptions },
        {
            typeof(IncreasableItemOption), c => c.ItemOptions.SelectMany(o => o.PossibleOptions)
                .Concat(c.ItemSetGroups.SelectMany(o => o.Options))
        },
        { typeof(ItemSetGroup), c => c.ItemSetGroups },
        { typeof(ItemOfItemSet), c => c.ItemSetGroups.SelectMany(o => o.Items) },
        { typeof(ItemOptionType), c => c.ItemOptionTypes },
        { typeof(ItemSlotType), c => c.ItemSlotTypes },
        { typeof(AttributeDefinition), c => c.Attributes },
        { typeof(DropItemGroup), c => c.DropItemGroups },
        { typeof(CharacterClass), c => c.CharacterClasses },
        { typeof(JewelMix), c => c.JewelMixes },
        { typeof(MagicEffectDefinition), c => c.MagicEffects },
        { typeof(MasterSkillRoot), c => c.MasterSkillRoots },
        { typeof(GameMapDefinition), c => c.Maps },
        { typeof(MonsterDefinition), c => c.Monsters },
        { typeof(QuestDefinition), c => c.Monsters.SelectMany(m => m.Quests) },
        { typeof(Skill), c => c.Skills },
        { typeof(PlugInConfiguration), c => c.PlugInConfigurations },
    }.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameConfigurationDataSource"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    public GameConfigurationDataSource(ILogger<GameConfigurationDataSource> logger, IPersistenceContextProvider persistenceContextProvider)
        : base(logger, persistenceContextProvider)
    {
    }

    /// <inheritdoc />
    protected override IReadOnlyDictionary<Type, Func<GameConfiguration, IEnumerable>> TypeToEnumerables => Enumerables;
}