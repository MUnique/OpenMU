// <copyright file="NonCachingRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A repository provider which does only provide non-caching repositories.
/// </summary>
internal class NonCachingRepositoryProvider : RepositoryProvider
{
    private readonly IContextAwareRepositoryProvider? _parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonCachingRepositoryProvider"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="changeListener">The change publisher.</param>
    /// <param name="contextStack">The context stack.</param>
    public NonCachingRepositoryProvider(ILoggerFactory loggerFactory, IContextAwareRepositoryProvider? parent, IConfigurationChangeListener? changeListener, IContextStack contextStack)
        : base(loggerFactory, changeListener, contextStack)
    {
        this._parent = parent;
    }

    /// <summary>
    /// Registers the repositories.
    /// </summary>
    protected override void Initialize()
    {
        this.RegisterRepository(new GameConfigurationRepository(this._parent ?? this, this.LoggerFactory, this.ChangeListener));

        this.RegisterRepository(new AccountRepository(this._parent ?? this, this.LoggerFactory)); // never cache accounts, so we pass this
        this.RegisterRepository(new LetterBodyRepository(this._parent ?? this, this.LoggerFactory)); // never cache letters, so we pass this

        this.RegisterMissingRepositoriesAsGeneric();
        base.Initialize();
    }

    /// <summary>
    /// Registers generic repositories for all types of the data model which are not registered otherwise.
    /// </summary>
    protected void RegisterMissingRepositoriesAsGeneric()
    {
        var modelTypes = EntityDataContext.CompleteModel.GetEntityTypes().Select(e => e.ClrType);
        foreach (var type in modelTypes)
        {
            // Checked against the current registrations instead of a snapshot of them,
            // because the registration of a previous type may already cover this one.
            if (type.BaseType is null
                || this.Repositories.ContainsKey(type)
                || this.Repositories.ContainsKey(type.BaseType))
            {
                continue;
            }

            var repository = this.CreateGenericRepository(type, this._parent ?? this);
            this.RegisterRepository(type, repository);
        }
    }
}