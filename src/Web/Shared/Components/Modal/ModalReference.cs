// <copyright file="ModalReference.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// Implementation of <see cref="IModalReference"/>.
/// </summary>
public class ModalReference : IModalReference
{
    private readonly TaskCompletionSource<ModalResult> _tcs = new();

    /// <inheritdoc />
    public Task<ModalResult> Result => this._tcs.Task;

    /// <summary>
    /// Tries to set the result of the modal.
    /// </summary>
    /// <param name="result">The result.</param>
    internal void TrySetResult(ModalResult result)
    {
        this._tcs.TrySetResult(result);
    }
}
