// <copyright file="AccountDataSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Implementation of a <see cref="IDataSource{Account}"/>.
/// </summary>
public sealed class AccountDataSource : DataSourceBase<Account>
{
    private static readonly IReadOnlyDictionary<Type, Func<Account, IEnumerable>> Enumerables = new Dictionary<Type, Func<Account, IEnumerable>>
    {
        { typeof(Account), a => Enumerable.Repeat(a, 1) },
        { typeof(Character), a => a.Characters },
        { typeof(ItemStorage), a => a.Characters.Select(c => c.Inventory).Concat(Enumerable.Repeat(a.Vault, 1)).Where(i => i is not null) },
        { typeof(Item), a => a.Characters.SelectMany(c => c.Inventory?.Items ?? Enumerable.Empty<Item>()).Concat(a.Vault?.Items ?? Enumerable.Empty<Item>()) },
        { typeof(StatAttribute), a => a.Characters.SelectMany(c => c.Attributes) },
        { typeof(LetterHeader), a => a.Characters.SelectMany(c => c.Letters) },
        { typeof(SkillEntry), a => a.Characters.SelectMany(c => c.LearnedSkills) },
        { typeof(CharacterQuestState), a => a.Characters.SelectMany(c => c.QuestStates) },
        { typeof(QuestMonsterKillRequirementState), a => a.Characters.SelectMany(c => c.QuestStates).SelectMany(q => q.RequirementStates) },
    }.AsReadOnly();

    private readonly IDataSource<GameConfiguration> _gameConfigurationSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountDataSource"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="gameConfigurationSource">The game configuration source.</param>
    public AccountDataSource(
        ILogger<DataSourceBase<Account>> logger,
        IPersistenceContextProvider persistenceContextProvider,
        IDataSource<GameConfiguration> gameConfigurationSource)
        : base(logger, persistenceContextProvider)
    {
        _gameConfigurationSource = gameConfigurationSource;
    }

    /// <inheritdoc />
    protected override IReadOnlyDictionary<Type, Func<Account, IEnumerable>> TypeToEnumerables => Enumerables;

    /// <inheritdoc />
    protected override async ValueTask<IContext> CreateNewContextAsync()
    {
        var gameConfiguration = await _gameConfigurationSource.GetOwnerAsync(Guid.Empty).ConfigureAwait(false);
        return this.ContextProvider.CreateNewPlayerContext(gameConfiguration);
    }
}