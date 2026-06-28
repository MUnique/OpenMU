// <copyright file="BoundingClientRect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

/// <summary>
/// Represents the bounding client rectangle of an HTML element.
/// </summary>
public sealed class BoundingClientRect
{
    /// <summary>Gets or sets the left-edge position relative to the viewport.</summary>
    public double Left { get; set; }

    /// <summary>Gets or sets the top-edge position relative to the viewport.</summary>
    public double Top { get; set; }
}
