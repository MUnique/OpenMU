// <copyright file="PersistentObjectsLookupController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using MUnique.OpenMU.Persistence;

/// <summary>
/// A lookup controller which will return persistent objects which start with or contain the specified text.
/// </summary>
/// <seealso cref="ILookupController" />
public class PersistentObjectsLookupController : ILookupController
{
    private readonly IPersistenceContextProvider _contextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentObjectsLookupController"/> class.
    /// </summary>
    /// <param name="contextProvider">The persistence context provider.</param>
    public PersistentObjectsLookupController(IPersistenceContextProvider contextProvider)
    {
        this._contextProvider = contextProvider;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetSuggestionsAsync<T>(string? text, IContext? persistenceContext)
        where T : class
    {
        if (!typeof(T).IsConfigurationType())
        {
            // Only config data should be searchable
            return Enumerable.Empty<T>();
        }

        using var context = persistenceContext is null ? this._contextProvider.CreateNewTypedContext<T>() : null;
        var effectiveContext = persistenceContext ?? context;
        if (effectiveContext is null)
        {
            return Enumerable.Empty<T>();
        }

        var values = await effectiveContext.GetAsync<T>().ConfigureAwait(false);

        if (string.IsNullOrEmpty(text))
        {
            return values;
        }

        return values.Where(v => v.GetName().StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
            .Concat(values.Where(v => v.GetName().Contains(text, StringComparison.InvariantCultureIgnoreCase)))
            .Distinct();
    }
}