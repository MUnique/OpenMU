// <copyright file="ModalParameters.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Modal;

using System.Collections;

/// <summary>
/// Parameters passed to a modal component.
/// </summary>
public class ModalParameters : IEnumerable<KeyValuePair<string, object?>>
{
    private readonly Dictionary<string, object?> _parameters = new();

    /// <summary>
    /// Adds a parameter.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    public void Add(string name, object? value)
    {
        this._parameters[name] = value;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => this._parameters.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this._parameters.GetEnumerator();
}
