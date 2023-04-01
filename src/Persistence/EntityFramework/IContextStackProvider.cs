// <copyright file="IContextStackProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Interface for a class which provides a <see cref="IContextStack"/>.
/// </summary>
internal interface IContextStackProvider
{
    /// <summary>
    /// Gets the context stack.
    /// </summary>
    IContextStack ContextStack { get; }
}