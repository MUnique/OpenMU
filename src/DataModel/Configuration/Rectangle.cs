﻿// <copyright file="Rectangle.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Defines a rectangle.
/// </summary>
public class Rectangle
{
    /// <summary>
    /// Gets or sets the upper left corner, x-coordinate.
    /// </summary>
    public byte X1 { get; set; }

    /// <summary>
    /// Gets or sets the upper left corner, y-coordinate.
    /// </summary>
    public byte Y1 { get; set; }

    /// <summary>
    /// Gets or sets the bottom right corner, x-coordinate.
    /// </summary>
    public byte X2 { get; set; }

    /// <summary>
    /// Gets or sets the bottom right corner, y-coordinate.
    /// </summary>
    public byte Y2 { get; set; }
}