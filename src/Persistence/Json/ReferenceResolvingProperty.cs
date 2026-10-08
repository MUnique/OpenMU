// <copyright file="ReferenceResolvingProperty.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Describes a json property which is read by a <see cref="ReferenceResolvingConverterBase{T}"/>.
/// </summary>
/// <param name="Name">The name of the json property. It's matched case-insensitive.</param>
/// <param name="PropertyType">The type of the value, or of the items when it's a collection.</param>
/// <param name="IsCollection">If set to <c>true</c>, the json value is an array of which every item is added to a collection.</param>
public readonly record struct ReferenceResolvingProperty(string Name, Type PropertyType, bool IsCollection);
