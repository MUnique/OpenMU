// <copyright file="ValueReferenceAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Describes what kind of object the value of an argument property refers to,
/// so that a user interface can offer a fitting picker for it.
/// </summary>
/// <remarks>
/// It's purely descriptive. The parsing and validation of the arguments doesn't depend on it.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class ValueReferenceAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueReferenceAttribute"/> class.
    /// </summary>
    /// <param name="kind">The kind of object which the value refers to.</param>
    public ValueReferenceAttribute(ChatCommandValueReference kind)
    {
        this.Kind = kind;
    }

    /// <summary>
    /// Gets the kind of object which the value refers to.
    /// </summary>
    public ChatCommandValueReference Kind { get; }

    /// <summary>
    /// Gets or sets the name of another argument property, which identifies the object
    /// together with this one. For example, an item is identified by its group and its number,
    /// so that a user interface can offer one item picker which fills both parameters.
    /// </summary>
    public string? GroupWith { get; set; }
}
