// <copyright file="CachingEntityFrameworkContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementation of <see cref="IContext"/> for the entity framework <see cref="PersistenceContextProvider"/>.
/// This class serves as a concrete type for instantiation and provides a convenience constructor that defaults to ownership mode.
/// </summary>
/// <remarks>
/// While this class doesn't add functionality beyond <see cref="EntityFrameworkContextBase"/>, it provides:
/// - A specific type for dependency injection and logging
/// - A convenience constructor with sensible defaults (isOwner=true)
/// - Type identification in the repository provider system
/// Removing it would require refactoring all call sites to use EntityFrameworkContextBase directly with explicit parameters.
/// </remarks>
internal class CachingEntityFrameworkContext : EntityFrameworkContextBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CachingEntityFrameworkContext" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="changeListener">The change listener.</param>
    /// <param name="logger">The logger.</param>
    public CachingEntityFrameworkContext(DbContext context, IContextAwareRepositoryProvider repositoryProvider, IConfigurationChangeListener? changeListener, ILogger<CachingEntityFrameworkContext> logger)
        : base(context, repositoryProvider, true, changeListener, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingEntityFrameworkContext" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="isOwner">if set to <c>true</c> this instance owns the <paramref name="context" />.</param>
    /// <param name="changeListener">The change listener.</param>
    /// <param name="logger">The logger.</param>
    public CachingEntityFrameworkContext(DbContext context, IContextAwareRepositoryProvider repositoryProvider, bool isOwner, IConfigurationChangeListener? changeListener, ILogger<CachingEntityFrameworkContext> logger)
        : base(context, repositoryProvider, isOwner, changeListener, logger)
    {
    }
}