// <copyright file="CachingEntityFrameworkContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementation of <see cref="IContext"/> for the entity framework <see cref="PersistenceContextProvider"/>.
/// </summary>
internal class CachingEntityFrameworkContext : EntityFrameworkContextBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CachingEntityFrameworkContext" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="logger">The logger.</param>
    public CachingEntityFrameworkContext(DbContext context, RepositoryManager repositoryManager, ILogger<CachingEntityFrameworkContext> logger)
        : base(context, repositoryManager, true, null, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingEntityFrameworkContext" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="isOwner">if set to <c>true</c> this instance owns the <paramref name="context" />.</param>
    /// <param name="logger">The logger.</param>
    public CachingEntityFrameworkContext(DbContext context, RepositoryManager repositoryManager, bool isOwner, ILogger<CachingEntityFrameworkContext> logger)
        : base(context, repositoryManager, isOwner, null, logger)
    {
    }
}