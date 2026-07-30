// <copyright file="ModalInstance.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// Instance of a modal dialog, passed to the modal content so it can close itself.
/// </summary>
public class ModalInstance
{
    private readonly ModalReference _reference;
    private readonly Action _onClose;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModalInstance"/> class.
    /// </summary>
    /// <param name="reference">The modal reference.</param>
    /// <param name="onClose">The close action.</param>
    internal ModalInstance(ModalReference reference, Action onClose)
    {
        this._reference = reference;
        this._onClose = onClose;
    }

    /// <summary>
    /// Closes the modal with the specified result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CloseAsync(ModalResult result)
    {
        this._reference.TrySetResult(result);
        this._onClose();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Cancels the modal.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CancelAsync()
    {
        return this.CloseAsync(ModalResult.Cancel());
    }
}
