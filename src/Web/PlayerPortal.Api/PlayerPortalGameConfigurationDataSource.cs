// <copyright file="PlayerPortalGameConfigurationDataSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

using System.Collections;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Loads game configuration through the restricted, read-only configuration context.
/// The player portal must not require the administrative EntityDataContext connection.
/// </summary>
public sealed class PlayerPortalGameConfigurationDataSource(
    ILogger<PlayerPortalGameConfigurationDataSource> logger,
    IPersistenceContextProvider persistenceContextProvider)
    : DataSourceBase<GameConfiguration>(logger, persistenceContextProvider)
{
    /// <inheritdoc />
    protected override IReadOnlyDictionary<Type, Func<GameConfiguration, IEnumerable>> TypeToEnumerables
        => GameConfigurationHelper.Enumerables;

    /// <inheritdoc />
    protected override ValueTask<IContext> CreateNewContextAsync()
        => ValueTask.FromResult<IContext>(this.ContextProvider.CreateNewConfigurationContext());
}
