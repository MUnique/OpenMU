// <copyright file="IModalService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Modal;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Service for showing modal dialogs.
/// </summary>
public interface IModalService
{
    /// <summary>
    /// Shows a modal dialog with the specified component.
    /// </summary>
    /// <typeparam name="TComponent">The type of the component to render in the modal.</typeparam>
    /// <param name="title">The title.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="options">The options.</param>
    /// <returns>A reference to the modal.</returns>
    IModalReference Show<TComponent>(string title, ModalParameters? parameters = null, ModalOptions? options = null)
        where TComponent : class, IComponent;

    /// <summary>
    /// Shows a modal dialog with the specified component.
    /// </summary>
    /// <param name="componentType">Type of the component to render.</param>
    /// <param name="title">The title.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="options">The options.</param>
    /// <returns>A reference to the modal.</returns>
    IModalReference Show(Type componentType, string title, ModalParameters? parameters = null, ModalOptions? options = null);
}
