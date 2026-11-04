// <copyright file="ScrollInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

/// <summary>
/// Represents the scroll position of an HTML element.
/// </summary>
public sealed class ScrollInfo
{
    /// <summary>Gets or sets the horizontal scroll offset in pixels.</summary>
    public double ScrollLeft { get; set; }

    /// <summary>Gets or sets the vertical scroll offset in pixels.</summary>
    public double ScrollTop { get; set; }
}
