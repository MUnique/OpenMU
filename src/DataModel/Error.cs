// <copyright file="Error.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Static class which offers convenience methods to throw exceptions.
/// </summary>
public static class Error
{
    /// <summary>
    /// Creates an <see cref="InvalidOperationException"/> because of on uninitialized property.
    /// </summary>
    /// <param name="parent">The parent object whose property is not initialized.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The created <see cref="InvalidOperationException"/>.</returns>
    public static Exception NotInitializedProperty(object parent, [CallerMemberName] string propertyName = "")
    {
        return new InvalidOperationException($"Property '{propertyName}' of {parent} is not initialized yet.");
    }

    /// <summary>
    /// Creates and throws an <see cref="InvalidOperationException"/> because of on uninitialized property.
    /// </summary>
    /// <param name="parent">The parent object whose property is not initialized.</param>
    /// <param name="propertyName">The property name.</param>
    [DoesNotReturn]
    public static void ThrowNotInitializedProperty(this object parent, string propertyName)
    {
        throw new InvalidOperationException($"Property '{propertyName}' of {parent} is not initialized yet.");
    }

    /// <summary>
    /// Creates and throws an <see cref="InvalidOperationException"/> if <paramref name="propertyIsNull"/> is <see langword="true"/>.
    /// </summary>
    /// <param name="parent">The parent object whose property is not initialized.</param>
    /// <param name="propertyIsNull">The flag, if the property is null.</param>
    /// <param name="propertyName">The property name.</param>
    public static void ThrowNotInitializedProperty(this object parent, [DoesNotReturnIf(true)] bool propertyIsNull, string propertyName)
    {
        if (propertyIsNull)
        {
            throw new InvalidOperationException($"Property '{propertyName}' of {parent} is not initialized yet.");
        }
    }
}