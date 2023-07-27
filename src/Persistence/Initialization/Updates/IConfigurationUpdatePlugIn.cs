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
public interface IConfigurationUpdatePlugIn : IStrategyPlugIn<int>
{
    /// <summary>
    /// Gets the version number of the update. This must be unique over all <see cref="DataInitializationKey"/>s.
    /// </summary>
    UpdateVersion Version { get; }

    /// <summary>
    /// Gets the <see cref="IDataInitializationPlugIn.Key"/> to which this update belongs to.
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
    /// </summary>
    DateTime CreatedAt { get; }

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