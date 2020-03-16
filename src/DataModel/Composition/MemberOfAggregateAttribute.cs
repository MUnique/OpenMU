// <copyright file="MemberOfAggregateAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Composition
{
    using System;

    /// <summary>
    /// Marks a property as a member of an aggregate.
    /// The declaring type owns the objects of the marked property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MemberOfAggregateAttribute : Attribute
    {
    }
}