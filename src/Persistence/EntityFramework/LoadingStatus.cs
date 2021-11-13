// <copyright file="LoadingStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// The loading status of a <see cref="ILoadingStatusAware"/>.
/// </summary>
internal enum LoadingStatus
{
    /// <summary>
    /// The undefined state.
    /// </summary>
    Undefined,

    /// <summary>
    /// The data is not loaded yet.
    /// </summary>
    Unloaded,

    /// <summary>
    /// The data is loaded.
    /// </summary>
    Loaded,

    /// <summary>
    /// The data is loading at the moment.
    /// </summary>
    Loading,

    /// <summary>
    /// The loading of the data failed.
    /// </summary>
    Failed,
}