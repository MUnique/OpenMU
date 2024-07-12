// <copyright file="PersistentObjectsLookupController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A <see cref="ILookupController"/> which will return persistent objects which
/// start with or contain the specified text.
/// </summary>
public class PersistentObjectsLookupController : ILookupController
{
    private readonly IPersistenceContextProvider _contextProvider;
    private readonly IDataSource<GameConfiguration> _gameConfigurationSource;
    private readonly ILogger<PersistentObjectsLookupController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentObjectsLookupController" /> class.
    /// </summary>
    /// <param name="contextProvider">The persistence context provider.</param>
    /// <param name="gameConfigurationSource">The game configuration provider.</param>
    /// <param name="logger">The logger.</param>
    public PersistentObjectsLookupController(
        IPersistenceContextProvider contextProvider,
        IDataSource<GameConfiguration> gameConfigurationSource,
        ILogger<PersistentObjectsLookupController> logger)
    {
        this._contextProvider = contextProvider;
        this._gameConfigurationSource = gameConfigurationSource;
        this._logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetSuggestionsAsync<T>(string? text, IContext? persistenceContext)
        where T : class
    {
        try
        {
            if (!typeof(T).IsConfigurationType())
            {
                // Only config data should be searchable
                return Enumerable.Empty<T>();
            }

            var owner = await this._gameConfigurationSource.GetOwnerAsync();
            IEnumerable<T> values;
            if (this._gameConfigurationSource.IsSupporting(typeof(T))
                && persistenceContext?.IsSupporting(typeof(T)) is not true)
            {
                values = this._gameConfigurationSource.GetAll<T>();
            }
            else
            {
                using var context = persistenceContext is null
                    ? this._contextProvider.CreateNewContext(owner)
                    : null;
                var effectiveContext = persistenceContext ?? context;
                if (effectiveContext is null)
                {
                    return Enumerable.Empty<T>();
                }

                values = await effectiveContext.GetAsync<T>().ConfigureAwait(false);
            }

            if (string.IsNullOrEmpty(text))
            {
                return values;
            }

            return values.Where(v => v.GetName().StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
                .Concat(values.Where(v => v.GetName().Contains(text, StringComparison.InvariantCultureIgnoreCase)))
                .Distinct();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error while searching for suggestions...");
        }

        return Enumerable.Empty<T>();
    }
}