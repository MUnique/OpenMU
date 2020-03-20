// <copyright file="AggregateRootAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Composition
{
    using System;

    /// <summary>
    /// Marks a class as an aggregate root.
    /// An instance of an aggregate root is an object which can stand on its own
    /// and consists of several other objects which are marked with the <see cref="MemberOfAggregateAttribute"/>.
    /// Example: A car (aggregate root) which consists of several other parts, e.g. engine, wheels, doors etc.
    /// Properties which are not part of the aggregate will not be marked with the <see cref="MemberOfAggregateAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AggregateRootAttribute : Attribute
    {
    }
}
