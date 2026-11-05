// <copyright file="CollapsedByDefaultAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Marks a collection property to be shown collapsed on the UI at first, whatever the number of its
/// items - for a configuration with many short lists, which would otherwise make one endless page.
/// </summary>
/// <remarks>
/// Without the attribute, a list starts collapsed only when it holds more than a few items.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CollapsedByDefaultAttribute : Attribute
{
}
