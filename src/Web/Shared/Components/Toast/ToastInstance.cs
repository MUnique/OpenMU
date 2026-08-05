// <copyright file="ToastInstance.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Toast;

using System;

/// <summary>
/// Represents a single toast message shown in the <see cref="ToastContainer"/>.
/// </summary>
public sealed class ToastInstance
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToastInstance"/> class.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="message">The message.</param>
    /// <param name="heading">The optional heading.</param>
    internal ToastInstance(ToastLevel level, string message, string? heading)
    {
        this.Key = Guid.NewGuid();
        this.Level = level;
        this.Message = message;
        this.Heading = heading;
    }

    /// <summary>
    /// Gets a stable key identifying this toast, used as a render key.
    /// </summary>
    public Guid Key { get; }

    /// <summary>
    /// Gets the level.
    /// </summary>
    public ToastLevel Level { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the optional heading.
    /// </summary>
    public string? Heading { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the toast is performing its closing animation.
    /// </summary>
    internal bool IsClosing { get; set; }
}