// <copyright file="GameConfigurationDataSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Provider which provides the latest <see cref="GameConfiguration"/> and
/// it's child data.
/// </summary>
public sealed class GameConfigurationDataSource : DataSourceBase<GameConfiguration>
{
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
    protected override IReadOnlyDictionary<Type, Func<GameConfiguration, IEnumerable>> TypeToEnumerables => GameConfigurationHelper.Enumerables;
}