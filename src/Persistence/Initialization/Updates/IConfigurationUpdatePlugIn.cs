// <copyright file="IConfigurationUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An interface for a plug in which provides data updates for a <see cref="IDataInitializationPlugIn"/>.
/// </summary>
[Guid("C4DB0C18-84DE-40DE-BD4C-22D884FB3790")]
[PlugInPoint("Configuration update", "Provides updates for initialized data.")]
public interface IConfigurationUpdatePlugIn : IStrategyPlugIn<Guid>
{
    /// <summary>
    /// Gets the data initialization key to which this update belongs.
    /// </summary>
    string DataInitializationKey { get; }

    /// <summary>
    /// Gets the name of the update.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description about the update.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets a value indicating whether this update is mandatory and will be
    /// installed automatically without asking the user.
    /// </summary>
    bool IsMandatory { get; }

    /// <summary>
    /// Gets the creation date of the update (at development).
    /// This date never changes, not even when <see cref="Version"/> is increased.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the date of the last change of the update (at development).
    /// Set it to today when <see cref="Version"/> is increased.
    /// It determines the order in which independent pending updates are applied.
    /// By default, it's the creation date.
    /// </summary>
    DateTime UpdatedAt => CreatedAt;

    /// <summary>
    /// Gets the version of the update. Increase it when an already released
    /// update is changed, so that databases which installed a previous version
    /// are offered the update again. The <see cref="Type.GUID"/> of the
    /// implementation stays the same; it remains the identity of the update.
    /// </summary>
    int Version => 1;

    /// <summary>
    /// Gets the keys of updates that must already be installed (or included in the
    /// same batch) before this update is applied. Empty for the common case of an
    /// update with no real dependency on another.
    /// </summary>
    IEnumerable<Guid> DependsOn => [];

    /// <summary>
    /// Applies this update on the given persistence context.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration which can be updated.</param>
    /// <remarks>
    /// Calling <see cref="IContext.SaveChangesAsync"/> is not required in this implementation.
    /// It will be called by <see cref="DataUpdateService"/>.
    /// </remarks>
    ValueTask ApplyUpdateAsync(IContext context, GameConfiguration gameConfiguration);
}