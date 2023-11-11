// <copyright file="EntityFrameworkContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A implementation of <see cref="EntityFrameworkContextBase"/> which doesn't cache and always asks the database for objects.
/// </summary>
internal class EntityFrameworkContext : EntityFrameworkContextBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFrameworkContext" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="isOwner">If set to <c>true</c>, this instance owns the <see cref="EntityFrameworkContextBase.Context" />. That means it will be disposed when this instance will be disposed.</param>
    /// <param name="changeListener">The change listener.</param>
    public EntityFrameworkContext(DbContext context, ILoggerFactory loggerFactory, IContextAwareRepositoryProvider repositoryProvider, bool isOwner, IConfigurationChangeListener? changeListener)
        : base(context, repositoryProvider, isOwner, changeListener, loggerFactory.CreateLogger<EntityFrameworkContext>())
    {
    }
}