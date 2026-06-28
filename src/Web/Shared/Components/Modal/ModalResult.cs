// <copyright file="ModalResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// Represents the result of a modal dialog.
/// </summary>
public class ModalResult
{
    private ModalResult(object? data, bool cancelled)
    {
        this.Data = data;
        this.Cancelled = cancelled;
    }

    /// <summary>
    /// Gets a value indicating whether the modal was cancelled.
    /// </summary>
    public bool Cancelled { get; }

    /// <summary>
    /// Gets the data returned by the modal, if not cancelled.
    /// </summary>
    public object? Data { get; }

    /// <summary>
    /// Creates a successful <see cref="ModalResult"/> with the specified data.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="data">The data.</param>
    /// <returns>A new <see cref="ModalResult"/>.</returns>
    public static ModalResult Ok<T>(T data) => new(data, false);

    /// <summary>
    /// Creates a cancelled <see cref="ModalResult"/>.
    /// </summary>
    /// <returns>A new <see cref="ModalResult"/>.</returns>
    public static ModalResult Cancel() => new(null, true);
}
