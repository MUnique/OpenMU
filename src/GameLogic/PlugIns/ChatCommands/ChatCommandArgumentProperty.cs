// <copyright file="ChatCommandArgumentProperty.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// A writable property of an arguments class of a chat command.
/// </summary>
/// <param name="Name">The name of the property.</param>
/// <param name="PropertyType">The type of the property.</param>
/// <param name="Argument">The <see cref="ArgumentAttribute"/> of the property, if it has one.</param>
/// <param name="ValidValues">The <see cref="ValidValuesAttribute"/> of the property, if it has one.</param>
/// <param name="Range">The <see cref="RangeAttribute"/> of the property, if it has one.</param>
/// <param name="ValueReference">The <see cref="ValueReferenceAttribute"/> of the property, if it has one.</param>
/// <param name="SetValue">The action which sets the value of the property of an instance of the arguments class.</param>
public sealed record ChatCommandArgumentProperty(
    string Name,
    Type PropertyType,
    ArgumentAttribute? Argument,
    ValidValuesAttribute? ValidValues,
    RangeAttribute? Range,
    ValueReferenceAttribute? ValueReference,
    Action<object, object?> SetValue);
