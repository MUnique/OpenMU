// <copyright file="EnumerableLookupController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Collections;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A <see cref="ILookupController"/> which will return objects of an
/// <see cref="IEnumerable"/> whose name start with or contain the specified text.
/// </summary>
public class EnumerableLookupController : ILookupController
{
    private readonly IEnumerable _sourceEnumerable;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerableLookupController"/> class.
    /// </summary>
    /// <param name="sourceEnumerable">The source enumerable.</param>
    public EnumerableLookupController(IEnumerable sourceEnumerable)
    {
        this._sourceEnumerable = sourceEnumerable;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetSuggestionsAsync<T>(string text, IContext? persistenceContext)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return this._sourceEnumerable.OfType<T>();
        }

        return this._sourceEnumerable.OfType<T>().Where(v => v.GetName().StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
            .Concat(this._sourceEnumerable.OfType<T>().Where(v => v.GetName().Contains(text, StringComparison.InvariantCultureIgnoreCase)))
            .Distinct();
    }
}