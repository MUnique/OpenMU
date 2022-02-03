// <copyright file="BoundingClientRect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Interop;

/// <summary>
/// A class which maps the object which comes back from the javascript function
/// Element.getBoundingClientRect().
/// </summary>
public class BoundingClientRect
{
    /// <summary>
    /// Gets or sets the x coordinate of the element in the viewport.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the x coordinate of the element in the viewport.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Gets or sets the width of the element.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the element.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Gets or sets the distance of the element from the top edge of the viewport.
    /// </summary>
    public double Top { get; set; }

    /// <summary>
    /// Gets or sets the distance of the element from the right edge of the viewport.
    /// </summary>
    public double Right { get; set; }

    /// <summary>
    /// Gets or sets the distance of the element from the bottom edge of the viewport.
    /// </summary>
    public double Bottom { get; set; }

    /// <summary>
    /// Gets or sets the distance of the element from the left edge of the viewport.
    /// </summary>
    public double Left { get; set; }
}