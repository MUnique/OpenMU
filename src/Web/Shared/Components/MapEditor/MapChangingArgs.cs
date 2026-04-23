// <copyright file="MapChangingArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.ComponentModel;

/// <summary>
/// Provides event data for the map changing event, supporting cancellation.
/// </summary>
public sealed class MapChangingArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapChangingArgs"/> class.
    /// </summary>
    /// <param name="nextMap">The ID of the map being switched to.</param>
    public MapChangingArgs(Guid nextMap)
    {
        this.NextMap = nextMap;
    }

    /// <summary>Gets the ID of the map that is about to be selected.</summary>
    public Guid NextMap { get; }
}
