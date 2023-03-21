// <copyright file="IContextAwareRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// A <see cref="IRepositoryProvider"/> which is aware of a current context by
/// implementing <see cref="IContextStackProvider"/>.
/// </summary>
internal interface IContextAwareRepositoryProvider : IRepositoryProvider, IContextStackProvider
{
}