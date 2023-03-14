// <copyright file="ITypedContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore.Metadata;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Interface for a context which is used to edit instances of the <see cref="RootType"/>
/// and their navigations marked with the <see cref="MemberOfAggregateAttribute"/>.
/// </summary>
internal interface ITypedContext
{
    /// <summary>
    /// Gets the root entity type.
    /// </summary>
    IEntityType RootType { get; }

    /// <summary>
    /// Determines whether the entity type of the specified clr type is included in the context.
    /// </summary>
    /// <param name="clrType">The clr type.</param>
    bool IsIncluded(Type clrType);

    /// <summary>
    /// Determines whether the entity type contains a back reference to the <see cref="RootType"/> or one of it's included types.
    /// </summary>
    /// <param name="clrType">Type of the color.</param>
    /// <remarks>
    /// These types are included in the context, but their navigations are not resolved.
    /// </remarks>
    bool IsBackReference(Type clrType);
}