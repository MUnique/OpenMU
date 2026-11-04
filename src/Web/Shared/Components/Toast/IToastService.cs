// <copyright file="IToastService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Toast;

using System;
using System.Collections.Generic;

/// <summary>
/// Service for showing toast notifications.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Occurs when the list of toasts has changed (added, closed, cleared).
    /// </summary>
    event Action? StateChanged;

    /// <summary>
    /// Gets the currently shown toasts.
    /// </summary>
    IReadOnlyList<ToastInstance> Toasts { get; }

    /// <summary>
    /// Shows a success toast.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="heading">The optional heading.</param>
    void ShowSuccess(string message, string? heading = null);

    /// <summary>
    /// Shows an info toast.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="heading">The optional heading.</param>
    void ShowInfo(string message, string? heading = null);

    /// <summary>
    /// Shows a warning toast.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="heading">The optional heading.</param>
    void ShowWarning(string message, string? heading = null);

    /// <summary>
    /// Shows an error toast.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="heading">The optional heading.</param>
    void ShowError(string message, string? heading = null);

    /// <summary>
    /// Closes the specified toast (triggers its closing animation).
    /// </summary>
    /// <param name="toast">The toast to close.</param>
    void Close(ToastInstance toast);

    /// <summary>
    /// Closes all currently shown toasts.
    /// </summary>
    void Clear();
}
