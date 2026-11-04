// <copyright file="ValidValuesAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Describes valid values for string argument properties.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValidValuesAttribute : Attribute
{
    /// <summary>
    /// The valid values.
    /// </summary>
    private readonly ISet<string> _validValues;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidValuesAttribute"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public ValidValuesAttribute(params string[] values)
    {
        this._validValues = new SortedSet<string>(values);
    }

    /// <summary>
    /// Gets the valid values.
    /// </summary>
    public IEnumerable<string> ValidValues => this._validValues;
}