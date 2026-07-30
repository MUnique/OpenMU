// <copyright file="IModalReference.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// Reference to an opened modal dialog.
/// </summary>
public interface IModalReference
{
    /// <summary>
    /// Gets the task that completes when the modal is closed.
    /// </summary>
    Task<ModalResult> Result { get; }
}
