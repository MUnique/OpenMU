// <copyright file="ILoadingStatusAware.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Interface for a loading status aware class.
/// </summary>
internal interface ILoadingStatusAware
{
    /// <summary>
    /// Gets or sets the loading status.
    /// </summary>
    LoadingStatus LoadingStatus { get; set; }
}