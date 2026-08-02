// <copyright file="ModalOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// Options for a modal dialog.
/// </summary>
public class ModalOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the modal can be cancelled by clicking the background.
    /// </summary>
    public bool DisableBackgroundCancel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the close button is hidden.
    /// </summary>
    public bool HideCloseButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the header is hidden.
    /// </summary>
    public bool HideHeader { get; set; }

    /// <summary>
    /// Gets or sets the size of the modal.
    /// </summary>
    public ModalSize Size { get; set; } = ModalSize.Default;
}
